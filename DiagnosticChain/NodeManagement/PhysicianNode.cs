using Blockchain.Entities;
using Blockchain.Interfaces;
using Grpc.Core;
using NetworkingFacilities.Clients;
using NodeManagement.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NodeManagement
{
    public class PhysicianNode : Node
    {
        public List<Blockchain.Entities.Physician> pendingPhysicians = new List<Blockchain.Entities.Physician>();

        public List<Blockchain.Entities.Physician> GetPendingPhysicians()
        {
            return pendingPhysicians;
        }

        public void UpdatePendingPhysicians()
        {
            foreach (var n in knownNodes)
            {
                try
                {
                    var physicians = new PhysicianClient(n).RequestPhysicians();

                    foreach (var p in physicians)
                    {
                        if (!pendingPhysicians.Where(x => x.Address == p.Address).Any())
                        {
                            pendingPhysicians.Add(p);
                        }
                    }

                    List<Blockchain.Entities.Physician> toRemove = new List<Blockchain.Entities.Physician>();
                    foreach (var p in pendingPhysicians)
                    {
                        if (!physicians.Where(x => x.Address == p.Address).Any())
                        {
                            toRemove.Add(p);
                        }
                    }
                    pendingPhysicians.RemoveAll(p => toRemove.Contains(p));
                } catch (RpcException) { }
            }
        }

        public bool SendTransaction(ITransaction transaction)
        {
            var success = false;

            foreach (var n in knownNodes)
            {
                try
                {
                    success |= new PhysicianClient(n).SendTransaction(transaction).Status == AckMessage.Types.Status.Ok;
                } catch (RpcException) { }

                if (success) break;
            }

            return success;
        }
    }
}
