using Microsoft.AspNetCore.Mvc;
using MyStore.Application.Interfaces.FacadPatterns;
using MyStore.Application.Services.Products.Queries.GetProductsForSite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EndPoint.Site.Controllers
{
    public class ProductsController : Controller
    {
        private IProductFacadeSite _productFacadeSite;
        public ProductsController(IProductFacadeSite productFacadeSite)
        {
            _productFacadeSite = productFacadeSite;
        }
        public IActionResult Index(Ordering order,string SearchKey,int page =1,long? CatID=null,int pageSize=20 )
        {
            return View(_productFacadeSite.GetProductForSiteSetvice.Execute(order,page,CatID,SearchKey,pageSize).Data);
        }

        public IActionResult Detail(long ID)
        {
            return View(_productFacadeSite.GetProductDetailForSiteService.Execute(ID).Data);
        }
    }
}
