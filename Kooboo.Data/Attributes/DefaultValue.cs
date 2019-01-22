﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Attributes
{
    [AttributeUsage(AttributeTargets.Property| AttributeTargets.Field)]
   public class DefaultValue : Attribute
    {
        public string Value { get; set; }

        public DefaultValue(string input)
        {
            this.Value = input; 
        }
    } 
}
