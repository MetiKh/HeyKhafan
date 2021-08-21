using EndPoint.Site.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyStore.Application.Services.Order.Query.GetOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EndPoint.Site.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        IGetOrdersService _getOrdersService;
      
        public OrderController(IGetOrdersService getOrdersService)
        {
            _getOrdersService = getOrdersService;
            
        }

        public IActionResult Index()
        {
            long? userID = ClaimUtility.GetUserId(User);
            return View(_getOrdersService.Execute(userID.Value).Data);
        }
    }
}
