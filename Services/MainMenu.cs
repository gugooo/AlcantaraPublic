using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Alcantara.Models;
using Microsoft.EntityFrameworkCore;

namespace Alcantara.Services
{
    public interface IMainMenu
    {
        Task<IEnumerable<IMenuElement>> GetMenu(string Culture);
    }
    public interface IMenuElement
    {
        string Name { get; }
        string Id { get; }
        IEnumerable<IMenuElement> ChildSection { get; }
    }

    class TwoLevelMenu : IMainMenu
    {
        private readonly UserDBContext userDB;
        public TwoLevelMenu(IServiceProvider service) 
        {
            this.userDB = (UserDBContext)service.GetService(typeof(UserDBContext));
        }
        public async Task<IEnumerable<IMenuElement>> GetMenu(string Culture)
        {
            try
            {
                var TempCatalog = await userDB.Catalogs.Where(_ => _.FatherCatalog == null).OrderBy(_=>_._Index).ToListAsync();
                if (TempCatalog == null || TempCatalog.Count == 0) return null;
                var me = TempCatalog.Select(_ => new MElement()
                {
                    Name = CultureData.GetDefoultName(_.CultureName, Culture, _.Name),
                    Id = _.Id,
                    ChildSection = _.ChaildCatalogs?.OrderBy(o=>o._Index).Select(ch => new MElement() { ChildSection = null, Name = CultureData.GetDefoultName(ch.CultureName, Culture, ch.Name), Id = ch.Id })
                });
                return me;
            }
            catch (Exception)
            {
                return null;
            }
        
        }
    }
    class MElement : IMenuElement
    {
        public string Name { get; set; }

        public string Id { get; set; }

        public IEnumerable<IMenuElement> ChildSection { get; set; }
    }
}
