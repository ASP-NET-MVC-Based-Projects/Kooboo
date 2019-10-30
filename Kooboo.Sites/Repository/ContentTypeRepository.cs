//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Definition;
using Kooboo.IndexedDB;
using Kooboo.Sites.Contents.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.Repository
{
    public class ContentTypeRepository : SiteRepositoryBase<ContentType>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                var paras = new ObjectStoreParameters();
                paras.AddColumn<ContentType>(it => it.Id);
                paras.AddColumn<ContentType>(it => it.Name);
                paras.AddColumn<ContentType>(it => it.CreationDate);
                paras.AddColumn<ContentType>(it => it.LastModified);
                paras.SetPrimaryKeyField<ContentType>(o => o.Id);
                return paras;
            }
        }

        public List<ContentProperty> GetColumns(Guid contentTypeId)
        {
            var value = this.Get(contentTypeId);
            return value == null ? new List<ContentProperty>() : value.Properties;
        }

        public List<ContentProperty> GetPropertiesByFolder(Guid folderId)
        {
            var folder = this.SiteDb.ContentFolders.Get(folderId);
            return folder != null ? GetColumns(folder.ContentTypeId) : null;
        }

        public override bool AddOrUpdate(ContentType value)
        {
            EnsureSystemFields(value);
            return base.AddOrUpdate(value);
        }

        public override bool AddOrUpdate(ContentType value, Guid userId)
        {
            EnsureSystemFields(value);
            return base.AddOrUpdate(value, userId);
        }

        public void EnsureSystemFields(ContentType contenttype)
        {
            foreach (var item in contenttype.Properties)
            {
                if (item.Name.ToLower() == SystemFields.UserKey.Name.ToLower())
                {
                    item.MultipleLanguage = false;
                    item.DataType = Data.Definition.DataTypes.String;
                    item.ControlType = ControlTypes.TextBox;
                    item.IsSystemField = true;
                }
                else if (item.Name.ToLower() == SystemFields.Sequence.Name.ToLower())
                {
                    item.MultipleLanguage = SystemFields.Sequence.MultipleLanguage;
                    item.DataType = SystemFields.Sequence.DataType;
                    item.ControlType = SystemFields.Sequence.ControlType;
                    item.IsSystemField = true;
                }
                else if (item.Name.ToLower() == SystemFields.Online.Name.ToLower())
                {
                    item.MultipleLanguage = SystemFields.Online.MultipleLanguage;
                    item.DataType = SystemFields.Online.DataType;
                    item.ControlType = SystemFields.Online.ControlType;
                    item.IsSystemField = true;
                }
            }

            // remove duplicate system fields...
            List<ContentProperty> removeProp = new List<ContentProperty>();

            bool hasuserkey = false;
            bool hasseq = false;
            bool hasonline = false;

            foreach (var item in contenttype.Properties)
            {
                if (item.Name.ToLower() == SystemFields.UserKey.Name.ToLower())
                {
                    if (hasuserkey)
                    {
                        removeProp.Add(item);
                    }
                    else { hasuserkey = true; }
                }
                else if (item.Name.ToLower() == SystemFields.Sequence.Name.ToLower())
                {
                    if (hasseq)
                    {
                        removeProp.Add(item);
                    }
                    else { hasseq = true; }
                }
                else if (item.Name.ToLower() == SystemFields.Online.Name.ToLower())
                {
                    if (hasonline)
                    {
                        removeProp.Add(item);
                    }
                    else { hasonline = true; }
                }
            }

            foreach (var item in removeProp)
            {
                contenttype.Properties.Remove(item);
            }

            if (!hasonline)
            {
                contenttype.Properties.Add(SystemFields.Online);
            }
        }

        public bool IsNameExists(string contentTypeName)
        {
            var type = this.GetByNameOrId(contentTypeName);
            return (type != null);
        }

        public List<string> GetTitleColumns(Guid contentTypeId)
        {
            List<string> names = new List<string>();
            var contentType = this.Get(contentTypeId);

            foreach (var item in contentType.Properties.Where(o => o.IsSummaryField && !o.IsSystemField))
            {
                names.Add(item.Name);
            }

            if (names.Any())
            {
                return names;
            }

            names.Add(contentType.Properties.OrderBy(o => o.Order).First().Name);
            return names;
        }

        public ContentType GetByFolder(Guid folderId)
        {
            var folder = this.SiteDb.ContentFolders.Get(folderId);
            return folder != null ? this.Get(folder.ContentTypeId) : null;
        }
    }
}