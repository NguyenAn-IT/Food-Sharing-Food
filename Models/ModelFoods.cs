using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Food_Sharing_Food.Models
{
    public partial class ModelFoods : DbContext
    {
        public ModelFoods()
            : base("name=ModelFoods")
        {
        }

        public virtual DbSet<Comments> Comments { get; set; }
        public virtual DbSet<FeedBack> FeedBacks { get; set; }
        public virtual DbSet<Foods> Fooding { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<OrderDetails> OrderDetails { get; set; }
        public virtual DbSet<TypeFoods> TypeFoods { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Foods>()
                .Property(e => e.ImageUrl)
                .IsUnicode(false);

            modelBuilder.Entity<Foods>()
                .HasMany(e => e.Comments)
                .WithRequired(e => e.Foods)
                .HasForeignKey(e => e.FoodId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Foods>()
                .HasMany(e => e.OrderDetails)
                .WithRequired(e => e.Foods)
                .HasForeignKey(e => e.FoodID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Order>()
                .Property(e => e.Status)
                .IsFixedLength();

            modelBuilder.Entity<Order>()
                .HasMany(e => e.OrderDetails)
                .WithRequired(e => e.Order)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<OrderDetails>()
                .Property(e => e.Price)
                .HasPrecision(19, 4);

            modelBuilder.Entity<TypeFoods>()
                .HasMany(e => e.Foods)
                .WithRequired(e => e.TypeFoods)
                .WillCascadeOnDelete(false);
        }
    }
}
