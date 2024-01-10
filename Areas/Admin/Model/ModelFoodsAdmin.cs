using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Food_Sharing_Food.Areas.Admin.Model
{
    public partial class ModelFoodsAdmin : DbContext
    {
        public ModelFoodsAdmin()
            : base("name=ModelFoodsAdmin")
        {
        }

        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<Comments> Comments { get; set; }
        public virtual DbSet<FeedBack> FeedBack { get; set; }
        public virtual DbSet<Foods> Foods { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<OrderDetails> OrderDetails { get; set; }
        public virtual DbSet<TypeFoods> TypeFoods { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.AspNetUserRoles)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.Foods)
                .WithRequired(e => e.AspNetUsers)
                .WillCascadeOnDelete(false);

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

            modelBuilder.Entity<TypeFoods>()
                .HasMany(e => e.Foods)
                .WithRequired(e => e.TypeFoods)
                .WillCascadeOnDelete(false);
        }
    }
}
