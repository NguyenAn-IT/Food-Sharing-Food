using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Food_Sharing_Food.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PagedList;

namespace Food_Sharing_Food.Controllers
{
    public class FoodController : Controller
    {
        public ActionResult Food(string Searching, string Search, int? page)
        {
            ModelFoods context = new ModelFoods();

            // Lấy danh sách các món ăn dựa trên tham số tìm kiếm
            var foods = context.Fooding.ToList();
            if (Searching == "Name")
            {
                foods = context.Fooding.Where(c => c.Name.StartsWith(Search)).ToList();
            }
            else if (Searching == "Address")
            {
                foods = context.Fooding.Where(c => c.Address.StartsWith(Search)).ToList();
            }

            // Đặt kích thước trang là 10, bạn có thể thay đổi nó nếu cần
            int pageSize = 6;
            int pageNumber = (page ?? 1);

            // Sử dụng phân trang để lấy danh sách các món ăn theo trang
            var pagedFoods = foods.ToPagedList(pageNumber, pageSize);

            // Truyền danh sách các món ăn đã phân trang vào view
            return View(pagedFoods);
        }


        public ActionResult Sort(string sortOrder, string sortBy)
        {
            ModelFoods context = new ModelFoods();

            ViewBag.SortOrder = sortOrder;
            var price = context.Fooding.ToList();
            switch (sortOrder)
            {
                case "Asc":
                    {
                        price = price.OrderBy(x => x.Price).ToList();
                        break;
                    }
                case "Desc":
                    {
                        price = price.OrderByDescending(x => x.Price).ToList();
                        break;
                    }
                default:
                    {
                        price = price.OrderBy(x => x.Price).ToList();
                        break;
                    }
            }
            return View(price);

        }
       
        public ActionResult Create()
        {
            using (var context = new ModelFoods())
            {
                Foods objCourse = new Foods();

                objCourse.ListTypeFoods = context.TypeFoods != null ? context.TypeFoods.ToList() : new List<TypeFoods>();

                ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()?.FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());

                if (user != null)
                {
                    objCourse.FoodsId = user.Id;
                }
                else
                {
                    return HttpNotFound();
                }

                return View(objCourse);
            }
        }
       
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Foods newFoods)
        {
            ModelFoods context = new ModelFoods();

            ModelState.Remove("FoodsId");
            if (!ModelState.IsValid)
            {
                newFoods.ListTypeFoods = context.TypeFoods.ToList();
                return View("Create", newFoods);
            }

            context.Fooding.Add(newFoods);
            context.SaveChanges();

            return RedirectToAction("Food", "Food");

        }
     
        public ActionResult Edit(int id)
        {
            using (var context = new ModelFoods())
            {
                var course = context.Fooding.SingleOrDefault(c => c.Id == id);
                if (course == null)
                {
                    return HttpNotFound();
                }
                course.ListTypeFoods = context.TypeFoods.ToList();
                return View("Edit", course);
            }
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, Foods food)
        {

            using (var context = new ModelFoods())
            {
                var loginUser = User.Identity.GetUserId();
                if (!ModelState.IsValid)
                {
                    food.ListTypeFoods = context.TypeFoods.ToList();
                    return View("Create", food);
                }
                var Foods = context.Fooding.SingleOrDefault(c => c.Id == food.Id && c.FoodsId == loginUser);
                if (Foods == null)
                {
                    return HttpNotFound();
                }
                Foods.Name = food.Name;
                Foods.Address = food.Address;
                Foods.Price = food.Price;
                Foods.TypeFoodsId = food.TypeFoodsId;

                context.SaveChanges();
            }
            return RedirectToAction("Food", "Food");
        }

     
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (ModelFoods context = new ModelFoods())
            {
                Foods food = context.Fooding.Find(id);
                if (food == null)
                {
                    return HttpNotFound();
                }
                context.Fooding.Remove(food);
                context.SaveChanges();
            }

            return RedirectToAction("Food", "Food");
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            using (var context = new ModelFoods())
            {
                var food = context.Fooding.SingleOrDefault(c => c.Id == id);
                if (food == null)
                {
                    return HttpNotFound();
                }
                context.Fooding.Remove(food);
                context.SaveChanges();
            }
            return RedirectToAction("Index", "Home");
        }


        public ActionResult Details(int? id)
        {
            using (var context = new ModelFoods())
            {
                if (id == null)
                {
                    return HttpNotFound();
                }

                Foods food = context.Fooding.Include(f => f.Comments).SingleOrDefault(f => f.Id == id);

                if (food == null)
                {
                    return HttpNotFound();
                }

                var typeFoodsList = context.TypeFoods.ToList(); // Truy vấn danh sách TypeFoods và lưu trữ trong List

                ViewBag.TypeFoodsList = new SelectList(typeFoodsList, "Id", "Name");

                // Lấy danh sách bình luận cho món ăn
                var comments = context.Comments.Where(c => c.FoodId == id).ToList();
                ViewBag.Comments = comments;
                ViewBag.NewComment = new Comments();


                var ratings = food.Comments.Select(c => c.Rating).ToList();
                int totalRating = ratings.Sum();
                int commentCount = ratings.Count;
                double averageRating = (double)totalRating / commentCount;

                return View(food);
            }
        }
        [HttpGet]
        public ActionResult AddComment(int id)
        {
            ModelFoods context = new ModelFoods();
            var food = context.Fooding.Find(id);

            if (food != null)
            {
                var model = new Comments { FoodId = food.Id };

                // Lấy danh sách Food và chuyển vào ViewData
                var foodList = context.Fooding.ToList();
                ViewData["FoodList"] = new SelectList(foodList, "Id", "Name");

                return View(model);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddComment(Comments model)
        {
            ModelFoods context = new ModelFoods();

            if (ModelState.IsValid)
            {
                var food = context.Fooding.SingleOrDefault(m => m.Id == model.FoodId);

                if (food != null)
                {
                    var newComment = new Comments
                    {
                        FullName = model.FullName,
                        Content = model.Content,
                        Rating = model.Rating,
                        CreatedAt = DateTime.Now
                    };

                    food.Comments.Add(newComment);
                    context.SaveChanges();

                    // Cập nhật lại đối tượng Food với danh sách bình luận mới nhất
                    food = context.Fooding
                        .Include(f => f.Comments)
                        .FirstOrDefault(f => f.Id == model.FoodId);

                    // Gửi lại dữ liệu chi tiết món ăn sau khi thêm bình luận thành công
                    return View("Details", food);
                }
                else
                {
                    // Xử lý khi không tìm thấy món ăn
                    return HttpNotFound(); // Hoặc trả về một trang lỗi khác tùy thuộc vào yêu cầu của bạn
                }
            }

            // Trả về View khi ModelState không hợp lệ
            return View("Details", model);
        }

        public ActionResult FoodLike1()
        {
            return View();

        }
        public ActionResult FoodLike2()
        {
            return View();
        }

        public ActionResult FoodLike3()
        {
            return View();
        }

    }

}