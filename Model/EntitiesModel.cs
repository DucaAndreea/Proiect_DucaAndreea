using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Model
{
    public partial class EntitiesModel : DbContext
    {
        public EntitiesModel()
            : base("name=EntitiesModel")
        {
        }

        public virtual DbSet<Fidelitate> Fidelitate { get; set; }
        public virtual DbSet<Programari> Programari { get; set; }
        public virtual DbSet<Servicii> Servicii { get; set; }
        public virtual DbSet<Utilizator> Utilizator { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Servicii>()
                .Property(e => e.Pret)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Servicii>()
                .HasMany(e => e.Programari)
                .WithRequired(e => e.Servicii)
                .HasForeignKey(e => e.Serviciu);
        }
    }
}
