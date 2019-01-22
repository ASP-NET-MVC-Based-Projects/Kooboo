﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Dom
{
 public   class ElementPointer
    {

// Initially, the head element pointer and the form element pointer are both null.

//Once a head element has been parsed (whether implicitly or explicitly) the head element pointer gets set to point to this node.

//The form element pointer points to the last form element that was opened and whose end tag has not yet been seen. It is used to make form controls associate with forms in the face of dramatically bad markup, for historical reasons. It is ignored inside template elements.

     public Element head;
     public Element form;

    }
}
