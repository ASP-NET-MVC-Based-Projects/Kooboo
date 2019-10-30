//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System.Text;

namespace Kooboo.Sites.Render
{
    public static class StyleRenderer
    {
        public static void Render(FrontContext context)
        {
            var css = context.SiteDb.Styles.Get(context.Route.objectId);
            context.RenderContext.Response.ContentType = "text/css;charset=utf-8";

            if (css?.Body != null)
            {
                // var body = GetBody(css);
                context.RenderContext.Response.Body = Encoding.UTF8.GetBytes(css.Body);
            }
        }

        public static string GetBody(Kooboo.Sites.Models.Style style)
        {
            if (style == null || string.IsNullOrEmpty(style.Body))
            {
                return null;
            }

            if (style.Extension == null || style.Extension == "css" || style.Extension == ".css")
            {
                return style.Body;
            }
            else
            {
                var styleEngines = Kooboo.Sites.Engine.Manager.GetStyle();

                var find = styleEngines.Find(o => o.Extension == style.Extension);
                return find != null ? find.Execute(null, style.Body) : style.Body;
            }
        }
    }
}