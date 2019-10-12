//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using System.Collections.Generic;

namespace Kooboo.Sites.InlineEditor
{
    public static class UpdateManager
    {
        public static void Execute(RenderContext context, List<IInlineModel> updatelist)
        {
            // ���������з�����ƻ��޸�˳�򣬻ᵼ���޸��쳣��
            foreach (var item in updatelist)
            {
                Execute(context, item.EditorType, new List<IInlineModel> { item });
            }
        }

        public static void Execute(RenderContext context, string editortype, List<IInlineModel> updates)
        {
            var executor = EditorContainer.GetExecutor(editortype);
            executor.Execute(context, updates);
        }
    }
}