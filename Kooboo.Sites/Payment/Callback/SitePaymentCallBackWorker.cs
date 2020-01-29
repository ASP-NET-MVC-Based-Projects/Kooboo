﻿using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Payment.Repository;

namespace Kooboo.Sites.Payment
{
    public class SitePaymentCallbackWorker : IPaymentCallbackWorker
    {
        public void Process(PaymentCallback callback, RenderContext context)
        {
            if (context.WebSite != null && callback.RequestId != default(Guid))
            {
                var sitedb = context.WebSite.SiteDb();

               var callbackrepo =  sitedb.GetSiteRepository<PaymentCallBackRepository>();

                callbackrepo.AddOrUpdate(callback); 

                var requestrepo = sitedb.GetSiteRepository<PaymentRequestRepository>();

                if (callback.Paid)
                { 
                    requestrepo.Store.UpdateColumn<bool>(callback.RequestId, o => o.Paid, true); 
                }
                else if (callback.Cancelled || callback.Rejected)
                {
                    requestrepo.Store.UpdateColumn<bool>(callback.RequestId, o => o.Failed, true);
                } 
            }
        }
    }
}
