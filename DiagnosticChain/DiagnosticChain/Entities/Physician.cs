using System;
using System.Collections.Generic;
using System.Text;

namespace DiagnosticChain.Entities
{
    class Physician
    {
        public Guid PhysicianAddress { get; set; }
        public string PublicKey { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string Name { get; set; }
        public int Upvotes { get; set; }
    }
}
