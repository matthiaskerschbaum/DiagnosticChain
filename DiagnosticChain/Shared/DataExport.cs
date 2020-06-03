using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shared
{
    public class DataExport
    {
        public Guid PatientAddress { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string Birthyear { get; set; }
        public Guid TreatmentAddress { get; set; }
        public DateTime TreatmentBeginDate { get; set; }
        public List<string> Symptoms { get; set; }
        public List<string> Diagnoses { get; set; }

        public string ForExport(string delimiter)
        {
            return
                PatientAddress.ToString() + delimiter +
                Country + delimiter +
                Region + delimiter +
                Birthyear + delimiter +
                TreatmentAddress + delimiter +
                TreatmentBeginDate.ToString("yyyy-MM-dd hh:mm:ss") + delimiter +
                Symptoms.Aggregate("", (x, y) => x + y + "|") + delimiter +
                Diagnoses.Aggregate("", (x, y) => x + y + "|");
        }
    }
}
