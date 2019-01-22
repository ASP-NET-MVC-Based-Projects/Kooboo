﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Dom.CSS.Tokens;

namespace Kooboo.Dom.CSS.rawmodel
{
  public static  class ComponentValueExtension
    {

      public static string getString(List<ComponentValue> valuelist, ref int startindex, ref int endindex, ref string CssText)
      {
          string returnstring = string.Empty;

          foreach (var item in valuelist)
          {
              if (startindex <= 0)
              {
                  startindex = item.startindex;
              }

              if (item.endindex > endindex)
              {
                  endindex = item.endindex; 
              }

              if (item.Type == CompoenentValueType.preservedToken)
              {
                  PreservedToken pretoken = item as PreservedToken;
                  returnstring += pretoken.token.GetString(ref CssText);
              }
              else if (item.Type == CompoenentValueType.function)
              {
                  Function func = item as Function;
                  returnstring += func.getString(ref CssText);

              }
              else if (item.Type == CompoenentValueType.simpleBlock)
              {
                  SimpleBlock simpleblock = item as SimpleBlock;
                  returnstring += simpleblock.getString(ref CssText);
              }
              
          }


          return returnstring.Trim(); 
      }

    }
}
