using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Food_Sharing_Food.Models
{
    public class CartFood
    {
        public Foods shopping_food { get; set; }
        public int shopping_quantity { get; set; }
    }
    public class Cart
    {
        List<CartFood> items = new List<CartFood>();
        public IEnumerable<CartFood> Items
        {
            get { return items; }
        }
        public void Add(Foods foodid, int quantity = 1)
        {
            var item = items.FirstOrDefault(s => s.shopping_food.Id == foodid.Id);
            if (item == null)
            {
                items.Add(new CartFood
                {
                    shopping_food = foodid,
                    shopping_quantity = quantity
                });
            }
            else
            {
                item.shopping_quantity += quantity;
            }

        }
        public void Update(int id, int quantity = 1)
        {
            var item = items.FirstOrDefault(s => s.shopping_food.Id == id);
            if (item != null)
            {
                item.shopping_quantity = quantity;
            }
        }
        public double Total()
        {
            var total = items.Sum(s => s.shopping_food.Price * s.shopping_quantity);
            return (Double)total;
        }
        public void Remove(int id)
        {
            items.RemoveAll(s => s.shopping_food.Id == id);
        }
        public int Total_quantity()
        {
            return items.Sum(s => s.shopping_quantity);
        }
    }
}