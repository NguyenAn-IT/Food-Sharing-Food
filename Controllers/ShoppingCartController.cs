using Food_Sharing_Food.Models;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Food_Sharing_Food.Controllers
{
    public class ShoppingCartController : Controller
    {

        public Cart GetCart()
        {
            Cart cart = Session["Cart"] as Cart;
            if (cart == null || Session["Cart"] == null)
            {
                cart = new Cart();
                Session["Cart"] = cart;
            }

            return cart;
        }

        public ActionResult AddtoCart(int id)
        {
            ModelFoods context = new ModelFoods();
            var food = context.Fooding.SingleOrDefault(f => f.Id == id);
            if (food != null)
            {
                GetCart().Add(food);
            }
            return RedirectToAction("ShowtoCart", "ShoppingCart");
        }
        public ActionResult ShowtoCart()
        {
            if (Session["Cart"] == null)
            {
                return RedirectToAction("ShowtoCart", "ShoppingCart");
            }
            Cart cart = Session["Cart"] as Cart;
            return View(cart);
        }

        public ActionResult updateCart(FormCollection form)
        {
            Cart cart = Session["Cart"] as Cart;
            int id_food = int.Parse(form["FoodID"]);
            int quantity_food = int.Parse(form["Quantity"]);
            cart.Update(id_food, quantity_food);
            return RedirectToAction("ShowtoCart", "ShoppingCart");
        }

        public ActionResult removeCart(int id)
        {
            Cart cart = Session["Cart"] as Cart;
            cart.Remove(id);
            return RedirectToAction("ShowtoCart", "ShoppingCart");
        }

        public PartialViewResult BagCart()
        {
            int item = 0;
            Cart cart = Session["Cart"] as Cart;
            if (cart != null)
            {
                item = cart.Total_quantity();

            }
            ViewBag.infoCart = item;
            return PartialView("BagCart");
        }

        [HttpGet]
        public ActionResult Payment()
        {
            if (Session["Cart"] == null)
            {
                return RedirectToAction("ShowtoCart", "ShoppingCart");
            }
            Cart cart = Session["Cart"] as Cart;
            var list = new List<Cart>();
            if (cart != null)
            {
                list.Add(cart);
            }
            return View(list.FirstOrDefault());
        }
        [HttpPost]
        public ActionResult Payment(string shipName, string mobile, string address, string email, Gmail gmail)
        {

            ModelFoods context = new ModelFoods();
            var order = new Models.Order();
            order.CreateDate = DateTime.Now;
            order.ShipAddress = address;
            order.ShipMoblie = mobile;
            order.ShipName = shipName;

            try
            {
                // Get the cart
                Cart cart = Session["Cart"] as Cart;


                string htmlTable = "<table border='1'><tr> <th>Tên Món Ăn</th> <th>Địa chỉ</th> <th>Giá</th> <th>Số lượng</th> </tr>";

                foreach (var item in cart.Items)
                {
                    htmlTable += $"<tr><td>{item.shopping_food.Name}</td><td>{item.shopping_food.Address}</td><td>{item.shopping_food.Price}</td><td>{item.shopping_quantity}</td></tr>";
                }

                htmlTable += $"</table>";


                string htmlContent = System.IO.File.ReadAllText(Server.MapPath("~/Content/template/newOrder.html"));

                htmlContent = htmlContent.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
                htmlContent = htmlContent.Replace("{{CustomerName}}", shipName);
                htmlContent = htmlContent.Replace("{{Phone}}", mobile);
                htmlContent = htmlContent.Replace("{{Address}}", address);
                htmlContent = htmlContent.Replace("{{Total}}", cart.Total().ToString("N0")); 
                htmlContent = htmlContent.Replace("{{OrderDetails}}", htmlTable); 

                // Set the body of the email
                gmail.Body = htmlContent;

                // Send email
                gmail.SendMail();

                return RedirectToAction("Success");
            }
            catch (Exception ex)
            {
                return RedirectToAction("UnSuccess");
            }
        }


        public ActionResult PaymentWithPaypal(string Cancel = null)
        {
            //getting the apiContext  
            APIContext apiContext = PaypalConfiguration.GetAPIContext();
            try
            {
                //A resource representing a Payer that funds a payment Payment Method as paypal  
                //Payer Id will be returned when payment proceeds or click to pay  
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    //this section will be executed first because PayerID doesn't exist  
                    //it is returned by the create function call of the payment class  
                    // Creating a payment  
                    // baseURL is the url on which paypal sendsback the data.  
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/shoppingcart/PaymentWithPayPal?";
                    //here we are generating guid for storing the paymentID received in session  
                    //which will be used in the payment execution  
                    var guid = Convert.ToString((new Random()).Next(100000));
                    //CreatePayment function gives us the payment approval url  
                    //on which payer is redirected for paypal account payment  
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);
                    //get links returned from paypal in response to Create function call  
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment  
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    // saving the paymentID in the key guid  
                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // This function exectues after receving all parameters for the payment  
                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
                    //If executed payment failed then we will show payment failure message to user  
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("UnSuccess");
                    }
                }
            }
            catch (Exception ex)
            {
                return View("UnSuccess");
            }
            //on successful payment, show success page to user.  
            return View("Success");
        }
        private PayPal.Api.Payment payment;
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }
        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {

            var ListSP = Session["Cart"] as Cart;
            //create itemlist and add item objects to it  
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };

            foreach (var item in ListSP.Items)
            {
                itemList.items.Add(new Item()
                {
                    name = item.shopping_food.Name,
                    currency = "USD",
                    price = item.shopping_food.Price.ToString(),
                    quantity = item.shopping_quantity.ToString(),
                    sku = item.shopping_food.Id.ToString()
                });
            }
            //Adding Item Details like name, currency, price etc  
           
            var payer = new Payer()
            {
                payment_method = "paypal"
            };
            // Configure Redirect Urls here with RedirectUrls object  
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };
            // Adding Tax, shipping and Subtotal details  
            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = ListSP.Total().ToString()
            };
            //Final amount with details  
            var amount = new Amount()
            {
                currency = "USD",
                total = ListSP.Total().ToString(), // Total must be equal to sum of tax, shipping and subtotal.  
                details = details
            };
            var transactionList = new List<Transaction>();
            // Adding description about the transaction  
            var paypalOrderId = DateTime.Now.Ticks;
            transactionList.Add(new Transaction()
            {
                description = $"Invoice #{paypalOrderId}",
                invoice_number = paypalOrderId.ToString(), //Generate an Invoice No    
                amount = amount,
                item_list = itemList
            });
            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            // Create a payment using a APIContext  
            return this.payment.Create(apiContext);
        }


        public ActionResult Success()
        {
            return View();
        }
        public ActionResult UnSuccess()
        {
            return View();
        }
    }
}