﻿// <auto-generated />
using System;
using Alcantara.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Alcantara.Migrations
{
    [DbContext(typeof(UserDBContext))]
    [Migration("20200616200329_First")]
    partial class First
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Alcantara.Models.AddressBook", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Address");

                    b.Property<string>("City");

                    b.Property<string>("Country");

                    b.Property<string>("FName");

                    b.Property<string>("LName");

                    b.Property<string>("PostCode");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AddressBook");
                });

            modelBuilder.Entity("Alcantara.Models.Atribute", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsActive");

                    b.Property<string>("Name");

                    b.Property<int>("_Index")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.HasKey("Id");

                    b.ToTable("Atributes");
                });

            modelBuilder.Entity("Alcantara.Models.AtributeValue", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FK_AtributeId");

                    b.Property<string>("Value");

                    b.Property<int>("_Index")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.HasKey("Id");

                    b.HasIndex("FK_AtributeId");

                    b.ToTable("AtributeValues");
                });

            modelBuilder.Entity("Alcantara.Models.Catalog", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FatherCatalogId");

                    b.Property<bool>("IsActive");

                    b.Property<string>("Name");

                    b.Property<int>("_Index")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.HasKey("Id");

                    b.HasIndex("FatherCatalogId");

                    b.ToTable("Catalogs");
                });

            modelBuilder.Entity("Alcantara.Models.CultureData", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AtributeId");

                    b.Property<string>("AtributeValueId");

                    b.Property<string>("CatalogId");

                    b.Property<string>("Culture");

                    b.Property<string>("FK_BrandId");

                    b.Property<string>("FK_DescriptionId");

                    b.Property<string>("FK_HP_DescriptionId");

                    b.Property<string>("FK_HP_TitleId");

                    b.Property<string>("FK_TitleId");

                    b.Property<string>("Text");

                    b.HasKey("Id");

                    b.HasIndex("AtributeId");

                    b.HasIndex("AtributeValueId");

                    b.HasIndex("CatalogId");

                    b.HasIndex("FK_BrandId");

                    b.HasIndex("FK_DescriptionId");

                    b.HasIndex("FK_HP_DescriptionId");

                    b.HasIndex("FK_HP_TitleId");

                    b.HasIndex("FK_TitleId");

                    b.ToTable("CultureDatas");
                });

            modelBuilder.Entity("Alcantara.Models.GlobalSetings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("EnableAllReviews");

                    b.Property<string>("LiveChat_AutoResponse");

                    b.Property<string>("LiveChat_OperatorName");

                    b.Property<decimal>("ShippingOrderSum")
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("ShippingWhenLess")
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("ShippingWhenMore")
                        .HasColumnType("decimal(18,4)");

                    b.HasKey("Id");

                    b.ToTable("GlobalSetings");
                });

            modelBuilder.Entity("Alcantara.Models.HomePageSection", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsFixedData");

                    b.Property<int>("Position");

                    b.Property<string>("SectionName");

                    b.Property<int>("_Index")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.HasKey("Id");

                    b.ToTable("HomePageSections");
                });

            modelBuilder.Entity("Alcantara.Models.HomePageSectionData", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CatalogId");

                    b.Property<string>("FK_HomePageSectionId");

                    b.Property<byte[]>("Img");

                    b.Property<bool>("TextIsWhith");

                    b.Property<int>("_Index")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.HasKey("Id");

                    b.HasIndex("CatalogId");

                    b.HasIndex("FK_HomePageSectionId");

                    b.ToTable("HomePageSectionDatas");
                });

            modelBuilder.Entity("Alcantara.Models.LiveChatMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsNew");

                    b.Property<bool>("IsOperator");

                    b.Property<int?>("LiveChatSessionId");

                    b.Property<string>("Message");

                    b.Property<DateTime>("Sended");

                    b.HasKey("Id");

                    b.HasIndex("LiveChatSessionId");

                    b.ToTable("LiveChatMessage");
                });

            modelBuilder.Entity("Alcantara.Models.LiveChatSession", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Added");

                    b.Property<string>("OperatorName");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.ToTable("LiveChatSessions");
                });

            modelBuilder.Entity("Alcantara.Models.MailingHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created");

                    b.Property<string>("Message");

                    b.Property<int>("MessageCount");

                    b.Property<string>("Title");

                    b.Property<string>("to");

                    b.HasKey("Id");

                    b.ToTable("MailingHistories");
                });

            modelBuilder.Entity("Alcantara.Models.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("FK_AdminSendId");

                    b.Property<string>("FK_UserSendId");

                    b.Property<bool>("IsNew");

                    b.Property<DateTime>("SendedDate");

                    b.Property<string>("Text");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("FK_AdminSendId");

                    b.HasIndex("FK_UserSendId");

                    b.ToTable("Message");
                });

            modelBuilder.Entity("Alcantara.Models.Order", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address");

                    b.Property<DateTime>("Created");

                    b.Property<string>("Email");

                    b.Property<long>("OrderNumber");

                    b.Property<string>("PaymentMethod");

                    b.Property<string>("Phone");

                    b.Property<decimal>("ProductsSum")
                        .HasColumnType("decimal(18,4)");

                    b.Property<string>("PromoCode");

                    b.Property<decimal>("PromoSale")
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal>("ShipingSum")
                        .HasColumnType("decimal(18,4)");

                    b.Property<string>("Status");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("Alcantara.Models.OrderAtributeValue", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Atribute");

                    b.Property<string>("AtributeValueId");

                    b.Property<int?>("ProductInfoId");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.HasIndex("AtributeValueId");

                    b.HasIndex("ProductInfoId");

                    b.ToTable("OrderAtributeValue");
                });

            modelBuilder.Entity("Alcantara.Models.OrderProductInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Brand");

                    b.Property<string>("OrderId");

                    b.Property<string>("ProductAtributesId");

                    b.Property<string>("ProductDescription");

                    b.Property<string>("ProductId");

                    b.Property<string>("ProductImgId");

                    b.Property<decimal>("ProductSum")
                        .HasColumnType("decimal(18,4)");

                    b.Property<string>("ProductTitle");

                    b.Property<string>("ProductTypeId");

                    b.Property<int>("Quantity");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.HasIndex("ProductAtributesId");

                    b.HasIndex("ProductId");

                    b.HasIndex("ProductTypeId");

                    b.ToTable("OrderProductInfo");
                });

            modelBuilder.Entity("Alcantara.Models.Product", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CatalogId");

                    b.Property<bool>("IsActive");

                    b.Property<string>("LinkAtributeId");

                    b.Property<int>("_Index")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.HasKey("Id");

                    b.HasIndex("CatalogId");

                    b.HasIndex("LinkAtributeId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("Alcantara.Models.ProductAtributeValue", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AtributeValueId");

                    b.Property<string>("ProductAtributesId");

                    b.Property<int>("_Index")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.HasKey("Id");

                    b.HasIndex("AtributeValueId");

                    b.HasIndex("ProductAtributesId");

                    b.ToTable("ProductAtributeValues");
                });

            modelBuilder.Entity("Alcantara.Models.ProductAtributes", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ProductQuantity");

                    b.Property<string>("ProductTypeId");

                    b.HasKey("Id");

                    b.HasIndex("ProductTypeId");

                    b.ToTable("ProductAtributes");
                });

            modelBuilder.Entity("Alcantara.Models.ProductIMG", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<byte[]>("IMG");

                    b.Property<string>("ProductTypeId");

                    b.Property<int>("_Index")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.HasKey("Id");

                    b.HasIndex("ProductTypeId");

                    b.ToTable("ProductIMGs");
                });

            modelBuilder.Entity("Alcantara.Models.ProductType", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FirstAtributeId");

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsMine");

                    b.Property<string>("LinkAtributeValueId");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,4)");

                    b.Property<string>("ProductId");

                    b.Property<decimal>("Sale")
                        .HasColumnType("decimal(18,4)");

                    b.Property<string>("SerchKeys");

                    b.Property<int>("_Index")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.HasKey("Id");

                    b.HasIndex("FirstAtributeId");

                    b.HasIndex("LinkAtributeValueId");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductTypes");
                });

            modelBuilder.Entity("Alcantara.Models.PromoCode", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created");

                    b.Property<DateTime>("Expired");

                    b.Property<string>("Name");

                    b.Property<decimal>("SalePercent")
                        .HasColumnType("decimal(18,4)");

                    b.Property<bool>("isActive");

                    b.HasKey("id");

                    b.ToTable("PromoCodes");
                });

            modelBuilder.Entity("Alcantara.Models.RequestCell", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created");

                    b.Property<string>("Number");

                    b.Property<bool>("isNew");

                    b.HasKey("Id");

                    b.ToTable("RequestCells");
                });

            modelBuilder.Entity("Alcantara.Models.RequestEmail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created");

                    b.Property<string>("Email");

                    b.Property<string>("Message");

                    b.Property<string>("Title");

                    b.Property<bool>("isNew");

                    b.HasKey("Id");

                    b.ToTable("RequestEmails");
                });

            modelBuilder.Entity("Alcantara.Models.Review", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Date");

                    b.Property<string>("Description");

                    b.Property<string>("FK_ProductId");

                    b.Property<string>("FK_UserId");

                    b.Property<int>("Rating");

                    b.Property<bool?>("Status");

                    b.HasKey("Id");

                    b.HasIndex("FK_ProductId");

                    b.HasIndex("FK_UserId");

                    b.ToTable("Reviews");
                });

            modelBuilder.Entity("Alcantara.Models.SerchHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created");

                    b.Property<string>("Key");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("SerchHistories");
                });

            modelBuilder.Entity("Alcantara.Models.SharedMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("SendedDate");

                    b.Property<string>("Text");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.ToTable("SharedMessage");
                });

            modelBuilder.Entity("Alcantara.Models.SharedMessageUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsNew");

                    b.Property<int?>("MessageId");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("MessageId");

                    b.HasIndex("UserId");

                    b.ToTable("SharedMessageUser");
                });

            modelBuilder.Entity("Alcantara.Models.SubscribeEmail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created");

                    b.Property<string>("Email");

                    b.Property<bool>("isNew");

                    b.HasKey("Id");

                    b.ToTable("SubscribeEmails");
                });

            modelBuilder.Entity("Alcantara.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("Address");

                    b.Property<string>("City");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Country");

                    b.Property<DateTime>("DateOfBirth");

                    b.Property<string>("Description");

                    b.Property<string>("Document");

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("PostCode");

                    b.Property<DateTime>("Registred");

                    b.Property<string>("SecurityStamp");

                    b.Property<int?>("SelectedAddressId");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.Property<int>("_Index")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.HasIndex("SelectedAddressId");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("Alcantara.Models.AddressBook", b =>
                {
                    b.HasOne("Alcantara.Models.User")
                        .WithMany("AddressBook")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Alcantara.Models.AtributeValue", b =>
                {
                    b.HasOne("Alcantara.Models.Atribute", "FK_Atribute")
                        .WithMany("Values")
                        .HasForeignKey("FK_AtributeId")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("Alcantara.Models.Catalog", b =>
                {
                    b.HasOne("Alcantara.Models.Catalog", "FatherCatalog")
                        .WithMany("ChaildCatalogs")
                        .HasForeignKey("FatherCatalogId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Alcantara.Models.CultureData", b =>
                {
                    b.HasOne("Alcantara.Models.Atribute")
                        .WithMany("CultureName")
                        .HasForeignKey("AtributeId");

                    b.HasOne("Alcantara.Models.AtributeValue")
                        .WithMany("CultureName")
                        .HasForeignKey("AtributeValueId");

                    b.HasOne("Alcantara.Models.Catalog")
                        .WithMany("CultureName")
                        .HasForeignKey("CatalogId");

                    b.HasOne("Alcantara.Models.ProductType", "FK_Brand")
                        .WithMany("CultureBrand")
                        .HasForeignKey("FK_BrandId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Alcantara.Models.ProductType", "FK_Description")
                        .WithMany("CultureDescription")
                        .HasForeignKey("FK_DescriptionId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Alcantara.Models.HomePageSectionData", "FK_HP_Description")
                        .WithMany("Description")
                        .HasForeignKey("FK_HP_DescriptionId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Alcantara.Models.HomePageSectionData", "FK_HP_Title")
                        .WithMany("Title")
                        .HasForeignKey("FK_HP_TitleId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Alcantara.Models.ProductType", "FK_Title")
                        .WithMany("CultureTitle")
                        .HasForeignKey("FK_TitleId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Alcantara.Models.HomePageSectionData", b =>
                {
                    b.HasOne("Alcantara.Models.Catalog", "Catalog")
                        .WithMany()
                        .HasForeignKey("CatalogId");

                    b.HasOne("Alcantara.Models.HomePageSection", "FK_HomePageSection")
                        .WithMany("HomePageSectionDatas")
                        .HasForeignKey("FK_HomePageSectionId")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("Alcantara.Models.LiveChatMessage", b =>
                {
                    b.HasOne("Alcantara.Models.LiveChatSession")
                        .WithMany("messages")
                        .HasForeignKey("LiveChatSessionId");
                });

            modelBuilder.Entity("Alcantara.Models.Message", b =>
                {
                    b.HasOne("Alcantara.Models.User", "FK_AdminSend")
                        .WithMany("AdminSend")
                        .HasForeignKey("FK_AdminSendId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Alcantara.Models.User", "FK_UserSend")
                        .WithMany("UserSend")
                        .HasForeignKey("FK_UserSendId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Alcantara.Models.Order", b =>
                {
                    b.HasOne("Alcantara.Models.User", "User")
                        .WithMany("Orders")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("Alcantara.Models.OrderAtributeValue", b =>
                {
                    b.HasOne("Alcantara.Models.AtributeValue", "AtributeValue")
                        .WithMany()
                        .HasForeignKey("AtributeValueId");

                    b.HasOne("Alcantara.Models.OrderProductInfo", "ProductInfo")
                        .WithMany("AtributeAndValue")
                        .HasForeignKey("ProductInfoId")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("Alcantara.Models.OrderProductInfo", b =>
                {
                    b.HasOne("Alcantara.Models.Order", "Order")
                        .WithMany("ProductsInfo")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("Alcantara.Models.ProductAtributes", "ProductAtributes")
                        .WithMany()
                        .HasForeignKey("ProductAtributesId");

                    b.HasOne("Alcantara.Models.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId");

                    b.HasOne("Alcantara.Models.ProductType", "ProductType")
                        .WithMany()
                        .HasForeignKey("ProductTypeId");
                });

            modelBuilder.Entity("Alcantara.Models.Product", b =>
                {
                    b.HasOne("Alcantara.Models.Catalog")
                        .WithMany("Products")
                        .HasForeignKey("CatalogId");

                    b.HasOne("Alcantara.Models.Atribute", "LinkAtribute")
                        .WithMany()
                        .HasForeignKey("LinkAtributeId");
                });

            modelBuilder.Entity("Alcantara.Models.ProductAtributeValue", b =>
                {
                    b.HasOne("Alcantara.Models.AtributeValue", "AtributeValue")
                        .WithMany()
                        .HasForeignKey("AtributeValueId");

                    b.HasOne("Alcantara.Models.ProductAtributes")
                        .WithMany("AtributeValues")
                        .HasForeignKey("ProductAtributesId");
                });

            modelBuilder.Entity("Alcantara.Models.ProductAtributes", b =>
                {
                    b.HasOne("Alcantara.Models.ProductType")
                        .WithMany("ProductAtributes")
                        .HasForeignKey("ProductTypeId");
                });

            modelBuilder.Entity("Alcantara.Models.ProductIMG", b =>
                {
                    b.HasOne("Alcantara.Models.ProductType")
                        .WithMany("Images")
                        .HasForeignKey("ProductTypeId");
                });

            modelBuilder.Entity("Alcantara.Models.ProductType", b =>
                {
                    b.HasOne("Alcantara.Models.Atribute", "FirstAtribute")
                        .WithMany()
                        .HasForeignKey("FirstAtributeId");

                    b.HasOne("Alcantara.Models.AtributeValue", "LinkAtributeValue")
                        .WithMany()
                        .HasForeignKey("LinkAtributeValueId");

                    b.HasOne("Alcantara.Models.Product")
                        .WithMany("ProductTypes")
                        .HasForeignKey("ProductId");
                });

            modelBuilder.Entity("Alcantara.Models.Review", b =>
                {
                    b.HasOne("Alcantara.Models.Product", "FK_Product")
                        .WithMany("Reviews")
                        .HasForeignKey("FK_ProductId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("Alcantara.Models.User", "FK_User")
                        .WithMany("Reviews")
                        .HasForeignKey("FK_UserId")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("Alcantara.Models.SerchHistory", b =>
                {
                    b.HasOne("Alcantara.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Alcantara.Models.SharedMessageUser", b =>
                {
                    b.HasOne("Alcantara.Models.SharedMessage", "Message")
                        .WithMany()
                        .HasForeignKey("MessageId");

                    b.HasOne("Alcantara.Models.User")
                        .WithMany("AdminSharedMessage")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Alcantara.Models.User", b =>
                {
                    b.HasOne("Alcantara.Models.AddressBook", "SelectedAddress")
                        .WithMany()
                        .HasForeignKey("SelectedAddressId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Alcantara.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Alcantara.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Alcantara.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Alcantara.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
