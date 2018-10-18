namespace BookDv2.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class BookD : DbContext
    {
        public BookD()
            : base("name=BookD")
        {
        }

        public virtual DbSet<Dietician> Dieticians { get; set; }

        public System.Data.Entity.DbSet<BookDv2.Models.Appointment> Appointments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dietician>()
            .Property(e => e.id)
            .HasDatabaseGeneratedOption(null);

            modelBuilder.Entity<Dietician>()
                .Property(e => e.qualification)
                .IsFixedLength();

            modelBuilder.Entity<Dietician>()
                .Property(e => e.address)
                .IsFixedLength();

            modelBuilder.Entity<Dietician>()
                .Property(e => e.latitude)
                .HasPrecision(18, 10);

            modelBuilder.Entity<Dietician>()
                .Property(e => e.longitude)
                .HasPrecision(18, 10);

            modelBuilder.Entity<Dietician>()
                .Property(e => e.contact)
                .IsFixedLength();

            modelBuilder.Entity<Dietician>()
                .Property(e => e.number_of_patients)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Dietician>()
                .Property(e => e.d_id)
                .IsFixedLength();

            modelBuilder.Entity<Appointment>()
                .Property(e => e.datetime)
                .HasColumnType("datetime2");
        }

        
    }
}
