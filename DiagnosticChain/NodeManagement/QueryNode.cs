using Blockchain;
using Blockchain.Transactions;
using Blockchain.Utilities;
using Grpc.Core;
using NetworkingFacilities.Clients;
using NodeManagement.Entities;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NodeManagement
{
    public class QueryNode : Node
    {
        public bool HasBlockchainConnection()
        {
            return knownNodes.Count > 0;
        }

        public void UpdateChain()
        {
            if (!IsChainInitialized())
            {
                var initialized = false;

                foreach (var n in knownNodes)
                {
                    try
                    {
                        chain = new QueryClient(n).RequestFullChain();
                        initialized |= !chain.IsEmpty();
                    } catch (RpcException) { }

                    if (initialized) break;
                }

                if (!initialized) return;

                participantHandler = new ParticipantHandler();
                chain.ProcessContracts(participantHandler, new List<Chain>() { chain });
            }

            long currentIndex = chain.Blockhead.Index;

            foreach (var n in knownNodes)
            {
                try
                {
                    var update = new QueryClient(n).RequestDeltaChain(currentIndex);
                    if (!update.IsEmpty())
                    {
                        update.ProcessContracts(participantHandler, new List<Chain>() { chain });
                        chain.Add(update);
                        currentIndex = chain.Blockhead.Index;
                    }
                } catch (RpcException) { }
            }
        }

        public List<DataExport> ExtractData(Func<PatientRegistrationTransaction, bool> patientExp
            , Func<TreatmentTransaction, bool> treatmentExp
            , Func<SymptomsTransaction, bool> symptomExp
            , Func<DiagnosesTransaction, bool> diagnosesExp)
        {
            var transactions = chain.GetTransactions();

            var patients = from t in transactions
                           where t.GetType() == typeof(PatientRegistrationTransaction)
                           select (PatientRegistrationTransaction)t;
            patients = patients.Where(patientExp);

            var treatments = from t in transactions
                             where t.GetType() == typeof(TreatmentTransaction)
                             select (TreatmentTransaction)t;
            treatments = treatments.Where(treatmentExp);

            var symptoms = from t in transactions
                           where t.GetType() == typeof(SymptomsTransaction)
                           select (SymptomsTransaction)t;
            symptoms = symptoms.Where(symptomExp);

            var diagnoses = from t in transactions
                            where t.GetType() == typeof(DiagnosesTransaction)
                            select (DiagnosesTransaction)t;
            diagnoses = diagnoses.Where(diagnosesExp);

            var dataExport = from p in patients
                             join t in treatments on p.TransactionId equals t.PatientAddress
                             join s in symptoms on t.TransactionId equals s.TreatmentTransactionAddress
                             join d in diagnoses on t.TransactionId equals d.TreatmentTransactionAddress
                             select new DataExport()
                             {
                                 PatientAddress = p.TransactionId
                                 ,
                                 Country = p.Country
                                 ,
                                 Region = p.Region
                                 ,
                                 Birthyear = p.Birthyear
                                 ,
                                 TreatmentAddress = t.TransactionId
                                 ,
                                 TreatmentBeginDate = t.Timestamp
                                 ,
                                 Symptoms = s.Symptoms
                                 ,
                                 Diagnoses = d.Diagnoses
                             } into d
                             group d by new { d.PatientAddress, d.Country, d.Region, d.Birthyear, d.TreatmentAddress, d.TreatmentBeginDate } into dg
                             select new DataExport()
                             {
                                 PatientAddress = dg.Key.PatientAddress
                                 ,
                                 Country = dg.Key.Country
                                 ,
                                 Region = dg.Key.Region
                                 ,
                                 Birthyear = dg.Key.Birthyear
                                 ,
                                 TreatmentAddress = dg.Key.TreatmentAddress
                                 ,
                                 TreatmentBeginDate = dg.Key.TreatmentBeginDate
                                 ,
                                 Symptoms = dg.Aggregate((x, y) => { return new DataExport() { Symptoms = CombineList(x.Symptoms, y.Symptoms) }; }).Symptoms
                                 ,
                                 Diagnoses = dg.Aggregate((x, y) => { return new DataExport() { Diagnoses = CombineList(x.Diagnoses, y.Diagnoses) }; }).Diagnoses
                             };

            return dataExport.ToList(); 
        }

        private List<string> CombineList(List<string> listA, List<string> listB)
        {
            var ret = new List<string>();
            ret.AddRange(listA);
            ret.AddRange(listB);
            return ret;
        }
    }
}
