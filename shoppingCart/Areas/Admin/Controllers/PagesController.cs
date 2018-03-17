﻿using shoppingCart.Models.Data;
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
        // GET: Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }
        // Post: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            //Check model state
            if(!ModelState.IsValid )
            {
                return View(model);
            }

            using (DB db = new DB())
            {
                string slug;

                PageDTO dto = new PageDTO();

                //check for slug and set
                if(string.IsNullOrWhiteSpace(model.Slug ))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();

                }

                //check any unique slug and title
                if(db.Pages.Any(x => x.Title == model.Title ) || db.Pages.Any(x => x.Slug == model.Slug))
                {
                    ModelState.AddModelError("","That title or slug already exists.");
                    return View(model);
                }

                //adding value to dto
                dto.Slug = slug;
                dto.Title = model.Title;
                dto.Body = model.Body;
                dto.hasSideBar = model.hasSideBar;
                dto.shorting = 100;

                //save dto
                db.Pages.Add(dto);
                db.SaveChanges(); 

            }

            //set temp message (SM -> Sucess message)
            TempData["SM"] = "You have added a new page!";

            //After save redirect to add page.
            return RedirectToAction("AddPage");

        }

        public ActionResult EditPage(int id)
        {
            PageVM model;

            using(DB db = new DB ())
            {
                PageDTO dto = db.Pages.Find(id);

                if(dto == null)
                {
                    return Content("The page does not exist.");

                }

                model = new PageVM(dto);


            }
            return View(model);
        } 

    }
}