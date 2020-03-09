using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public enum TransactionType
    {
        [StringValue("TREATMENT")]
        TREATMENT = 0,
        [StringValue("SYMPTOM")]
        SYMPTOM = 1,
        [StringValue("DIAGNOSES")]
        DIAGNOSES = 2,
        [StringValue("PUBLISHER")]
        PUBLISHER = 3,
        [StringValue("PHYSICIAN")]
        PHYSICIAN = 4,
        [StringValue("PATIENT")]
        PATIENT = 5,
        [StringValue("VOTING")]
        VOTING = 6
    }
}
