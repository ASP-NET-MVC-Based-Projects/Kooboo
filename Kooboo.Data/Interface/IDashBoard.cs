﻿using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Interface
{
    public interface IDashBoard
    {
        string Name { get; }

        string DisplayName(RenderContext Context); 
        IDashBoardResponse Render(RenderContext Context);    
    }

    
}
