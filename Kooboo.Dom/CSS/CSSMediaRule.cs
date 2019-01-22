﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Dom.CSS
{
    /// <summary>
    /// The CSSMediaRule is an interface representing a single CSS @media rule. 
    // It implements the CSSConditionRule interface, and therefore the CSSGroupingRule and the CSSRule interface with a type value of 4 (CSSRule.MEDIA_RULE)
    /// </summary>
   [Serializable]
   public class CSSMediaRule : CSSConditionRule
    {
       public CSSMediaRule()
       {
           base.type = enumCSSRuleType.MEDIA_RULE;
           media = new MediaList();
       }

       public MediaList media;


    }
}
