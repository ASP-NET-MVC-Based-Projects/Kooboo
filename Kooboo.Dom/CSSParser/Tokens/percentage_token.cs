//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Dom.CSS.Tokens
{
 public   class percentage_token : cssToken
    {

     public percentage_token()
     {

         this.Type = enumTokenType.percentage;

     }

   
     public string representation;

     public double Number;



    }
}
