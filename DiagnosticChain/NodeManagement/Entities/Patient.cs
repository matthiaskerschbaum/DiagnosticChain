using System;
using System.Collections.Generic;
using System.Text;

namespace NodeManagement.Entities
{
    public class Patient
    {
        public Guid PatientAddress { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string Name { get; set; }

        public List<Treatment> Treatments { get; set; }
    }
}
