namespace BookDv2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Appointment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string c_id { get; set; }

        [Required]
        [StringLength(100)]
        public string d_id { get; set; }

        public DateTime datetime { get; set; }

        public string review { get; set; }

        public string status { get; set; }
    }
}
