using shoppingCart.Models.Data;
using shoppingCart.Models.ViewModel.Shop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using PagedList;
namespace shoppingCart.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        // GET: Admin/Shop/Categories

        public ActionResult Categories()
        {

            List<CategoriesVM> CategoriesList;

            using (DB db = new DB())
            {

                CategoriesList = db.Categories
                                    .ToArray()
                                    .OrderBy(x => x.Sorting)
                                    .Select(x => new CategoriesVM(x))
                                    .ToList();

            }


            return View(CategoriesList);
        }

        // POST: Admin/Shop/AddNewCategory
        [HttpPost]
        public string AddNewCategory(string catName, string catDesc)
        {
            string id;
            // string catName = cat[0];
            //tring catDesc = cat[1];
            using (DB db = new DB())
            {
                if (db.Categories.Any(x => x.Name == catName))
                    return "titletaken";

                CategoriesDTO dto = new CategoriesDTO();

                dto.Name = catName;
                dto.Slug = catName.Replace(" ", "-");
                dto.Description = catDesc;
                dto.Sorting = 100;

                db.Categories.Add(dto);
                db.SaveChanges();

                id = dto.Id.ToString();

            }
            return id;
        }

        // POST: Admin/Shop/ReorderCagegories
        [HttpPost]
        public void ReorderCagegories(int[] id)
        {
            using (DB db = new DB())
            {
                //initialize count
                int count = 1;

                //declare dto
                CategoriesDTO dto;


                //set shorting each page
                foreach (var CategoryId in id)
                {
                    dto = db.Categories.Find(CategoryId);
                    dto.Sorting = count;

                    db.SaveChanges();
                    count++;
                }
            }
        }

        // GET: Admin/shop/DeleteCategory/id

        public ActionResult DeleteCategory(int id)
        {

            using (DB db = new DB())
            {
                CategoriesDTO dto = db.Categories.Find(id);

                db.Categories.Remove(dto);

                db.SaveChanges();
            }
            //set temp message (SM -> Sucess message)
            TempData["SM"] = "You have deleted the Category!";

            //After save redirect to Categories .
            return RedirectToAction("Categories");

        }

        // POST: Admin/shop/EditCategory/id
        [HttpPost]
        public string EditCategory(string Name, string Desc, int id)
        {

            using (DB db = new DB())
            {
                CategoriesDTO dto = db.Categories.Find(id);

                if (dto.Name != Name)
                {
                    if (db.Categories.Any(x => x.Name == Name))
                    {
                        return "titletaken";
                    }
                }

                dto.Name = Name;
                dto.Slug = Name.Replace(" ", "-").ToLower();
                dto.Description = Desc;

                db.SaveChanges();
            }
            return "ok";
        }

        // GET: Admin/shop/AddProduct
        [HttpGet]
        public ActionResult AddProduct()
        {
            ProductVM model = new ProductVM();

            using (DB db = new DB())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }
            return View(model);
        }

        [HttpPost]
        // POST: Admin/shop/AddProduct
        public ActionResult AddProduct(ProductVM model, HttpPostedFileBase file)
        {

            //check model state
            if (!ModelState.IsValid)
            {
                using (DB db = new DB())
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "ID", "Name");
                    return View(model);
                }
            }

            // Check product is unique

            using (DB db = new DB())
            {
                if (db.Categories.Any(x => x.Name == model.Name))
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "ID", "Name");
                    ModelState.AddModelError("", "That product title is taken!");
                    return View(model);
                }

            }


            //declare product id

            int Id;

            //init and save prodct dto
            using (DB db = new DB())
            {
                ProductDTO dto = new ProductDTO();
                dto.Name = model.Name;
                dto.Slug = model.Name.Replace(" ", "-").ToLower();
                dto.Description = model.Description;
                dto.Price = model.Price;
                dto.CategoryId = model.CategoryId;

                CategoriesDTO catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);

                dto.CategoryName = catDTO.Name;

                db.Products.Add(dto);
                db.SaveChanges();

                Id = dto.id;

            }
            TempData["SM"] = "You have added a product!";


            #region Upload Image

            // Create a necessary directory

            var originalDirectory = new DirectoryInfo(string.Format("{0}\\Images\\Uploads", Server.MapPath(@"\")));

            var pathString1 = Path.Combine(originalDirectory.ToString(), "Products");
            var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + Id.ToString());
            var pathString3 = Path.Combine(originalDirectory.ToString(), "Products\\" + Id.ToString() + "\\Thumbs");
            var pathString4 = Path.Combine(originalDirectory.ToString(), "Products\\" + Id.ToString() + "\\Gallery");
            var pathString5 = Path.Combine(originalDirectory.ToString(), "Products\\" + Id.ToString() + "\\Gallery\\Thumbs");


            if (!Directory.Exists(pathString1))
                Directory.CreateDirectory(pathString1);

            if (!Directory.Exists(pathString2))
                Directory.CreateDirectory(pathString2);


            if (!Directory.Exists(pathString3))
                Directory.CreateDirectory(pathString3);


            if (!Directory.Exists(pathString4))
                Directory.CreateDirectory(pathString4);


            if (!Directory.Exists(pathString5))
                Directory.CreateDirectory(pathString5);


            //check  if a file was uploaded

            if (file != null && file.ContentLength > 0)
            {


                //Get file extention
                var ext = file.ContentType.ToLower();

                //verify the file ectention

                if (ext != "image/jpg" &&
                        ext != "image/jpeg" &&
                        ext != "image/pjpeg" &&
                        ext != "image/gif" &&
                        ext != "image/x-png" &&
                        ext != "image/png")
                {
                    using (DB db = new DB())
                    {
                        model.Categories = new SelectList(db.Categories.ToList(), "ID", "Name");
                        ModelState.AddModelError("", "The image was not uploaded - wrong image extention!");
                        return View(model);

                    }
                }

                //initlize image name

                string imageName = file.FileName;

                //save image name to DTO

                using (DB db = new DB())
                {
                    ProductDTO dto = db.Products.Find(Id);
                    dto.ImageName = imageName;
                    db.SaveChanges();
                }

                //set original and thumb image path
                var path = string.Format("{0}\\{1}", pathString2, imageName);
                var path2 = string.Format("{0}\\{1}", pathString3, imageName);

                //save original image
                file.SaveAs(path);

                //create and save the thumb
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);

            }



            #endregion

            return RedirectToAction("AddProduct");
        }

        public ActionResult Products(int? page, int? catId)
        {

            List<ProductVM> lislofProductVM;

            var pageNumber = page ?? 1;

            using (DB db = new DB())
            {
                lislofProductVM = db.Products.ToArray()
                                    .Where(x => catId == null || catId == 0 || x.CategoryId == catId)
                                    .Select(x => new ProductVM(x))
                                    .ToList();

                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                ViewBag.SelectedCat = catId.ToString();
            }

            var onePageOfProducts = lislofProductVM.ToPagedList(pageNumber, 3);

            ViewBag.OnePageOfProducts = onePageOfProducts;
            return View(lislofProductVM);
        }


        // GET: Admin/shop/EditProduct
        [HttpGet]
        public ActionResult EditProduct(int id)
        {

            ProductVM model;

            using (DB db = new DB())
            {

                ProductDTO dto = db.Products.Find(id);

                if (dto == null)
                {
                    return Content("That product does not exist!");
                }

                model = new ProductVM(dto);

                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                                                                    .Select(fn => Path.GetFileName(fn));

            }
            return View(model);
        }

        // POST: Admin/shop/EditProduct
        [HttpPost]
        public ActionResult EditProduct(ProductVM model, HttpPostedFileBase file)
        {
            int id = model.id;

            using (DB db = new DB())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

            }

            model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                                                                   .Select(fn => Path.GetFileName(fn));


            if (!ModelState.IsValid)
            {
                return View(model);
            }


            using (DB db = new DB())
            {
                if (db.Products.Where(x => x.id != id).Any(x => x.Name == model.Name))
                {
                    ModelState.AddModelError("", "That title is taken!");

                    return View(model);
                }
            }

            using (DB db = new DB())
            {

                ProductDTO dto = db.Products.Find(id);
                dto.Name = model.Name;
                dto.Slug = model.Name.Replace(" ", "-").ToLower();
                dto.Price = model.Price;
                dto.Description = model.Description;
                dto.ImageName = model.ImageName == null ? dto.ImageName : model.ImageName;
                dto.CategoryId = model.CategoryId;
                dto.CategoryName = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId).Name;
                db.SaveChanges();

            }

            TempData["SM"] = "You have edited the product!";
            #region image upload

            if (file != null && file.ContentLength > 0)
            {
                string ext = file.ContentType.ToLower();

                if (ext != "image/jpg" &&
                        ext != "image/jpeg" &&
                        ext != "image/pjpeg" &&
                        ext != "image/gif" &&
                        ext != "image/x-png" &&
                        ext != "image/png")
                {
                    using (DB db = new DB())
                    {
                        ModelState.AddModelError("", "The image was not uploaded - wrong image extention!");
                        return View(model);

                    }
                }
                var originalDirectory = new DirectoryInfo(string.Format("{0}\\Images\\Uploads", Server.MapPath(@"\")));

                var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
                var pathString3 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");


                DirectoryInfo di1 = new DirectoryInfo(pathString2);
                DirectoryInfo di2 = new DirectoryInfo(pathString3);

                foreach (FileInfo f in di1.GetFiles())
                {
                    f.Delete();
                }
                foreach (FileInfo f in di2.GetFiles())
                {
                    f.Delete();
                }
                string imageName = file.FileName;
                using (DB db = new DB())
                {
                    ProductDTO dto = db.Products.Find(id);
                    dto.ImageName = imageName;
                    db.SaveChanges();
                }
                var path = string.Format("{0}\\{1}", pathString2, imageName);
                var path2 = string.Format("{0}\\{1}", pathString3, imageName);

                //save original image
                file.SaveAs(path);

                //create and save the thumb
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);

            }


            #endregion

            return RedirectToAction("EditProduct");
        }


        public ActionResult DeleteProduct(int id)
        {
            using (DB db = new DB())
            {
                ProductDTO dto = db.Products.Find(id);

                db.Products.Remove(dto);

                db.SaveChanges();

                var originalDirectory = new DirectoryInfo(string.Format("{0}\\Images\\Uploads", Server.MapPath(@"\")));

                var pathString = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());

                if (Directory.Exists(pathString))
                    Directory.Delete(pathString, true);



            }
            return RedirectToAction("Products");
        }
    }
}