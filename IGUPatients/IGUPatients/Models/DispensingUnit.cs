﻿using System;
using System.Collections.Generic;

namespace IGUPatients.Models
{
    public partial class DispensingUnit
    {
        public DispensingUnit()
        {
            Medication = new HashSet<Medication>();
        }

        public string DispensingCode { get; set; }

        public ICollection<Medication> Medication { get; set; }
    }
}
