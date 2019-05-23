﻿using System;
using System.Collections.Generic;
using System.Linq;

using Kooboo.Dom;

namespace Kooboo.Model.Render.Parsers
{
    public class SubmitParser : IVirtualElementParser
    {
        public const string ModelAttribute = "model";
        public const string Data_ModelName = "modelName";
        public const string Data_Url = "url";

        public string Name => "submit";

        public int Priority => ParserPriority.Normal;

        public void Parse(ElementWrap el, TagParseContext context, Action visitChildren)
        {
            // Get url, modelName
            var url = el.getAttribute(context.Options.GetAttributeName(Name));
            var modelName = el.getAttribute(context.Options.GetAttributeName(ModelAttribute));
            if (String.IsNullOrEmpty(modelName))
            {
                modelName = ParserHelper.GetModelNameFromUrl(url);
            }

            context.Set(Data_Url, url);
            context.Set(Data_ModelName, modelName);

            // Get meta
            var meta = context.Options.ApiMetaProvider.GetMeta(url);

            // action attribute
            var urlWithParams = ParserHelper.GenerateUrlFromApiParameters(url, meta.Parameters);
            el.setAttribute("action", urlWithParams);

            // method attributes
            if (String.IsNullOrEmpty(el.getAttribute("method")))
            {
                el.setAttribute("method", "post");
            }

            // @submit.native
            el.setAttribute("@submit.prevent", $"{Vue.SubmitData.Keyword_Submit}_{modelName}");

            // data model
            context.Js.Data(modelName, ParserHelper.GetJsonFromModel(meta.Model));

            // sub view
            if (context.ViewContext.ViewType == ViewType.Sub)
            {
                context.Js.Data(LogicKeywords.PropsData, null).Load();
            }

            // submit method
            context.Js.Submit(urlWithParams, modelName);


            // validations
            var props = meta.Model.Properties.Where(o => o.Rules.Any()).ToArray();
            context.Js.Validation(modelName, props);

            visitChildren?.Invoke();
        }
    }
}
