using EndPoint.Site.Utilities;
using Microsoft.AspNetCore.Mvc;
using MyStore.Application.Services.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EndPoint.Site.Controllers
{
    public class CartController : Controller
    {
        ICartService _cartService;
        CookiesManager _cookiesManager;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
            _cookiesManager = new CookiesManager();
        }

        public IActionResult Index()
        {
            var userId = ClaimUtility.GetUserId(User);
            return View(_cartService.GetMyCart(userId,_cookiesManager.GetBrowserID(HttpContext)).Data);
        }

        public IActionResult AddToCart(long ID)
        {
            _cartService.AddToCart(ID, _cookiesManager.GetBrowserID(HttpContext));
            return RedirectToAction("Index");
        }
        public IActionResult Delete(long ID)
        {
            _cartService.RemoveFromCart(ID, _cookiesManager.GetBrowserID(HttpContext));
            return RedirectToAction("Index");
        }

        public IActionResult Add(long CartItemId)
        {
            _cartService.Add(CartItemId);
            return RedirectToAction("Index");
        }
        public IActionResult LowOff(long CartItemId)
        {
            _cartService.LowOff(CartItemId);
            return RedirectToAction("Index");
        }
    }
}
