﻿using System;
using System.Collections.Generic;

namespace IGUPatients.Models
{
    public partial class TreatmentMedication
    {
        public int TreatmentId { get; set; }
        public string Din { get; set; }

        public Medication DinNavigation { get; set; }
        public Treatment Treatment { get; set; }
    }
}
