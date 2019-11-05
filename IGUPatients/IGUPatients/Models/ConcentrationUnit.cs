using System;
using System.Collections.Generic;

namespace IGUPatients.Models
{
    public partial class ConcentrationUnit
    {
        public ConcentrationUnit()
        {
            Medication = new HashSet<Medication>();
        }

        public string ConcentrationCode { get; set; }

        public ICollection<Medication> Medication { get; set; }
    }
}
