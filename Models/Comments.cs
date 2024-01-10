namespace Food_Sharing_Food.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Comments
    {
        public int Id { get; set; }

        public string FullName { get; set; }
        public int FoodId { get; set; }

        public int Rating { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual Foods Foods { get; set; }
    }
}