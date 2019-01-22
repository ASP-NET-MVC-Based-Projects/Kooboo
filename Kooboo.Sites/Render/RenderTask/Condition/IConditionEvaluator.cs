﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Render.RenderTasks.Tal
{
   public interface IConditionEvaluator
    {
        /// <summary>
        /// ><=, contains, !=, 
        /// </summary>
        string ConditionOperator{ get;}

        string LeftExpression { get; set; }

        string RightValue { get; set; }

        bool Check(Kooboo.Data.Context.DataContext context); 

    }
     
}
