﻿using System;
using System.Collections.Generic;

namespace IGUPatients.Models
{
    public partial class Treatment
    {
        public Treatment()
        {
            PatientTreatment = new HashSet<PatientTreatment>();
            TreatmentMedication = new HashSet<TreatmentMedication>();
        }

        public int TreatmentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DiagnosisId { get; set; }

        public Diagnosis Diagnosis { get; set; }
        public ICollection<PatientTreatment> PatientTreatment { get; set; }
        public ICollection<TreatmentMedication> TreatmentMedication { get; set; }
    }
}
