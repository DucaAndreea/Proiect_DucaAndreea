namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Fidelitate")]
    public partial class Fidelitate
    {
        public int Id { get; set; }

        public int UtilizatorId { get; set; }

        public int NrProgramari { get; set; }

        public virtual Utilizator Utilizator { get; set; }
    }
}
