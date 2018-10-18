namespace BookDv2.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class BookD : DbContext
    {
        public BookD ()
            : base("name=BookDData")
        {
        }


        public System.Data.Entity.DbSet<BookDv2.Models.Dietician> Dieticians { get; set; }
    }
}
