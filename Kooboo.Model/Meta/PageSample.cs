﻿using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Model.Meta;
using Kooboo.Model.Meta.Form;
using Kooboo.Model.Meta.Table;
using Kooboo.Model.Meta.Validation;
using Kooboo.Model.Meta.Popup;

namespace Kooboo.Model.Meta
{

    public class PageSample : ITableMetaConfigure<PageTest>
    {
        public bool IsCreator =>true;

        public void Configure(TableMeta meta)
        {
            meta.DataApi ="/page/all";
            meta.ListName = "page";

            #region menu
            meta.Menu.Add(new ButtonMenu
            {
                Text =new Localizable("New page"),
                Class = "green",
                Action = new ActionMeta
                {
                    Type = ActionType.Redirect,
                    Url = "/_Admin/Page/EditPage"
                }
            });

            meta.Menu.Add(new ButtonMenu
            {
                Text = new Localizable("New rich text page"),
                Class = "green",
                Action = new ActionMeta()
                {
                    Type = ActionType.Redirect,
                    Url = "/_Admin/Page/EditRichText"
                }
            });

            meta.Menu.Add(new DropDownMenu
            {
                Text = new Localizable("New layout page"),
                Class = "green",
                Action = new ActionMeta()
                {
                    Type = ActionType.Redirect,
                    Url = "/_Admin/Page/EditLayout?layoutId={id}"
                },
                Options = SelectOptions.UseContext("layouts", "{name}")
            });

            meta.Menu.Add(new ButtonMenu
            {
                Text = new Localizable("Import"),
                Class = "green",
                Action = new ActionMeta()
                {
                    Type = ActionType.Popup,

                    Url = "popup?modelName=importPopup"
                }
            });

            meta.Menu.Add(new ButtonMenu()
            {
                Text = new Localizable("Copy"),
                Class = "green",
                Action=ActionMeta.EmbeddedPopup(CreateCopyPopupMeta()),
                Visible = Comparison.Equal(1)
            });

            meta.Menu.Add(new ButtonMenu
            {
                Text = new Localizable("Delete"),
                Class = "red",
                Action = new ActionMeta
                {
                    Type = ActionType.Post,
                    Confirm = "confirm.deleteItems",
                    Url = "page/deletes"
                },
                Visible = Comparison.EqualOrGreaterThan(1)
            });

            meta.Menu.Add(new IconMenu
            {
                //Text = "Copy",
                Align = MenuAlign.Right,
                IconClass = "gear",
                Action=ActionMeta.EmbeddedPopup(CreateRoutePopupMeta())
            });
            #endregion

            #region table
            meta.Builder<PageViewModel>()
                //.MergeModel()
                .Column<TextCell>(p=>p.name, new Localizable("Name"), null)
                .Column<BadgeCell>(p=>p.linked, new Localizable("Linked"), null)
                .Column<LabelCell>(o => o.online, new Localizable("Online"), cell =>
                  {
                      cell.Class = Class.UseCondition(Condition.Boolean("green", "blue"));
                      cell.Text = Format.UseCondition(Condition.Boolean("{online.yes}", "{online.no}"));
                  })
                .Column<ArrayCell>(o => o.relations, new Localizable("Relations"), cell =>
                  {
                      cell.Text = new Localizable("{0} {key}");//todo confirm
                      cell.Action = ActionMeta.EmbeddedPopup(CreateRelationPopupMeta(), "?id={id}&by={key}&type=page");//todo confirm
                  })
                .Column<DateCell>(o => o.lastModified, new Localizable("Last modified"), null)
                .Column<LinkCell>(o => o.previewUrl, new Localizable("Preview"), cell =>
                  {
                      cell.Action = new ActionMeta
                      {
                          Type = ActionType.NewWindow
                      };
                  })
                .Column<ButtonCell>("setting", new Localizable(""), cell =>
                {
                    cell.Class = "btn-primary hidden-xs";
                    cell.Text = new Localizable("Setting");
                    cell.Action = new ActionMeta
                    {
                        Type = ActionType.Redirect,
                        Url = "/_admin/page/details?id={id}"
                    };
                })
                 .Column<ButtonCell>(o => o.inlineUrl, new Localizable(""), cell =>
                   {
                       cell.Class = "btn-primary hidden-xs";
                       cell.Text = new Localizable("Inline edit");
                       cell.Action = new ActionMeta
                       {
                           Type = ActionType.NewWindow
                       };
                   })
                .Column<IconCell>("qrCode", new Localizable("qrCode"), cell =>
                {
                    cell.Class = "btn-xs";
                    cell.IconClass = "qrcode";
                    cell.Action = new ActionMeta
                    {
                        Type = ActionType.Event,
                        Url = "showQrcode"
                    };
                });
            #endregion
        }

        #region popup
        private PopupMeta CreateCopyPopupMeta()
        {
            PopupMeta popupMeta = new PopupMeta();
            popupMeta.Title = new Localizable("Copy");
            popupMeta.Buttons.Add(new PopupButton()
            {
                Class = "green",
                Text = new Localizable("start"),
                Type = PageButtonAction.Submit
            });
            popupMeta.Buttons.Add(new PopupButton()
            {
                Class = "gray",
                Text = new Localizable("cancel"),
                Type = PageButtonAction.Close
            });

            var meta = new FormMeta();
            meta.LoadApi = "";
            meta.SubmitApi = "/page/copy";
            meta.Layout = FormLayout.Horizontal;

            meta.Builder<CopyForm>()
                .MergeModel()
                .Item(i => i.name, item =>
                {
                    item.Type = "textBox";
                    item.Label = new Localizable("name");
                    item.Class = "col-md-9";
                    item.Options = SelectOptions.UseContext("selected", "{name}_copy");

                    item.Rules.Add(new RequiredRule("required"));
                    item.Rules.Add(new MinLengthRule(1));
                    item.Rules.Add(new MaxLengthRule(64));
                    item.Rules.Add(new UniqueRule("/page/isUniqueName?name={name}", "taken"));
                })
                .Item(i => i.id, item =>
                {
                    item.Type = "hidden";
                    item.Label = new Localizable("id");
                    item.Name = "id";
                    item.Options = SelectOptions.UseContext("selected", "{id}");
                })
                .Item(i => i.url, item =>
                {
                    item.Type = "textBox";
                    item.Label = new Localizable("url");
                    item.Name = "url";
                    item.Class = "col-md-9";
                    item.Options = SelectOptions.UseContext("selected", "/{name}_copy");
                    item.Rules.Add(new RequiredRule("required"));
                    //item.Rules.Add(new RegexRule(".+", "invalid"));
                })
                //.Item(i => i.textarea, item =>
                //   {
                //       item.Type = "textarea";
                //       item.Label = "textarea";
                //       item.Name = "textarea";
                //       item.Class = "col-md-9";
                //       item.Rules.Add(new RequiredRule("required"));
                //   })
                //   .Item(i => i.radiobox, item =>
                //   {
                //       item.Type = "radiobox";
                //       item.Label = "radiobox";
                //       item.Name = "radiobox";
                //       item.Class = "col-md-9";
                //       item.Options = SelectOptions.UseList(new SelectOptions.OptionItem
                //       {
                //           Text = "url"
                //         ,
                //           Data = "url"
                //       }, new SelectOptions.OptionItem { Text = "File", Data = "File" }
                //     , new SelectOptions.OptionItem { Text = "Others", Data = "Others" });

                //       item.Rules.Add(new RequiredRule("required"));
                //   })
                //.Item(i => i.number, item =>
                // {
                //     item.Type = "number";
                //     item.Label = "number";
                //     item.Name = "number";
                //     item.Class = "col-md-9";
                     
                //     item.Rules.Add(new RequiredRule("required"));
                // })
                //.Item(i => i.datetime, item =>
                // {
                //     item.Type = "datetime";
                //     item.Label = "datetime";
                //     item.Name = "datetime";
                //     item.Class = "col-md-9";
                //     item.Rules.Add(new RequiredRule("required"));
                // })
                // .Item(i => i.checkbox, item =>
                // {
                //     item.Type = "checkbox";
                //     item.Label = "checkbox";
                //     item.Name = "checkbox";
                //     item.Class = "col-md-9";
                //     item.Options = SelectOptions.UseList(new SelectOptions.OptionItem
                //     {
                //         Text = "url"
                //         ,
                //         Data = "url"
                //     }, new SelectOptions.OptionItem { Text = "File", Data = "File" });
                //     item.Rules.Add(new RequiredRule("required"));
                // })
            ;
            popupMeta.Views.Add(meta);

            return popupMeta;
        }

        //move to common method method
        private PopupMeta CreateRelationPopupMeta()
        {
            PopupMeta popupMeta = new PopupMeta();
            popupMeta.Title =new Localizable("Relation");
            popupMeta.Buttons.Add(new PopupButton()
            {
                Class = "green",
                Text = new Localizable("OK"),
                Type = PageButtonAction.Close
            });

            var meta = new TableMeta();
            meta.DataApi = "Relation/ShowBy?id={id}&by={by}&type={type}";
            meta.ListName = "";
            meta.ShowSelected = false;
            meta.Builder<RelationTable>()
                .MergeModel()
                .Column<LinkCell>("name", new Localizable("Name"), (LinkCell cell) =>
                {
                    cell.Action = ActionMeta.NewWindow("{url}");

                })
                  .Column<TextCell>("remark", new Localizable("Remark"), null)
                  .Column<IconCell>("Edit", new Localizable("Edit"), (IconCell cell) =>
                  {
                      cell.Class = "blue btn-xs";
                      cell.IconClass = "pencil";
                      cell.Action = ActionMeta.NewWindow("/_Admin/Development/{by}?id={objectId}");
                  })
                  ;

            popupMeta.Views.Add(meta);

            return popupMeta;
        }

        private PopupMeta CreateRoutePopupMeta()
        {
            PopupMeta popupMeta = new PopupMeta();
            popupMeta.Title =new Localizable("Route setting");
            popupMeta.Description = new Description()
            {
                Title = new Localizable("Redirect routes"),
                Content = new Localizable("Set the redirect pages for default home page, 404 and error pages"),
                Closable = true,
                Class = "alert alert-info alert-dismissable"
            };
            popupMeta.Buttons.Add(new PopupButton()
            {
                Class = "green",
                Text = new Localizable("save"),
                Type = PageButtonAction.Submit
            });
            popupMeta.Buttons.Add(new PopupButton()
            {
                Class = "gray",
                Text = new Localizable("cancel"),
                Type = PageButtonAction.Close
            });

            var meta = new FormMeta();
            meta.LoadApi = "/Page/DefaultRoute";
            meta.SubmitApi = "/page/defaultRouteUpdate";
            meta.Layout = FormLayout.Horizontal;

            meta.Builder<RouteFormMeta>()
                .MergeModel()
                .Item((i) => i.startPage, (FormItem item) =>
                {
                    item.Type = "selection";
                    item.Label = new Localizable("Home page");
                    item.Name = "startPage";
                    item.Options = SelectOptions.UseContext("context", "{name}", "{id}");


                })
                .Item((i) => i.notFound, (FormItem item) =>
                {
                    item.Type = "selection";
                    item.Label = new Localizable("404 page");
                    item.Name = "notFound";
                    item.Options = SelectOptions.UseContext("context", "{name}", "{id}");
                    item.Placeholder = new Localizable("System default");
                    item.DefaultValue = ValueContext.DirectValue(Guid.Empty.ToString());
                })
                .Item((i) => i.error, item =>
                {
                    item.Type = "selection";
                    item.Label = new Localizable("Error page");
                    item.Name = "error";
                    item.Options = SelectOptions.UseContext("context", "{name}", "{id}");
                    item.Placeholder = new Localizable("System default");
                    item.DefaultValue = ValueContext.DirectValue(Guid.Empty.ToString());
                })
                ;
            popupMeta.Views.Add(meta);

            return popupMeta;
        }

        #endregion
    }

    class PageTest : Kooboo.Data.Interface.ISiteObject
    {
        public byte ConstType { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModified { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    class PageViewModel
    {
        //public Guid Id { get; set; }
        public string name { get; set; }

        //public string Title { get; set; }

        //public int Warning { get; set; }
        //[BadgeColumn("Linked")]
        public string linked { get; set; }

        //public int PageView { get; set; }

        //public Guid LayoutId { get; set; }
        //[LabelColumn("Online")]
        ////cell
        public string online { get; set; }

        //[ArrayColumn("Relations")]
        public Dictionary<string, int> relations { get; set; } = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        //[DateColumn("Last modified")]
        public string lastModified { get; set; }

        //public string Path { get; set; }
        //how to specify url from data
        //[LinkColumn("Preview",ActionType.NewWindow,null)]
        public string previewUrl { get; set; }

        public string inlineUrl { get; set; }

        //public bool StartPage { get; set; }
    
        //[JsonConverter(typeof(StringEnumConverter))]
        //public PageType Type { get; set; }

    }
    class CopyForm
    {
        //[RequiredRule("required")]
        //[MinLengthRule(1)]
        //[MaxLengthRule(64)]
        //[UniqueRule("/page/isUniqueName?name={name}","take")]
        public string name { get; set; }

        public string id { get; set; }

        public string url { get; set; }

        //public string textarea { get; set; }

        //public string radiobox { get; set; }

        //public string number { get; set; }

        //public string datetime { get; set; }

        //public string checkbox { get; set; }

    }
    class RelationTable { }

    class RouteFormMeta
    {
        public string startPage { get; set; }

        public string notFound { get; set; }

        public string error { get; set; }
    }
}
