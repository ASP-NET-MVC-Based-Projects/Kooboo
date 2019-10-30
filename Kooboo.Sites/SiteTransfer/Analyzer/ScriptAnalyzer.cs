//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Dom;
using Kooboo.Lib.Helper;

namespace Kooboo.Sites.SiteTransfer
{
    public class ScriptAnalyzer : ITransferAnalyzer
    {
        public void Execute(AnalyzerContext context)
        {
            int embeddedItemIndex = 0;

            HTMLCollection scripts = context.Dom.getElementsByTagName("script");

            foreach (var item in scripts.item)
            {
                if (item.hasAttribute("src"))
                {
                    string srcurl = Service.DomUrlService.GetLinkOrSrc(item);

                    if (string.IsNullOrEmpty(srcurl))
                    {
                        // script tag with a src source. does not consider as a script.
                        continue;
                    }

                    string fullurl = UrlHelper.Combine(context.AbsoluteUrl, srcurl);

                    bool issamehost = Kooboo.Lib.Helper.UrlHelper.isSameHost(context.OriginalImportUrl, fullurl);

                    if (issamehost)
                    {
                        string relativeurl = UrlHelper.RelativePath(fullurl, issamehost);
                        relativeurl = TransferHelper.TrimQuestionMark(relativeurl);

                        if (srcurl != relativeurl)
                        {
                            string oldstring = Kooboo.Sites.Service.DomService.GetOpenTag(item);
                            string newstring = oldstring.Replace(srcurl, relativeurl);
                            context.Changes.Add(new AnalyzerUpdate()
                            {
                                StartIndex = item.location.openTokenStartIndex,
                                EndIndex = item.location.openTokenEndIndex,
                                NewValue = newstring
                            });
                        }

                        context.DownloadManager.AddTask(new Download.DownloadTask()
                        {
                            AbsoluteUrl = fullurl,
                            RelativeUrl = relativeurl,
                            ConstType = ConstObjectType.Script,
                            OwnerObjectId = context.ObjectId
                        });
                    }
                }
                else
                {
                    //string text = item.InnerHtml;
                    //if (!string.IsNullOrEmpty(text))
                    //{
                    //    // this is an embedded script.
                    //    var script = new Script
                    //    {
                    //        IsEmbedded = true,
                    //        Body = text,
                    //        OwnerObjectId = Context.ObjectId,
                    //        OwnerConstType = Context.ObjectType,
                    //        ItemIndex = embeddedItemIndex,
                    //        Name = UrlHelper.FileName(Context.AbsoluteUrl)
                    //    };

                    //    embeddedItemIndex += 1;

                    //    Context.SiteDb.Scripts.AddOrUpdate(script);

                    //}
                }
            }
        }
    }
}