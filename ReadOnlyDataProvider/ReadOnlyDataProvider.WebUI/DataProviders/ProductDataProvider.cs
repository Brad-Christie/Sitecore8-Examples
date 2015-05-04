using ReadOnlyDataProvider.WebUI.Services;
using ReadOnlyDataProvider.WebUI.Services.Entities;
using Sitecore.Caching;
using Sitecore.Collections;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.DataProviders;
using Sitecore.Data.IDTables;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Data.Templates;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Reflection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ReadOnlyDataProvider.WebUI
{
    // Very simple read-only DataProvider. The import things to note here are:
    // * The provider implements the DataProvider class within Sitecore. Because
    //   of this, a lot of the overrides don't need to be implemented.
    // * The templates are specified as a means of targetting which items
    //   should be loaded using this datasource. For the purpose of this demo,
    //   the critical templates are "Products" and "Product" located in
    //   sitecore/Template/ReadOnlyDataProvider/.
    // * You do not have to do it this way, but this simply keeps things flexible
    //   and fairly simple. You could determine when/where to load items based
    //   on path (or any other number of factors).
    // * Each item derives from the "Product" template, so if you needed metadata
    //   or wanted to assign a layout, doing so on that template's standard values
    //   would be the way to go.
    public class ProductDataProvider : DataProvider
    {
        const String IDTablePrefix = "products_";

        #region Ctor

        private readonly IGenericRepository<Product, Guid> productRepository;
        private readonly String contentDatabase;
        private readonly ID productsTemplateId;
        private readonly ID productTemplateId;

        public ProductDataProvider(
            IGenericRepository<Product, Guid> productRepository,
            String contentDatabase,
            String productsTemplateId,
            String productTemplateId
        )
        {
            Assert.ArgumentNotNull(productRepository, "productRepository");
            Assert.ArgumentNotNullOrEmpty(contentDatabase, "contentDatabase");
            Assert.ArgumentNotNullOrEmpty(productsTemplateId, "productsTemplateId");
            Assert.ArgumentNotNullOrEmpty(productTemplateId, "productTemplateId");
            Assert.IsTrue(ID.TryParse(productsTemplateId, out this.productsTemplateId), "productsTemplateId");
            Assert.IsTrue(ID.TryParse(productTemplateId, out this.productTemplateId), "productTemplateId");

            this.productRepository = productRepository;
            this.contentDatabase = contentDatabase;
        }

        #endregion

        // Here we check if the item implements our container template. If it does,
        // we create an association between what Sitecore will store the item as
        // and what our repository is storing the item as (SitecoreID<->EntityID).
        // This is the purpose of the IDTable.
        public override IDList GetChildIDs(ItemDefinition itemDefinition, CallContext context)
        {
            if (this.canProcessParent(itemDefinition.ID))
            {
                Trace.WriteLine(String.Format("GetChildIDs({0}, {1})", itemDefinition, context), "ProductDataProvider");
                context.Abort();

                var idList = new IDList();
                var products = this.productRepository.GetAll();
                foreach (var product in products)
                {
                    var tableEntry = this.getSitecoreId(product.Id, itemDefinition.ID, true);
                    idList.Add(tableEntry);
                }
                context.DataManager.Database.Caches.DataCache.Clear();
                return idList;
            }
            return base.GetChildIDs(itemDefinition, context);
        }

        // Here we take the EntityID we generated from GetChildIDs and resolve back
        // to our original entity. From there, simply create a basic item (unpopulated)
        // and return it back so Sitecore can place it in the tree.
        // GetItemFields will be reponsible for populating the item.
        public override ItemDefinition GetItemDefinition(ID itemId, CallContext context)
        {
            if (context.CurrentResult == null)
            {
                Trace.WriteLine(String.Format("GetItemDefinition({0}, {1})", itemId, context), "ProductDataProvider");

                Guid externalId = this.getExternalId(itemId);
                if (externalId != Guid.Empty)
                {
                    var product = this.productRepository.Find(externalId);
                    if (product != null)
                    {
                        var itemName = ItemUtil.ProposeValidItemName(product.Name);
                        var itemDefinition = new ItemDefinition(itemId, itemName, this.productTemplateId, ID.Null);
                        return itemDefinition;
                    }
                }
                return null;
            }
            return base.GetItemDefinition(itemId, context);
        }

        // In this method we re-retrieve the item form the repository and populate the
        // Sitecore item fields we care about from this item. By breaking this out from
        // GetItemDefinition, we're able to keeps things flexible in terms of later updates.
        public override FieldList GetItemFields(ItemDefinition itemDefinition, VersionUri versionUri, CallContext context)
        {
            if (this.canProcessItem(itemDefinition.ID))
            {
                Trace.WriteLine(String.Format("GetItemFields({0}, {1}, {2})", itemDefinition, versionUri, context), "ProductDataProvider");

                var fields = new FieldList();

                var template = TemplateManager.GetTemplate(this.productTemplateId, this.ContentDatabase);
                if (template != null)
                {
                    var originalId = this.getExternalId(itemDefinition.ID);
                    if (originalId != Guid.Empty)
                    {
                        if (context.DataManager.DataSource.ItemExists(itemDefinition.ID))
                        {
                            ReflectionUtil.CallMethod(typeof(ItemCache), "RemoveItem", true, true, new Object[] { itemDefinition.ID });
                        }

                        var product = this.productRepository.Find(originalId);
                        if (product != null)
                        {
                            foreach (var dataField in template.GetFields().Where(ItemUtil.IsDataField))
                            {
                                fields.Add(dataField.ID, this.getFieldValue(dataField, product));
                            }
                        }
                    }
                }

                return fields;

            }
            return base.GetItemFields(itemDefinition, versionUri, context);
        }

        // If our repository implemented language versions, this is where you'd manage it.
        // Some Product Inventory Management (PIM) system can version, so we could manage
        // that here. For this demo, we're assuming the products we have are suitable for
        // every version.
        public override VersionUriList GetItemVersions(ItemDefinition itemDefinition, CallContext context)
        {
            if (this.canProcessItem(itemDefinition.ID))
            {
                Trace.WriteLine(String.Format("GetItemVersions({0}, {1})", itemDefinition, context), "ProductDataProvider");

                var versions = new VersionUriList();
                versions.Add(Language.Current, Sitecore.Data.Version.First);

                context.Abort();
                return versions;
            }
            return base.GetItemVersions(itemDefinition, context);
        }

        // This actually populates the drop-down on the top-right of the content editor's
        // window when an item is selected. To avoid duplicates, we simply return NULL.
        public override LanguageCollection GetLanguages(CallContext context)
        {
            return null;
        }

        // This demo is a very simple one-dimentional listing of products. However, we
        // could get more sphisticated and break it down by category in the tree. This
        // would be the place to do that.
        public override ID GetParentID(ItemDefinition itemDefinition, CallContext context)
        {
            if (this.canProcessItem(itemDefinition.ID))
            {
                Trace.WriteLine("GetParentID", "ProductDataProvider");
                context.Abort();

                return this.getParentId(itemDefinition.ID);
            }
            return base.GetParentID(itemDefinition, context);
        }

        #region Helpers

        // Current database we're targetting based on the setting defined in the patch file
        private Database ContentDatabase
        {
            get { return Factory.GetDatabase(this.contentDatabase); }
        }

        // Determines if the item is a Product item.
        private Boolean canProcessItem(ID id)
        {
            var idTable = IDTable.GetKeys(IDTablePrefix, id);
            var result = (idTable != null && idTable.Length > 0);
            Trace.WriteLineIf(result, String.Format("{0} can be processed.", id), "ProductDataProvider");
            return result;
        }

        // Determines if the item is a Products item.
        private Boolean canProcessParent(ID parentId)
        {
            var db = Factory.GetDatabase(this.contentDatabase);
            var item = db.Items[parentId];
            var result = (item != null && item.TemplateID == this.productsTemplateId);
            Trace.WriteLineIf(result, String.Format("{0}'s parent can be processed.", parentId), "ProductDataProvider");
            return result;
        }

        // Returns only the data fields (avoids Standard Template fields and other
        // Sitecore system fields).
        private IEnumerable<TemplateField> getDataFields(Template template)
        {
            return template.GetFields().Where(ItemUtil.IsDataField).AsEnumerable();
        }

        // Creates a mapping between what Sitecore labels a field as and which
        // Property we're targetting on our Entity.
        private String getFieldValue(TemplateField templateField, Product product)
        {
            switch (templateField.Name)
            {
                case "Description":
                    return product.Description;
                case "Name":
                    return product.Name;
                case "Database Id":
                    return product.Id.ToString();
            }
            return String.Empty;
        }

        // Get's the parent id
        private ID getParentId(ID childId)
        {
            var keys = IDTable.GetKeys(IDTablePrefix, childId);
            if (keys != null && keys.Length > 0)
            {
                Trace.WriteLine(String.Format("{0}'s parent is {1}", childId, keys[0].ParentID), "ProductDataProvider");
                return keys[0].ParentID;
            }
            Trace.WriteLine(String.Format("{0} doesn't have a parent", childId), "ProductDataProvider");
            return null;
        }

        // From the Sitecore ID, get the original ID of the entity.
        private Guid getExternalId(ID id)
        {
            var tableEntries = IDTable.GetKeys(IDTablePrefix, id);
            if (tableEntries != null && tableEntries.Length > 0)
            {
                Guid guid;
                if (Guid.TryParse(tableEntries[0].Key, out guid))
                {
                    Trace.WriteLine(String.Format("{0} (in sitecore) == {1} (repository)", id, guid, "ProductRepository"));
                    return guid;
                }
            }
            Trace.WriteLine(String.Format("{0} doesn't have a repository entry", id), "ProductDataProvider");
            return Guid.Empty;
        }

        // From the entity ID, get the Sitecore ID.
        // Also can creates the new ID if one does not exist.
        private ID getSitecoreId(Guid id, ID parentId, Boolean createIfNotExist = true)
        {
            var tableEntry = IDTable.GetID(IDTablePrefix, id.ToString());
            if (tableEntry == null && createIfNotExist)
            {
                tableEntry = IDTable.GetNewID(IDTablePrefix, id.ToString(), parentId);
            }
            if (tableEntry != null)
            {
                Trace.WriteLine(String.Format("{0} (repository) is {1} (sitecore)", id, tableEntry.ID), "ProductDataProvider");
                return tableEntry.ID;
            }
            Trace.WriteLine(String.Format("{0} doesn't have a Sitecore id", id), "ProductDataProvider");
            return null;
        }

        #endregion
    }
}
