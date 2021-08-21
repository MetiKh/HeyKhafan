using Bugeto_Store.Application.Interfaces.FacadPatterns;
using EndPoint.Site.Models;
using EndPoint.Site.Models.ViewModels.HomePage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyStore.Application.Interfaces.FacadPatterns;
using MyStore.Application.Services.Common.GetHomePageImages;
using MyStore.Application.Services.HomePage.GetSliders;
using MyStore.Application.Services.Products.Queries.GetProductsForSite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EndPoint.Site.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IGetSlidersService _getSlidersService;
        private readonly IGetHomePageImagesService _getHomePageImagesService;
        private readonly IProductFacadeSite _productFacadeSite;
        public HomeController(ILogger<HomeController> logger,IGetSlidersService getSlidersService,IProductFacadeSite productFacadeSite, IGetHomePageImagesService getHomePageImagesService)
        {
            _getSlidersService = getSlidersService;
            _getHomePageImagesService= getHomePageImagesService;
            _productFacadeSite = productFacadeSite;
            _logger = logger;
        }

        public IActionResult Index()
        {
            HomePageViewModel homePage = new HomePageViewModel {
                GetSliders=_getSlidersService.Execute().Data
                ,GetHomePageImages=_getHomePageImagesService.Execute().Data
               ,GetMobiles=_productFacadeSite.GetProductForSiteSetvice.Execute(Ordering.theNewest,1,6,null,25).Data.Products
                };
            return View(homePage);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
