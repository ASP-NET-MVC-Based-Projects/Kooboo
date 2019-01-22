﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Mail.ViewModel
{
  public  class ComposeViewModel
    {
        public int? MessageId { get; set; }

        public string Subject { get; set; }

        public int From { get; set; }


        private List<string> _to; 

        public List<string> To {
            get
            {
                if (_to == null)
                {
                    _to = new List<string>(); 
                }
                return _to; 
            }
            set
            {
                _to = value; 
            }
        }

        private List<string> _Cc; 

        public List<string> Cc  {
            get
            {
                if (_Cc == null)
                {
                    _Cc = new List<string>(); 
                }
                return _Cc; 
            }
            set
            {
                _Cc = value; 
            }
        }

        private List<string> _Bcc; 

        public List<string> Bcc {
            get
            {
                if (_Bcc == null)
                {
                    _Bcc = new List<string>();
                }
                return _Bcc; 
            }
            set
            {
                _Bcc = value; 
            }
        }

        private List<Models.Attachment> _attachments; 

        public List<Models.Attachment> Attachments {
            get 
            {
                if (_attachments == null)
                {
                    _attachments = new List<Models.Attachment>(); 
                }
                return _attachments;
            }
            set
            {
                _attachments = value; 
            }
        }
          
        public string Html { get; set; }
    }
}
