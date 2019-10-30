//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Dom;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.Render
{
    public class AttributeEvaluator : IEvaluator
    {
        private Dictionary<string, string> _appendattributes;

        private Dictionary<string, string> AppendAttributes
        {
            get
            {
                return _appendattributes ?? (_appendattributes =
                           new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                           {
                               {"style", ";"}, {"class", " "}
                           });
            }
        }

        public EvaluatorResponse Evaluate(Node node, EvaluatorOption options)
        {
            if (options.IgnoreEvaluators.HasFlag(EnumEvaluator.Attribute))
            {
                return null;
            }

            Dictionary<string, string> appendValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (node.nodeType != enumNodeType.ELEMENT)
            {
                return null;
            }
            var element = node as Element;

            string attName = null;
            foreach (var item in element.attributes)
            {
                if (item.name == "tal-attribute" || item.name == "k-attribute" || item.name == "tal-attributes" || item.name == "k-attributes")
                {
                    attName = item.name;
                    break;
                }
            }

            if (string.IsNullOrEmpty(attName))
            {
                return null;
            }

            string attributeValues = element.getAttribute(attName);
            if (!options.RequireBindingInfo)
            {
                element.removeAttribute(attName);
            }

            if (string.IsNullOrEmpty(attributeValues) || attributeValues.IndexOf(' ') < 0)
            {
                return null;
            }

            EvaluatorResponse response = new EvaluatorResponse();

            string[] attributes = attributeValues.Split(';');

            foreach (var item in attributes)
            {
                var attkeyvalue = ParseAtt(item);
                if (attkeyvalue == null)
                {
                    continue;
                }

                string attributeName = attkeyvalue.Key;
                string attributeValue = attkeyvalue.Value;

                if (AppendAttributes.ContainsKey(attributeName))
                {
                    string sep = AppendAttributes[attributeName];
                    string value = element.getAttribute(attributeName);

                    if (!string.IsNullOrEmpty(value))
                    {
                        if (!value.Trim().EndsWith(sep))
                        {
                            value = value + sep;
                        }
                        if (appendValues.ContainsKey(attributeName))
                        {
                            var orgvalue = appendValues[attributeName];
                            value = orgvalue + value;
                        }
                        appendValues[attributeName] = value;
                    }
                }

                List<IRenderTask> tasks = new List<IRenderTask> {new ContentRenderTask(" " + attributeName + "=\"")};

                if (appendValues.ContainsKey(attributeName))
                {
                    tasks.Add(new ContentRenderTask(appendValues[attributeName]));
                }

                tasks.Add(new ValueRenderTask(attributeValue));
                tasks.Add(new ContentRenderTask("\""));

                if (response.AttributeTask == null)
                {
                    response.AttributeTask = tasks;
                }
                else
                {
                    response.AttributeTask.AddRange(tasks);
                }

                element.removeAttribute(attributeName);

                if (options.RequireBindingInfo)
                {
                    string koobooid = element.getAttribute(SiteConstants.KoobooIdAttributeName);
                    BindingObjectRenderTask binding = new BindingObjectRenderTask() { ObjectType = "attribute", AttributeName = attributeName, BindingValue = attributeValue, KoobooId = koobooid };
                    List<IRenderTask> bindings = new List<IRenderTask> {binding};
                    if (response.BindingTask == null)
                    {
                        response.BindingTask = bindings;
                    }
                    else
                    {
                        response.BindingTask.AddRange(bindings);
                    }
                }
            }

            if (response.AttributeTask == null || response.AttributeTask.Count == 0)
            {
                return null;
            }
            else
            {
                return response;
            }
        }

        private AttKeyValue ParseAtt(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            input = input.Trim();

            int spaceindex = input.IndexOf(" ");
            if (spaceindex == -1)
            {
                return null;
            }

            AttKeyValue result = new AttKeyValue
            {
                Key = input.Substring(0, spaceindex), Value = input.Substring(spaceindex).Trim()
            };

            return result;
        }

        public class AttKeyValue
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        public class AppendValues
        {
            public string AttName { get; set; }
            public string Value { get; set; }
        }
    }
}