using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Alcantara.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Alcantara.ViewModels;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Alcantara.Controllers
{ 
    public class HomeController : Controller
    {
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly UserDBContext DBContext;
        private readonly UserManager<User> userManager;
        private readonly int CacheAgeSeconds = 60 * 60 * 24 * 30; //IMG Cach Max time
        private readonly int ProductsCountInPage = 32;
        private static object orderLock = new object();
        private static object orderNumLock = new object();
        public HomeController(IStringLocalizer<HomeController> stringLocalizer,UserDBContext userDBContext, UserManager<User> _userManager)
        {
            DBContext = userDBContext;
            this._localizer = stringLocalizer;
            userManager = _userManager;
        }
        public IActionResult ErrorPage() {
            return View();
        }
        public async Task<IActionResult> Index()
        {
            string Culture = Request.HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.Culture.Name;
            ViewBag.Culture = Culture;
            var m = await DBContext.HomePageSections.OrderBy(_ => _._Index).ToListAsync();
            if (m != null && m.Count() == 7 && m.ElementAt(6).HomePageSectionDatas!=null)
            {
                var TempCatalog = m.ElementAt(6).HomePageSectionDatas.ElementAt(0)?.Catalog;
                if (TempCatalog != null)
                {
                    ViewBag.Products = TempCatalog?.Products?.OrderBy(_=>_._Index).Where(_ => _.IsActive).Take(20).ToList();
                }
            }
            ViewBag._Model = m;
            return View();
        }
        #region Products
        public async Task<IActionResult> Products(string id, string brand = null, IEnumerable<string> atributeIds = null, string serchKeys=null)
        {
            string Culture = Request.HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.Culture.Name;
            ViewBag.Culture = Culture;
            ViewBag.SerchKeys = serchKeys;
            ViewBag.amd = getAMD();
            List<ProductView> ProductList = new List<ProductView>();
            if (!string.IsNullOrEmpty(id) || (serchKeys != null && serchKeys.Count() > 0))
            {
                Catalog catalog = null;
                if (!string.IsNullOrEmpty(id)) catalog = await DBContext.Catalogs.FirstOrDefaultAsync(_ => _.Id == id);
                else if (serchKeys != null && serchKeys.Count() > 0) {
                    catalog = new Catalog() { ChaildCatalogs = await DBContext.Catalogs.Where(_ => _.FatherCatalog == null).ToListAsync(), Name = "All" };
                }
                if (catalog != null)
                {
                    #region Products
                    ViewBag.Catalog = catalog;

                    var products = GetProducts(catalog, brand, atributeIds, serchKeys);
                    ViewBag.Atributes = GetAtributes(products);
                    IEnumerable<string> brands = GetBrands(GetProducts(catalog));
                    if (string.IsNullOrEmpty(catalog.Id) && !string.IsNullOrEmpty(serchKeys)) brands = brands?.Take(10);
                    ViewBag.Brands = brands;
                    ViewBag.CurrentBrand = brand;
                    ViewBag.CurrentAtributes = atributeIds?.Where(_ => !string.IsNullOrEmpty(_));
                    ProductList = GetProductViews(products?.Take(ProductsCountInPage));
                    #endregion
                }
            }
            ViewBag.Products = ProductList;
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> Products([FromBody]ProductLoadMoreAjax req)
        {
            try
            {
                if (req!=null && !string.IsNullOrEmpty(req.id))
                {
                    var catalog = await DBContext.Catalogs.FirstOrDefaultAsync(_ => _.Id == req.id);
                    if (catalog != null)
                    {
                        var products = GetProducts(catalog, req.brand, req.atributeIds, req.serchKeys)?.Skip(req.index * ProductsCountInPage).Take(ProductsCountInPage);
                        if(products!=null && products.Count() > 0)
                        {
                            return new JsonResult(new { res = GetProductViews(products) });
                        }
                    }
                }
                return new JsonResult(new { });
            }
            catch(Exception ex)
            {
                return new JsonResult(new { error = ex.Message });
            }
            
        }
        private List<ProductView> GetProductViews(IEnumerable<Product> products)
        {
            string Culture = Request.HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.Culture.Name;
            List<ProductView> ProductList = new List<ProductView>();
            if (products != null && products.Count() > 0)
            {
                foreach (var pr in products)
                {
                    var pt = pr?.ProductTypes?.OrderBy(_ => _._Index)?.FirstOrDefault(_ => _.IsMine);
                    var RatingList = pr?.Reviews?.Where(_ => _.Status == true)?.Select(_ => _.Rating);
                    int Rating = 0;
                    if (RatingList != null && RatingList.Count() > 0) Rating = (int)RatingList.Average();

                    var TempPT = new ProductView()
                    {
                        Id = pr.Id,
                        Rating = Rating,
                        ImgId = pt?.Images?.OrderBy(_ => _._Index).FirstOrDefault()?.Id,
                        Title = CultureData.GetDefoultName(pt.CultureTitle, Culture),
                        Price = pt.Price.ToString("N", new CultureInfo("en")).Replace(".00", String.Empty),
                        Sale = pt.Sale.ToString("N", new CultureInfo("en")).Replace(".00", String.Empty),
                        Brand = CultureData.GetDefoultName(pt.CultureBrand, Culture).Trim(),
                        LinkAtrVal = pt.LinkAtributeValue
                        //AtributeValueIds = pt.ProductAtributes.SelectMany(_ => _.AtributeValues).Select(_ => _.AtributeValue.Id).Distinct()
                    };
                    if (TempPT != null) ProductList.Add(TempPT);
                }
            }
            return ProductList;
        }
        private IEnumerable<Product> GetProducts(Catalog catalog, string Brand = null, IEnumerable<string> AtributeValueIds = null, string serchKeys = null)
        {
            if (catalog == null) return null;
            var skeys = serchKeys?.Trim().Split(' ', ',');
            string Culture = Request.HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.Culture.Name;
            List<Product> TempProducts = new List<Product>();
            var TempAtrValIds = AtributeValueIds?.Where(_ => !string.IsNullOrEmpty(_));
            if (catalog.Products != null && catalog.Products.Count() > 0)
            {
                var pr = catalog.Products.Where(_ => _.IsActive)
                    .Where(_ => _.ProductTypes != null && _.ProductTypes.Count() > 0 && _.ProductTypes.FirstOrDefault(pt => pt.IsMine).IsActive && (string.IsNullOrEmpty(Brand) || CultureData.GetDefoultName(_.ProductTypes.FirstOrDefault(pt => pt.IsMine).CultureBrand, Culture).Trim() == Brand.Trim()))
                    .Where(_ => _.ProductTypes.Any(pt =>pt.IsActive && pt.ProductAtributes.Any(atr => atr.ProductQuantity > 0)))
                    .Where(_=>TempAtrValIds==null || TempAtrValIds.Count()==0 || _.ProductTypes.Any(pt=>pt.IsActive && pt.ProductAtributes.SelectMany(pa=>pa.AtributeValues).Any(av=>TempAtrValIds.Any(tav=>tav== av.AtributeValue.Id))));
                if (pr != null && pr.Count() > 0 && skeys != null && skeys.Length > 0)
                {
                    pr = pr.Where(_ => hasKeyInProduct(_, skeys));
                }
                if (pr != null && pr.Count() > 0) TempProducts.AddRange(pr);
            }
            if(catalog.ChaildCatalogs!=null && catalog.ChaildCatalogs.Count() > 0)
            {
                foreach(var ca in catalog.ChaildCatalogs)
                {
                    var TempPr = GetProducts(ca, Brand, TempAtrValIds, serchKeys);
                    if (TempPr != null && TempPr.Count() > 0) TempProducts.AddRange(TempPr);
                }
            }
            return TempProducts.OrderBy(_ => _._Index);
        }
        private IDictionary<Atribute,List<AtributeValue>> GetAtributes(IEnumerable<Product> products)
        {
            var TempAtributes = products.SelectMany(_ => _.ProductTypes).SelectMany(_ => _.ProductAtributes)
                .SelectMany(_ => _.AtributeValues).Select(_ => _.AtributeValue).Distinct().GroupBy(_ => _.FK_Atribute).Where(_ => _.Key.IsActive).ToDictionary(_ => _.Key, _ => _.ToList());
            return TempAtributes;
        }

        private IEnumerable<string> GetBrands(IEnumerable<Product> products )
        {
            string Culture = Request.HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.Culture.Name;
            IEnumerable<string> brands = null;
            if (products!=null && products.Count() > 0)
            {
                brands = products.Select(_ => _.ProductTypes.FirstOrDefault(pt => pt.IsMine)).Where(_ => !string.IsNullOrEmpty(CultureData.GetDefoultName(_.CultureBrand, Culture)))
                    .Select(_ => CultureData.GetDefoultName(_.CultureBrand, Culture)).Distinct();
            }
            return brands;
        }
        #endregion
        public async Task<IActionResult> Product(string id,string typeId=null)
        {
            
            if (string.IsNullOrEmpty(id)) return RedirectToAction("Index");
            var product = await DBContext.Products.FirstOrDefaultAsync(_ => _.Id == id);
            if(product==null || !product.IsActive) return RedirectToAction("Index");
            ViewBag.Reviews = await DBContext.Reviews?.Where(_ => _.FK_Product.Id == id && _.Status != null && (bool)_.Status).ToArrayAsync();
            ViewBag.ID = id;
            string Culture = Request.HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.Culture.Name;
            ViewBag.Culture = Culture;
            ProductType productType = null;
            if (string.IsNullOrEmpty(typeId))
            {
                productType = product.ProductTypes.FirstOrDefault(_ => _.Id == typeId && _.IsActive);
                if (productType == null) productType = product.ProductTypes.FirstOrDefault(_ => _.IsMine && _.IsActive);
            }
            else productType = product.ProductTypes.FirstOrDefault(_ => _.IsMine && _.IsActive);

            ProductView resp = null;
            if (product != null)
            {
                resp = new ProductView()
                {
                    Id = product.Id,
                    Title = CultureData.GetDefoultName(productType.CultureTitle, Culture),
                    Description = CultureData.GetDefoultName(productType.CultureDescription, Culture),
                    Brand = CultureData.GetDefoultName(productType.CultureBrand, Culture),
                    Price = productType.Price.ToString("N", new CultureInfo("en")).Replace(".00", string.Empty),
                    Sale = productType.Sale.ToString("N", new CultureInfo("en")).Replace(".00", string.Empty),
                    Imgs = productType.Images?.OrderBy(_ => _._Index).Select(_ => _.Id),
                    Atributes = product.ProductTypes?.SelectMany(p => p.ProductAtributes).SelectMany(p => p.AtributeValues).Select(p => p.AtributeValue).Distinct().GroupBy(p => p.FK_Atribute).ToDictionary(p => p.Key, p => p.ToList()),
                    LinkAtrVal = productType.LinkAtributeValue
                };
            }
            ViewBag.Product = resp;

            return View();
        }
        [HttpPost]
        public async Task<JsonResult> getAtributes(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            var product = await DBContext.Products.FirstOrDefaultAsync(_ => _.Id == id);
            if (product == null) return null;
            var atr = product.ProductTypes?.FirstOrDefault(_ => _.IsMine && _.IsActive)?.ProductAtributes.Select(_ => _.AtributeValues.Select(av=>av.AtributeValue.Id));
            return new JsonResult(atr);
        }
        [HttpPost]
        public async Task<JsonResult> getProductData(string id,string atrVal)
        {
            try
            {


                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(atrVal)) return new JsonResult(new { });
                var product = await DBContext.Products.FirstOrDefaultAsync(_ => _.Id == id);
                if (product == null) return new JsonResult(new { });
                if (product.LinkAtribute == null || !product.LinkAtribute.Values.Any(_ => _.Id == atrVal)) new JsonResult(new { });
                var productType = product.ProductTypes.FirstOrDefault(_ => _.LinkAtributeValue != null && _.IsActive && _.LinkAtributeValue.Id == atrVal);
                if (productType == null) return new JsonResult(new { });
                string Culture = Request.HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.Culture.Name;
                return new JsonResult(new
                {
                    imgs = productType?.Images.Select(_ => _.Id),
                    title = CultureData.GetDefoultName(productType.CultureTitle, Culture),
                    description = CultureData.GetDefoultName(productType.CultureDescription, Culture),
                    brand = CultureData.GetDefoultName(productType.CultureBrand, Culture),
                    sale = productType.Sale,
                    price = productType.Price
                });
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        [Authorize]
        [HttpPost]
        public async Task<JsonResult> Review(int Rating, string Description, string ProductId)
        {
            try
            {
                if (Rating < 0 || Rating > 5 || string.IsNullOrEmpty(Description) || string.IsNullOrEmpty(ProductId))return new JsonResult(new { err="Incorrect Data."}) ;
                var user = await userManager.GetUserAsync(User);
                var product = await DBContext.Products.FirstOrDefaultAsync(_ => _.Id == ProductId);
                if (user != null && product != null) 
                {
                    bool? TempStatus = null;
                    var gs = await DBContext.GlobalSetings.FirstOrDefaultAsync();
                    if (gs != null && gs.EnableAllReviews) TempStatus = true;
                    DBContext.Reviews.Add(new Models.Review() { Description = Description, Rating = Rating, Status = TempStatus, FK_Product = product, FK_User = user, Date = DateTime.Now });
                    if (await DBContext.SaveChangesAsync() > 0) return new JsonResult(new { res = true });
                }
                return new JsonResult(new { err = "Incorrect Data." });
            }
            catch(Exception)
            {
                return new JsonResult(new { err = "Exception" });
            }
        }
        public async Task<IActionResult> Favorites()
        {
            try
            {
                string Culture = Request.HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.Culture.Name;
                ViewBag.Culture = Culture;
                ViewBag.amd = getAMD();
                var priceCulture = new CultureInfo("en");
                string idsJson = Request.Cookies["FavoritIds"];
                IEnumerable<favoriteJson> ids = null;
                if (!string.IsNullOrEmpty(idsJson)) JArray.Parse(idsJson).ToObject<IEnumerable<favoriteJson>>();
                if (ids != null && ids.Count() > 0)
                {
                    var TempProducts = await DBContext.Products.Where(_ => ids.Any(atr => atr.id == _.Id)).Where(_ => _.IsActive && _.ProductTypes.FirstOrDefault(pt => pt.IsMine) != null && _.ProductTypes.First(pt => pt.IsMine).IsActive).ToListAsync();
                    if (TempProducts != null)
                    {
                        if (TempProducts.Count != ids.Count())
                        {
                            var newIds = TempProducts.Select(_ => _.Id);
                            Response.Cookies.Append("FavoritIds", JsonConvert.SerializeObject(newIds));
                        }
                        ViewBag.Products = TempProducts.Select((_) =>
                        {
                            var pt = _.ProductTypes.FirstOrDefault(p => p.IsMine);
                            return new ProductView()
                            {
                                Id = _.Id,
                                ImgId = pt?.Images?.FirstOrDefault()?.Id,
                                Price = pt?.Price.ToString("N", priceCulture).Replace(".00", string.Empty),
                                Sale = pt?.Sale.ToString("N", priceCulture).Replace(".00", string.Empty),
                                Title = CultureData.GetDefoultName(pt?.CultureTitle, Culture),
                                LinkAtrVal = pt.LinkAtributeValue,
                                Atributes = _.ProductTypes?.SelectMany(p => p.ProductAtributes).SelectMany(p => p.AtributeValues).Select(p => p.AtributeValue).Distinct().GroupBy(p => p.FK_Atribute).ToDictionary(p => p.Key, p => p.ToList())
                            };
                        });
                    }
                }
                return View();
            }
            catch (Exception)
            {
                ViewBag.Products = null;
                return View();
            }
        }
        public async Task<IActionResult> Bag()
        {
            try
            {
                string Culture = Request.HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.Culture.Name;
                ViewBag.Culture = Culture;
                ViewBag.amd = getAMD();
                var gs = await DBContext.GlobalSetings.FirstOrDefaultAsync();
                ViewBag.ShippingOrderSum = gs?.ShippingOrderSum?? 0;
                ViewBag.shippingWhenMore =gs?.ShippingWhenMore?? 0;
                ViewBag.shippingWhenLess =gs?.ShippingWhenLess?? 0;
                ViewBag.Shipping = 0;
                var priceCulture = new CultureInfo("en");
                string idsJson = Request.Cookies["ProductList"];
                IEnumerable<ProductJson> ids = null;
                if (!string.IsNullOrEmpty(idsJson)) JArray.Parse(idsJson).ToObject<IEnumerable<ProductJson>>();
                if (ids == null || ids.Count() == 0) { Response.Cookies.Append("ProductList", ""); return View(); }
                var products = await DBContext.Products.Where(_ => ids.Any(prId => prId.id == _.Id)).ToListAsync();
                if (products == null || products.Count() == 0) { Response.Cookies.Append("ProductList", ""); return View(); }

                List<ProductBagViewModel> resp = new List<ProductBagViewModel>();

                foreach(var pr in products)
                {
                    IEnumerable<ProductJson> reqEls = ids.Where(_ => _.id == pr.Id);
                    if(reqEls!=null && reqEls.Count() > 0)
                    {
                        foreach(var reqEl in reqEls)
                        {
                            var mainPrType = pr.ProductTypes.FirstOrDefault(_ => _.IsActive && _.IsMine);
                            var prTypeAtrs = mainPrType?.ProductAtributes;
                            if(prTypeAtrs!=null && prTypeAtrs.Count() > 0)
                            {
                                foreach(var prTypeAtr in prTypeAtrs)
                                {
                                    var atrValIds = prTypeAtr.AtributeValues.Select(_ => _.AtributeValue.Id);
                                    if (atrValIds.Count()==reqEl.atrs.Count() && atrValIds.All(_ => reqEl.atrs.Any(r => r == _)))
                                    {
                                        string imgId = mainPrType?.Images?.FirstOrDefault()?.Id;
                                        ICollection<CultureData> cultureDatas = mainPrType?.CultureTitle;
                                        decimal price = mainPrType.Price;
                                        decimal sale = mainPrType.Sale;
                                        if (pr.LinkAtribute != null)
                                        {
                                            var pt = pr.ProductTypes.FirstOrDefault(_ => reqEl.atrs.Any(id => id == _.LinkAtributeValue?.Id));
                                            if (pt != null)
                                            {
                                                if (pt.Images != null && pt.Images.Count() > 0) imgId = pt.Images.FirstOrDefault()?.Id;
                                                if (!string.IsNullOrEmpty(CultureData.GetDefoultName(pt.CultureTitle, Culture))) cultureDatas = pt.CultureTitle;
                                                if (pt.Price != 0 || pt.Sale != 0)
                                                {
                                                    price = pt.Price;
                                                    sale = pt.Sale;
                                                }
                                            }
                                        }
                                        resp.Add(new ProductBagViewModel()
                                        {
                                            ProductId = pr.Id,
                                            Title = CultureData.GetDefoultName(cultureDatas, Culture),
                                            MaxQuantity = prTypeAtr.ProductQuantity,
                                            Quantity = reqEl.count,
                                            Atributes = prTypeAtr.AtributeValues.Select(_ => _.AtributeValue),
                                            Price = price.ToString("N", priceCulture).Replace(".00", ""),
                                            dPrice=price,
                                            dSale=sale,
                                            Sale = sale.ToString("N", priceCulture).Replace(".00", ""),
                                            ImgId = imgId
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
                ViewBag.Products = resp;
                
                if (resp != null && resp.Count() > 0) ViewBag.Shipping = await shippingSum(resp.Sum(_ => _.dPrice));
                return View();
            }
            catch (Exception)
            {
                ViewBag.Products = null;
                return View();
            }
            
        }
        public async Task<IActionResult> Checkout(string promoCode)
        {
            PromoCode promo = null;
            if (!string.IsNullOrEmpty(promoCode))
            {
                promo = await DBContext.PromoCodes.FirstOrDefaultAsync(_ => _.Name.ToUpper() == promoCode.ToUpper());
                if (promo == null) promoCode = "";
            }
            ViewBag.promoCode = promoCode;
            ViewBag.amd = getAMD();
            ViewBag.PromoSale = 0;
            ViewBag.OrderValue = 0;
            string idsJson = Request.Cookies["ProductList"];
            IEnumerable<ProductJson> ids = JArray.Parse(idsJson).ToObject<IEnumerable<ProductJson>>();
            if (ids == null || ids.Count() == 0) return RedirectToAction("Bag");
            var products = await DBContext.Products.Where(_ => ids.Any(prId => prId.id == _.Id)).ToListAsync();
            if (products == null || products.Count() == 0) return RedirectToAction("Bag");
            decimal sum = 0;
            foreach (var pr in products)
            {
                IEnumerable<ProductJson> reqEls = ids.Where(_ => _.id == pr.Id);
                if (reqEls != null && reqEls.Count() > 0)
                {
                    foreach (var reqEl in reqEls)
                    {
                        var mainPrType = pr.ProductTypes.FirstOrDefault(_ => _.IsActive && _.IsMine);
                        var prTypeAtrs = mainPrType?.ProductAtributes;
                        if (prTypeAtrs != null && prTypeAtrs.Count() > 0)
                        {
                            foreach (var prTypeAtr in prTypeAtrs)
                            {
                                var atrValIds = prTypeAtr.AtributeValues.Select(_ => _.AtributeValue.Id);
                                if (atrValIds.Count() == reqEl.atrs.Count() && atrValIds.All(_ => reqEl.atrs.Any(r => r == _)))
                                {
                                    decimal price = mainPrType.Price;
                                    decimal sale = mainPrType.Sale;
                                    if (pr.LinkAtribute != null)
                                    {
                                        var pt = pr.ProductTypes.FirstOrDefault(_ => reqEl.atrs.Any(id => id == _.LinkAtributeValue?.Id));
                                        if (pt != null)
                                        {
                                            if (pt.Price != 0 || pt.Sale != 0)
                                            {
                                                price = pt.Price;
                                                sale = pt.Sale;
                                            }
                                        }
                                    }
                                    sum += reqEl.count * (sale != 0 ? sale : price);
                                }
                            }
                        }
                    }
                }
            }
            //-------------Promo Sale----------------------
            decimal promoSale = 0;
            if (!string.IsNullOrEmpty(promoCode))
            {
                if (promo != null && promo.Expired > DateTime.Now && promo.isActive)
                {
                    promoSale = promo.SalePercent;
                }
            }
            //---------------------------------------------
            ViewBag.PromoSale = promoSale;
            ViewBag.OrderValue = sum;
            ViewBag.Shipping = await shippingSum(sum * (1 - promoSale / 100));
            CheckoutViewModel model = new CheckoutViewModel();
            if (User.Identity.IsAuthenticated)
            {
                var user = await userManager.GetUserAsync(User);
                model.Phone = user?.PhoneNumber??"";
                model.Address = (user?.Country==null?"":user.Country + (string.IsNullOrEmpty(user.City) ? "" : (" " + user?.City)) + (string.IsNullOrEmpty(user?.Address) ? "" : (" " + user.Address))).TrimStart();
                model.Email = user?.Email;
                model.Description = (user?.FirstName??"" + " " + user?.LastName??"").Trim();
                if (user?.SelectedAddress != null)
                {
                    var adr = user.SelectedAddress;
                    model.Address = ((adr?.Country??"") + (string.IsNullOrEmpty(adr?.City) ? "" : (" " + adr.City)) + (string.IsNullOrEmpty(adr?.Country) ? "" : (" " + adr.Country)) + (string.IsNullOrEmpty(adr?.Address) ? "" : (" " + adr.Address))).TrimStart();
                    model.Description = (adr?.FName??"" + " " + adr?.LName??"").Trim();
                }
            }
            ViewBag.CheckoutViewModel = model;
            return View();
        }
        public async Task<decimal> shippingSum(decimal orderSum)
        {
            var gs =await DBContext.GlobalSetings.FirstOrDefaultAsync();
            if (gs == null) return 0;
            if (orderSum > gs.ShippingOrderSum) return gs.ShippingWhenMore;
            return gs.ShippingWhenLess;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel _m)
        {
            try
            {
                ViewBag.CheckoutViewModel = _m;
                ViewBag.PromoSale = 0;
                ViewBag.OrderValue = 0;
                ViewBag.Shipping = 0;
                if (ModelState.IsValid)
                {
                    string Culture = Request.HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.Culture.Name;
                    ViewBag.Culture = Culture;
                
                    string idsJson = Request.Cookies["ProductList"];
                    IEnumerable<ProductJson> ids = JArray.Parse(idsJson).ToObject<IEnumerable<ProductJson>>();
                    if (ids == null || ids.Count() == 0)
                    {
                        ViewBag.ModelError= "Bag is empty.";
                        return View();
                    }
                    var products = await DBContext.Products.Where(_ => ids.Any(prId => prId.id == _.Id)).ToListAsync();
                    if (products == null || products.Count() == 0)
                    {
                        ViewBag.ModelError = "Incorrect data. Please clear browser history and try again.";
                        return View();
                    }
                    decimal ProductSum = 0;
                    List<OrderProductInfo> OrderProductInfo = new List<OrderProductInfo>();
                    lock (orderLock)
                    {
                        foreach (var pr in products)
                        {
                            IEnumerable<ProductJson> reqEls = ids.Where(_ => _.id == pr.Id);
                            if (reqEls != null && reqEls.Count() > 0)
                            {
                                foreach (var reqEl in reqEls)
                                {
                                    var mainPrType = pr.ProductTypes.FirstOrDefault(_ => _.IsActive && _.IsMine);
                                    var prTypeAtrs = mainPrType?.ProductAtributes;
                                    if (prTypeAtrs != null && prTypeAtrs.Count() > 0)
                                    {
                                        List<OrderAtributeValue> OrderAtrVals = new List<OrderAtributeValue>();
                                        foreach (var prTypeAtr in prTypeAtrs)
                                        {
                                            var atrValIds = prTypeAtr.AtributeValues.Select(_ => _.AtributeValue.Id);
                                            if (atrValIds.Count() == reqEl.atrs.Count() && atrValIds.All(_ => reqEl.atrs.Any(r => r == _)))
                                            {
                                                if (prTypeAtr.ProductQuantity < reqEl.count)
                                                {
                                                    ViewBag.ModelError = "Please change your bag, some products is ended.";
                                                    return View();
                                                }
                                                prTypeAtr.ProductQuantity -= reqEl.count;
                                                string imgId = mainPrType?.Images?.FirstOrDefault()?.Id;
                                                var cultureTitle = mainPrType?.CultureTitle;
                                                var cultureDescription = mainPrType?.CultureDescription;
                                                var cultureBrand = mainPrType?.CultureBrand;
                                                decimal price = mainPrType.Price;
                                                decimal sale = mainPrType.Sale;
                                                var OrderPrTy = mainPrType;
                                                if (pr.LinkAtribute != null)
                                                {
                                                    var pt = pr.ProductTypes.FirstOrDefault(_ => reqEl.atrs.Any(id => id == _.LinkAtributeValue?.Id));
                                                    OrderPrTy = pt;
                                                    if (pt != null)
                                                    {
                                                        if (pt.Images != null && pt.Images.Count() > 0) imgId = pt.Images.FirstOrDefault()?.Id;
                                                        if (!string.IsNullOrEmpty(CultureData.GetDefoultName(pt.CultureTitle, Culture))) cultureTitle = pt.CultureTitle;
                                                        if (!string.IsNullOrEmpty(CultureData.GetDefoultName(pt.CultureDescription, Culture))) cultureTitle = pt.CultureDescription;
                                                        if (!string.IsNullOrEmpty(CultureData.GetDefoultName(pt.CultureBrand, Culture))) cultureTitle = pt.CultureBrand;

                                                        if (pt.Price != 0 || pt.Sale != 0)
                                                        {
                                                            price = pt.Price;
                                                            sale = pt.Sale;
                                                        }

                                                    }
                                                }

                                                prTypeAtr.AtributeValues.ToList().ForEach(_ => OrderAtrVals.Add(new OrderAtributeValue()
                                                {
                                                    Atribute = CultureData.GetDefoultName(_.AtributeValue.FK_Atribute.CultureName, Culture, _.AtributeValue.FK_Atribute.Name),
                                                    Value = CultureData.GetDefoultName(_.AtributeValue.CultureName, Culture, _.AtributeValue.Value),
                                                    AtributeValue = _.AtributeValue,
                                                }));

                                                ProductSum += reqEl.count * (sale != 0 ? sale : price);

                                                #region OrderInf
                                                OrderProductInfo.Add(new Models.OrderProductInfo()
                                                {
                                                    AtributeAndValue = OrderAtrVals,
                                                    ProductSum = (sale != 0 ? sale : price),
                                                    Quantity = reqEl.count,
                                                    ProductTitle = CultureData.GetDefoultName(cultureTitle, Culture),
                                                    ProductDescription = CultureData.GetDefoultName(cultureDescription, Culture),
                                                    Brand = CultureData.GetDefoultName(cultureBrand, Culture),
                                                    ProductImgId = imgId,
                                                    ProductType = OrderPrTy,
                                                    ProductAtributes = prTypeAtr,
                                                    Product = pr
                                                });

                                                #endregion
                                            }

                                        }
                                    }
                                }
                            }
                        }
                        //change product count
                    }
                        decimal PromoSale = 0;
                        if (!string.IsNullOrEmpty(_m.PromoCode))
                        {
                            var promo =await DBContext.PromoCodes.FirstOrDefaultAsync(_ => _.Name.ToUpper() == _m.PromoCode.ToUpper());
                            if (promo != null && promo.isActive && promo.Expired > DateTime.Now)
                            {
                                PromoSale = promo.SalePercent;
                            }
                        }
                        decimal ShipingSum=await shippingSum(ProductSum * (1 - PromoSale / 100));
                        ViewBag.PromoSale = PromoSale;
                        ViewBag.OrderValue = ProductSum;
                        ViewBag.Shipping = ShipingSum;
                         #region Create New Order
                        var NewOrder = new Order()
                        {
                            Address = _m.Address+(string.IsNullOrEmpty(_m.Building)?"":", "+_m.Building) + (string.IsNullOrEmpty(_m.Apt) ? "" : ", " + _m.Apt) + (string.IsNullOrEmpty(_m.Enter) ? "" : ", " + _m.Enter) + (string.IsNullOrEmpty(_m.Floor) ? "" : ", " + _m.Floor) + (string.IsNullOrEmpty(_m.Code) ? "" : ", " + _m.Code),
                            Email = _m.Email,
                            PaymentMethod = "Cash",
                            Phone = _m.Phone,
                            Status = Order.StatusType.Pending.ToString(),
                            PromoCode = _m.PromoCode,
                            User = User.Identity.IsAuthenticated ? await userManager.GetUserAsync(User) : null,
                            ProductsSum = ProductSum,
                            ShipingSum = ShipingSum,
                            PromoSale = PromoSale,
                            ProductsInfo = OrderProductInfo,
                            Created=DateTime.Now,
                    
                        };
                        #endregion
                        DBContext.Orders.Add(NewOrder);
                        int status = await DBContext.SaveChangesAsync();
                        if (status > 0)
                        {
                            Random random = new Random();
                            var orderNum = RandomOrderId(random);
                            lock (orderNumLock)
                            {
                                for (;  DBContext.Orders.Any(_ => _.OrderNumber == orderNum); orderNum = RandomOrderId(random)) ;
                                NewOrder.OrderNumber = orderNum;
                                DBContext.SaveChanges();
                            }
                            Response.Cookies.Append("ProductList","");
                            ViewBag.Order = NewOrder;
                            return RedirectToAction("CheckoutOrder",new { id = NewOrder?.Id });// ("CheckoutOrder","Home", );
                        }
                }
                return View();
            }
            catch(Exception)
            {
                ViewBag.ModelError = "Error.";
                return View();
            }
            
        }
        private long RandomOrderId(Random random)
        {
            byte[] arr = new byte[8];
            random.NextBytes(arr);
            var i64 = BitConverter.ToInt64(arr, 0);
            var res = i64 % 10000000000;
            res = Math.Abs(res);
            if (res < 1000000000) res += 1000000000;
            return res;
        }

        [HttpGet]
        public async Task<IActionResult> CheckoutOrder(string id)
        {
            ViewBag.amd = getAMD();
            if (string.IsNullOrEmpty(id)) return View();
            var order = await DBContext.Orders.FirstOrDefaultAsync(_ => _.Id == id && _.Status == Order.StatusType.Pending.ToString());
            if (order == null) return View();
            ViewBag.Order = order;
            ViewBag.culture= Request.HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.Culture.Name;
            return await Task.Run(() => View());
        }
        public async Task<IActionResult> About()
        {
            return await Task.Run(() => View());
        }
        public async Task<IActionResult> Contact()
        {
            return await Task.Run(() => View());
        }
        public async Task<IActionResult> Policy()
        {
            return await Task.Run(() => View());
        }
        public async Task<IActionResult> GetProductImg(string Id)
        {
            if (string.IsNullOrEmpty(Id)) return NotFound();
            var TempImg = await DBContext.ProductIMGs.FirstOrDefaultAsync(_ => _.Id == Id);
            if (TempImg != null)
            {
                Response.Headers["Cache-Control"] = $"public,max-age={CacheAgeSeconds}";
                return File(TempImg.IMG, "image/jpeg");
            }
            return NotFound();
        }
        public async Task<IActionResult> GetIndexImg(string Id)
        {
            if (string.IsNullOrEmpty(Id)) return NotFound();
            var TempImg = await DBContext.HomePageSectionDatas.FirstOrDefaultAsync(_ => _.Id == Id);
            if (TempImg != null && TempImg.Img != null && TempImg.Img.Count() > 0) 
            {
                Response.Headers["Cache-Control"] = $"public,max-age={CacheAgeSeconds}";
                return File(TempImg.Img, "image/jpeg");
            }
            return NotFound();
        }
        public async Task<JsonResult> getPromoCode(string code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                var Tempcode = await DBContext.PromoCodes.FirstOrDefaultAsync(_ => _.Name.ToUpper() == code.ToUpper());
                if (Tempcode != null)
                {
                    if (Tempcode.Expired < DateTime.Now) new JsonResult(new { error = "Code is expired." });
                    if (!Tempcode.isActive) new JsonResult(new { error = "Code is stopped." });
                    return new JsonResult(new { res = Tempcode.SalePercent });
                }
            }
            return new JsonResult(new { error="Incorrect code."});
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error()
        {
            return await Task.Run(() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }));
        }

        [HttpPost]
        public async Task<IActionResult> SetLanguage(string culture, string returnUrl)
        {
            HttpContext.Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });

            return await Task.Run(() => LocalRedirect(returnUrl));
        }

        [HttpPost]
        public async Task<JsonResult> getSerchKeys(string val,string catalogId)
        {
            if (string.IsNullOrEmpty(val)) return new JsonResult(new { });
            IEnumerable<Catalog> catalogs = await DBContext.Catalogs.Where(_ => string.IsNullOrEmpty(catalogId) || _.Id == catalogId).ToListAsync();
            if (catalogs == null || catalogs.Count() == 0) return new JsonResult(new { });
            List<string> keys = new List<string>();
            val = val.Trim();
            var serchKeys = val.Split(' ', ',');
            if (serchKeys.Length > 1)
            {
                getSerchKeysInCatalog(catalogs, serchKeys.Last().ToUpper(), keys);
                val = val.Remove(val.Length - serchKeys.Last().Length);
                keys = keys.Select(_ => (val + _)).ToList();
            }
            else
            {
                getSerchKeysInCatalog(catalogs, val.ToUpper(), keys);
            }
            return new JsonResult(keys.Select(_=>_.ToLower()));
        }
        private bool hasKeyInProduct(Product product,IEnumerable<string> keys)
        {
            if (product == null || product.ProductTypes == null || product.ProductTypes.Count() == 0 || keys == null || keys.Count() == 0) return false;
            keys = keys.Select(_ => _.ToUpper());
            foreach(var prType in product.ProductTypes)
            {
                bool HasKey = prType.CultureTitle.Union(prType.CultureBrand).Union(prType.CultureDescription).SelectMany(_ => _.Text.Split(' ', ' ')).Union(prType.SerchKeys.Split(' ', ','))
                    .Where(_ => !string.IsNullOrEmpty(_)).Distinct().Any(_ => keys.Any(k => _.ToUpper().StartsWith(k)));
                if (HasKey) return true;
            }
            return false;
        }
        private void getSerchKeysInCatalog(IEnumerable<Catalog> catalogs, string val, List<string> keys)
        {
            if (keys == null) keys = new List<string>();
            if (keys.Count > 10 || catalogs == null || string.IsNullOrEmpty(val) || catalogs.Count() == 0) return;
            foreach(var catalog in catalogs)
            {
                if (catalog.Products == null || catalog.Products.Count() == 0) continue;
                foreach(var pr in catalog.Products)
                {
                    if (pr.ProductTypes == null && pr.ProductTypes.Count() == 0) continue;
                    foreach(var prType in pr.ProductTypes)
                    {
                        var newKeys = prType.CultureTitle.Union(prType.CultureBrand).Union(prType.CultureDescription).SelectMany(_ => _.Text.Split(' ',' ')).Union(prType.SerchKeys.Split(' ', ',')).Where(_=>!string.IsNullOrEmpty(_)).Distinct().Where(_ => _.ToUpper().StartsWith(val)).ToList();
                        newKeys = newKeys.Except(keys).ToList();
                        if (newKeys.Count() > 0)
                        {
                            keys.AddRange(newKeys);
                            if (keys.Count > 10)
                            {
                                keys = keys.Take(10).ToList();
                                return;
                            }
                        }
                    }
                }
                if (catalog.ChaildCatalogs != null && catalog.ChaildCatalogs.Count > 0) getSerchKeysInCatalog(catalog.ChaildCatalogs, val, keys);
            }
        }
        [HttpPost]
        public async Task serchHistory(string key)
        {
            key = key?.Trim();
            if (string.IsNullOrEmpty(key)) return;

            User user = null;
            if (User.Identity.IsAuthenticated) user = await userManager.GetUserAsync(User);
            SerchHistory temp = new SerchHistory() { Created = DateTime.Now, Key = key, User = user };
            DBContext.SerchHistories.Add(temp);
            await DBContext.SaveChangesAsync();
        }
        [HttpPost]
        public async Task<JsonResult> Subscribe(string email)
        {
            if (string.IsNullOrEmpty(email)) return new JsonResult(new { err = "Email is empty." });
            if (!Classes.EmailService.isEmailAddres(email)) return new JsonResult(new { err = "Incorrect email address." });
            try
            {
                string le = email.ToLower();
                var has = await DBContext.SubscribeEmails.FirstOrDefaultAsync(_ => _.Email == le);
                if(has!=null) return new JsonResult(new { res = "Email already added." });
                DBContext.SubscribeEmails.Add(new SubscribeEmail() { Created=DateTime.Now,Email=email.ToLower(),isNew=true});
                int status = await DBContext.SaveChangesAsync();
                if (status > 0) return new JsonResult(new { res = "Email is add." }); ;
                return new JsonResult(new { err = "Email not added." });
            }
            catch (Exception)
            {
                return new JsonResult(new { err = "Exeption" });
            }
        }
        [HttpPost]
        public async Task<JsonResult> requestCall(string number)
        {
            number = number?.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "");
            if(Regex.Match(number, @"^([\++[0-9]{8,15})$").Success)
            {
                DBContext.RequestCells.Add(new RequestCell() { Created = DateTime.Now, isNew = true, Number = number });
                if(await DBContext.SaveChangesAsync()>0)
                {
                    return new JsonResult(new { res = true });
                }
                return new JsonResult(new { err = "Number not added." });
            }
            return new JsonResult(new {err="Incorrect number." });
        }
        [HttpPost]
        public async Task<JsonResult> requestEmail(string email, string title, string mess)
        {
            if (Classes.EmailService.isEmailAddres(email))
            {
                if (title != null && title.Length > 150) title= title.Substring(0, 150);
                if (mess != null && mess.Length > 500) mess = mess?.Substring(0, 500);
                DBContext.RequestEmails.Add(new RequestEmail() { Created = DateTime.Now, Title = title, Email = email, Message = mess, isNew = true });
                if (await DBContext.SaveChangesAsync() > 0)
                {
                    return new JsonResult(new { res = true });
                }
                return new JsonResult(new { err = "Email not added." });
            }
            return new JsonResult(new { err = "Incorrect email." });
        }
        private string getAMD()
        {
            string Culture = Request.HttpContext.Features.Get<IRequestCultureFeature>().RequestCulture.Culture.Name;
            if (Culture == "hy") return "Դրամ";
            else if (Culture == "ru") return "Драм";
            return "AMD";
        }
    }
}
