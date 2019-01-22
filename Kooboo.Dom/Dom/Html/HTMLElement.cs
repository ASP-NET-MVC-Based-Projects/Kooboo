﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Dom
{
   public class HTMLElement : Element
    {

       public string title;
       public string lang;
       public bool translate;

       public string dir;

       public string dataset;

       public bool hidden;

       public int tabIndex;
       
       public string accessKey;

       public string accessKeyLabel;

       public bool  draggable;
       
        //[PutForwards=value] readonly attribute DOMSettableTokenList dropzone;

       public string contentEditable;
       
       public bool isContentEditable;

       public bool spellcheck;


       public void click()
       {
           throw new NotImplementedException();
       }
       public void focus()
       {
           throw new NotImplementedException();
       }
      public void blur()
       {
           throw new NotImplementedException();
       }

    }
}
