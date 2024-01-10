namespace Food_Sharing_Food.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Foods
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Foods()
        {
            Comments = new HashSet<Comments>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(128)]
        public string FoodsId { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Tên Món Ăn")]
        public string Name { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Địa Chỉ")]
        public string Address { get; set; }

        [Required]
        [Display(Name = "Giá Tiền")]
        public int Price { get; set; }

        [Required]
        [Display(Name = "Loại Thức Ăn")]
        public int TypeFoodsId { get; set; }

        [StringLength(255)]
        public string ImageUrl { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comments> Comments { get; set; }

        public virtual TypeFoods TypeFoods { get; set; }


        public List<TypeFoods> ListTypeFoods = new List<TypeFoods>();

        [NotMapped]
        public Comments NewComment { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetails> OrderDetails { get; set; }


    }
}
