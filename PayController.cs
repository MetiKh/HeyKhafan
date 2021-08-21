using Dto.Payment;
using EndPoint.Site.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyStore.Application.Services.Cart;
using MyStore.Application.Services.Order.Command.AddOrder;
using MyStore.Application.Services.Pay.Command.AddRequestPay;
using MyStore.Application.Services.Pay.Query.GetRequestPayService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZarinPal.Class;

namespace EndPoint.Site.Controllers
{
      [Authorize]
    public class PayController : Controller
    {
        private readonly Payment _payment;
        private readonly Authority _authority;
        private readonly Transactions _transactions;
        ICartService _cartService;
        IAddRequestPayService _requestPayService;
        CookiesManager _cookiesManager;
        IGetRequestPayService _getRequestPayService;
        IAddOrderService _addOrderService;
        public PayController(ICartService cartService, IAddRequestPayService requestPayService,IGetRequestPayService getRequestPayService
            , IAddOrderService addOrderService)
        {
            _getRequestPayService = getRequestPayService;
            var expose = new Expose();
            _payment = expose.CreatePayment();
            _authority = expose.CreateAuthority();
            _transactions = expose.CreateTransactions();
            _requestPayService = requestPayService;
            _cartService = cartService;
            _cookiesManager = new CookiesManager();
            _addOrderService = addOrderService;
        }

        public async Task<IActionResult> Index()
        {
            long? userId = ClaimUtility.GetUserId(User);
            var cart = _cartService.GetMyCart(userId, _cookiesManager.GetBrowserID(HttpContext));
            if (cart.Data.SumAmount > 0)
            {
                var requestPay = _requestPayService.Execute(userId.Value, cart.Data.SumAmount);
                var result = await _payment.Request(new DtoRequest()
                {
                    Mobile = "09121112222",
                    CallbackUrl = $"https://localhost:44334/pay/Verify?guid={requestPay.Data.Guid}",
                    Description = $"پرداخت فاکتور شماره {requestPay.Data.RequestPayID}",
                    Email = requestPay.Data.Email,
                    Amount = (int)requestPay.Data.Amount,
                    MerchantId = "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX"
                }, ZarinPal.Class.Payment.Mode.sandbox);
                return Redirect($"https://sandbox.zarinpal.com/pg/StartPay/{result.Authority}");
            }
            else
            {
                return RedirectToAction("index","cart");
            }

        
        }

        public async Task<IActionResult> Verify(Guid guid,string authority, string status)
        {
            var requestPay = _getRequestPayService.Execute(guid);
            var verification = await _payment.Verification(new DtoVerification
            {
                 
                Amount = (int)requestPay.Data.Amount,
                MerchantId = "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX",
                Authority = authority
            }, Payment.Mode.sandbox);

            long? userID = ClaimUtility.GetUserId(User);
            var cart = _cartService.GetMyCart(userID, _cookiesManager.GetBrowserID(HttpContext));

            if (verification.Status == 100)
            {
                _addOrderService.Execute(new RequestAddOrderDto
                {
                    UserID = userID.Value,
                    CartID = cart.Data.CartID,
                    RequestPayID = requestPay.Data.RequestPayID
                });
                return RedirectToAction("Index", "Order");
            }
            else
            {

            }

            return View();
        }
        public class VerificationPayResultDto
        {
            public int Status { get; set; }
            public long RefID { get; set; }
        }

    }
}
