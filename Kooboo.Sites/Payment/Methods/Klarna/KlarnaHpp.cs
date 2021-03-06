﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using Kooboo.Data.Context;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Payment.Methods.Klarna.lib;
using Kooboo.Sites.Payment.Response;

namespace Kooboo.Sites.Payment.Methods.Klarna
{
    public class KlarnaHpp : IPaymentMethod<KlarnaHppSetting>
    {
        public string Name => "KlarnaHpp";

        public string DisplayName => Data.Language.Hardcoded.GetValue("KlarnaHpp", Context);

        // icons can be found here: https://developers.klarna.com/resources/branding/
        public string Icon => "https://x.klarnacdn.net/payment-method/assets/badges/generic/klarna.svg";

        public string IconType => "img";

        public List<string> supportedCurrency => new List<string> { "USD", "EUR", "GBP", "DKK", "NOK", "SEK" };

        public RenderContext Context { get; set; }

        public KlarnaHppSetting Setting { get; set; }

        [Description(@"<script engine='kscript'>
    var charge = {};
    charge.total = 150; // $1.50
    charge.currency='USD';
    charge.country='US';
    charge.name = 'green tea order'; 
    charge.description = 'The best tea from Xiamen';  
    var res = k.payment.klarnaHpp.charge(charge);  
    k.response.redirect(res.redirectUrl);
    // var hppSessionId = res.paymemtMethodReferenceId;
</script>")]
        public IPaymentResponse Charge(PaymentRequest request)
        {
            var req = new KpSessionRequest
            {
                PurchaseCurrency = request.Currency,
                PurchaseCountry = request.Country,
                OrderAmount = request.TotalAmount,
                OrderLines = new[]
                {
                    new KpSessionRequest.OrderLine
                    {
                        Name = request.Name,
                        Quantity = 1,
                        UnitPrice = request.TotalAmount,
                        TotalAmount = request.TotalAmount
                    }
                },
            };

            var callbackUrl = PaymentHelper.GetCallbackUrl(this, nameof(Notify), Context);

            var apiClient = new KlarnaApi(Setting, request.Country);
            var kpSession = apiClient.CreateKpSession(req);
            var hppSession = apiClient.CreateHppSession(kpSession.SessionId, Setting.GetGetMerchantUrls(callbackUrl, request.Id));

            return new RedirectResponse(hppSession.RedirectUrl, request.Id)
            {
                paymemtMethodReferenceId = hppSession.SessionId
            };
        }

        public PaymentStatusResponse checkStatus(PaymentRequest request)
        {
            var result = new PaymentStatusResponse { HasResult = true };
            try
            {
                var hppSessionId = request.ReferenceId;
                var statusResponse = new KlarnaApi(Setting, request.Country).CheckStatus(hppSessionId);

                result.Status = ConvertStatus(statusResponse.Status);
            }
            catch (Exception ex)
            {
                Kooboo.Data.Log.Instance.Exception.WriteException(ex);
            }

            return result;
        }

        public PaymentCallback Notify(RenderContext context)
        {
            var body = context.Request.Body;
            var requestId = context.Request.Get("secretToken");
            if (!Guid.TryParse(requestId, out var id) || PaymentManager.GetRequest(id, context) == null)
            {
                return null;
            }

            var data = JsonHelper.Deserialize<CallbackRequest>(body);
            var result = new PaymentCallback
            {
                RequestId = id,
                RawData = body,
                CallbackResponse = new Callback.CallbackResponse { StatusCode = 204 },
                Status = ConvertStatus(data.Session.Status)
            };

            return result;
        }

        private PaymentStatus ConvertStatus(KlarnaStatus status)
        {
            switch (status)
            {
                case KlarnaStatus.COMPLETED:
                    return PaymentStatus.Paid;
                case KlarnaStatus.CANCELLED:
                case KlarnaStatus.BACK:
                case KlarnaStatus.DISABLED:
                    return PaymentStatus.Cancelled;
                case KlarnaStatus.ERROR:
                case KlarnaStatus.FAILED:
                    return PaymentStatus.Rejected;
                case KlarnaStatus.WAITING:
                case KlarnaStatus.IN_PROGRESS:
                    return PaymentStatus.Pending;
                default:
                    return PaymentStatus.NotAvailable;
            }
        }
    }
}