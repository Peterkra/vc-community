﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using foundation = VirtoCommerce.Foundation.Catalogs.Model;
using module = VirtoCommerce.CatalogModule.Model;
using VirtoCommerce.CatalogModule.Data.Converters;
using VirtoCommerce.Foundation.Frameworks;
using VirtoCommerce.CatalogModule.Repositories;
using VirtoCommerce.CatalogModule.Data.Repositories;
using VirtoCommerce.CatalogModule.Data.Services;
using VirtoCommerce.CatalogModule.Services;
using VirtoCommerce.CatalogModule.Model;
using System.Collections.Generic;

namespace VirtoCommerce.CatalogModule.Test
{
	[TestClass]
	public class CatalogTest
	{
		[TestMethod]
		public void CatalogPatchTest()
		{
			var chageTracker = new ObservableChangeTracker
			{
				RemoveAction = (x) =>
				{
					Console.WriteLine(x.ToString());
				}
			};

			var dbCatalog1 = new foundation.Catalog
			{
				 Name = "catalog1",
				 DefaultLanguage = "en-us"
			};
			dbCatalog1.CatalogLanguages.Add(new foundation.CatalogLanguage { Language = "en-us" });
			dbCatalog1.CatalogLanguages.Add(new foundation.CatalogLanguage { Language = "fr-fr" });

			var dbCatalog2 = new foundation.Catalog
			{
				Name = "unknow"
			};
			dbCatalog2.CatalogLanguages.Add(new foundation.CatalogLanguage { Language = "ru-ru" });

			chageTracker.Attach(dbCatalog2);
			dbCatalog1.Patch(dbCatalog2);

		}

		[TestMethod]
		public void WorkingWithCatalogTest()
		{
			var catalogService = GetCatalogService();
			var catalog = new Catalog
			{
				Name = "Test",
			};
			var languages = new CatalogLanguage[]
			{
				new CatalogLanguage { LanguageCode = "en-us"}
			};
			//Create catalog
			catalog.Languages = languages.ToList();
			catalog = catalogService.Create(catalog);
			//Add language
			catalog.Languages.Add( new  CatalogLanguage { LanguageCode = "fr-fr", IsDefault = true });
			catalog.Name = null; //not define should no changed
			catalogService.Update(new Catalog[] { catalog });
			//Clear languages
			catalog.Languages.Clear();
			catalogService.Update(new Catalog[] { catalog });
			//Verification
			catalog = catalogService.GetById(catalog.Id);

			Assert.IsTrue(catalog.Name == "Test");
			Assert.IsFalse(catalog.Languages.Any());
		}

		[TestMethod]
		public void GetProduct()
		{
			var itemService = GetItemService();
			var product = itemService.GetById("b7871dc6-9b2c-4a9a-a1aa-2c48fb0a6243");
		}
		[TestMethod]
		public void WorkingWithItemTest()
		{
			var itemService = GetItemService();
			var catalogService = GetCatalogService();
			var categoryService = GetCategoryService();
			var propertyService = GetPropertyService();

			var category = categoryService.GetById("a8bfc7cd-4363-4f12-976e-3f236433b6cf");

			//Add product property to category
			//var productProperty = new Property
			//{
			//	CategoryId = category.Id,
			//	CatalogId = category.Catalog.Id,
			//	Name = "testProp",
			//	Type = PropertyType.Product,
			//	ValueType = PropertyValueType.ShortText
			//};
			//productProperty = propertyService.Create(productProperty);


			var productProperty = propertyService.GetCategoryProperties(category.Id)
													.Where(x=>x.Type == PropertyType.Product).FirstOrDefault();
			var productPropValue = new PropertyValue
			{
				//Property = productProperty,
				PropertyId = productProperty.Id,
				Value = "some value",
				PropertyName = productProperty.Name
			};
			var product = new CatalogProduct
			{
				Name = "TestProduct",
				Code = "Code",
				CatalogId = category.CatalogId,
				CategoryId = category.Id
			};
			product.PropertyValues = new List<PropertyValue>();
			product.PropertyValues.Add(productPropValue);
			product = itemService.Create(product);
		}

		[TestMethod]
		public void CreateProperty()
		{
			var categoryService = GetCategoryService();
			var propertyService = GetPropertyService();
			var category = categoryService.GetById("a8bfc7cd-4363-4f12-976e-3f236433b6cf");
			var property = new Property
			{
				Id = "testProperty",
				CatalogId = category.CatalogId,
				CategoryId = category.Id,
				Name = "testProperty2",
				Type = PropertyType.Product,
				ValueType = PropertyValueType.ShortText,
				Dictionary = true
			};
			property.Attributes = new List<PropertyAttribute>();
			property.DictionaryValues = new List<PropertyDictionaryValue>();

			var attribute = new PropertyAttribute
			{
				 Name = "attr1",
				 Value = "val1"

			};
			property.Attributes.Add(attribute);

			property.DictionaryValues.Add(new PropertyDictionaryValue { Value = "ss", Property = property });
			property.DictionaryValues.Add(new PropertyDictionaryValue { Value = "ddd", Property = property });

			//property = propertyService.Create(property);

			property = propertyService.GetById(property.Id);

			property.DictionaryValues.Remove(property.DictionaryValues.First());
			property.DictionaryValues.Add(new PropertyDictionaryValue { Value = "fff", Property = property });

			propertyService.Update(new Property[] { property });

			property = propertyService.GetById(property.Id);

			propertyService.Delete(new string[] { property.Id });
		}

		[TestMethod]
		public void SearchTest()
		{
			var criteria = new SearchCriteria()
			{
				ResponseGroup = ResponseGroup.WithCatalogs | ResponseGroup.WithCategories
			};
			var searchService = GetSearchService();

			var result = searchService.Search(criteria);

			criteria.CatalogId = "Samsung";
			
			result = searchService.Search(criteria);

			criteria.ResponseGroup = ResponseGroup.WithItems;
			criteria.CategoryId = result.Categories[0].Id;

			result = searchService.Search(criteria);
		}

		private ICatalogSearchService GetSearchService()
		{
			return new CatalogSearchServiceImpl(GetRepository, GetItemService(), GetCatalogService(), GetCategoryService());
		}

		private IPropertyService GetPropertyService()
		{
			return new PropertyServiceImpl(() => { return GetRepository(); }, null);
		}

		private ICategoryService GetCategoryService()
		{
			return new CategoryServiceImpl(() => { return GetRepository(); }, null);
		}

		private ICatalogService GetCatalogService()
		{
			return new CatalogServiceImpl(() => { return GetRepository(); }, null);
		}

		private IItemService GetItemService()
		{
			return new ItemServiceImpl(() => { return GetRepository(); }, null);
		}


		private IFoundationCatalogRepository GetRepository()
		{
			var retVal = new FoundationCatalogRepositoryImpl("VirtoCommerce");
			return retVal;
		}
	}
}