namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Programari")]
    public partial class Programari
    {
        public int Id { get; set; }

        public int UtilizatorId { get; set; }

        public int Serviciu { get; set; }

        public DateTime DataProgramarii { get; set; }

        public virtual Servicii Servicii { get; set; }

        public virtual Utilizator Utilizator { get; set; }
    }
}
