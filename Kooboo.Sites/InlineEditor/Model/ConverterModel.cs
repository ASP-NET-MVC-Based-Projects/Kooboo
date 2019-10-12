//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.

namespace Kooboo.Sites.InlineEditor.Model
{
    public class ConverterModel : IInlineModel
    {
        public ActionType Action
        {
            get; set;
        }

        public string EditorType
        {
            get
            {
                return "converter";
            }
        }

        public string NameOrId
        {
            get; set;
        }

        public string ObjectType
        {
            get; set;
        }

        public string Value
        {
            get; set;
        }

        public string type { get; set; }

        public string convertResult { get; set; }
    }
}