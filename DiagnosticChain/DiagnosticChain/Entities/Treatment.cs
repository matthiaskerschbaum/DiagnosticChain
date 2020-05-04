using System;
using System.Collections.Generic;
using System.Text;

namespace DiagnosticChain.Entities
{
    class Treatment
    {
        public Guid TreatmentAddress { get; set; }
        public List<string> Symptoms { get; set; }
        public List<string> Diagnoses { get; set; }

        public void AddSymptom(string symptom)
        {
            if (!Symptoms.Contains(symptom))
            {
                Symptoms.Add(symptom);
            }
        }

        public void AddDiagnoses(string diagnoses)
        {
            if (!Diagnoses.Contains(diagnoses))
            {
                Diagnoses.Add(diagnoses);
            }
        }
    }
}
