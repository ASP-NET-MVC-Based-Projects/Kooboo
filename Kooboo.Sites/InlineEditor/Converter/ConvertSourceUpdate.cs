﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.InlineEditor.Converter
{
   public class ConvertSourceUpdate
    {
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public string NewValue { get; set; }
        public Guid PageId { get; set; }
    }
}
