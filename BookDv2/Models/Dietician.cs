namespace BookDv2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Dietician
    {
        [Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [StringLength(500)]
        public string qualification { get; set; }

        [StringLength(500)]
        public string address { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? latitude { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? longitude { get; set; }

        [StringLength(500)]
        public string contact { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? number_of_patients { get; set; }

        [StringLength(100)]
        public string d_id { get; set; }
    }
}
