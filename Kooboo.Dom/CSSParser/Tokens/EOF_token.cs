﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Dom.CSS.Tokens
{
  public  class EOF_token : cssToken
    {
      public EOF_token()
      {
          this.Type = enumTokenType.EOF;
      }

    }
}
