//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Dom
{
  public  class HTMLScriptElement : Element
    {
        public HTMLScriptElement()
        {
          
        }

        public string src;
        public string type;
        public string charset;
        public bool async;
        public bool defer;
        public string crossOrigin;
        public string text;
      
    }
}
