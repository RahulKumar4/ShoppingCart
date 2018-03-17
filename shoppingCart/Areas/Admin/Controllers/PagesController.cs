using shoppingCart.Models.Data;
using shoppingCart.Models.ViewModel.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace shoppingCart.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            //Declare list of pageVM
            List<PageVM> pageList;
            
           
            using (DB db = new DB())
            {
                //initilized the list
                pageList = db.Pages.ToArray().OrderBy(x => x.shorting).Select(x => new PageVM(x)).ToList(); 

            }

            //return view with list

            return View(pageList);
        }
    }
}