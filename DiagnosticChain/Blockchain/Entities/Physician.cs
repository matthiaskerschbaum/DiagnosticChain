using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Blockchain.Entities
{
    public class Physician
    {
        public Guid Address { get; set; }
        public RSAParameters PublicKey { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string Name { get; set; }

        public Physician Clone()
        {
            return new Physician()
            {
                Address = Address
                ,
                PublicKey = PublicKey
                ,
                Country = Country
                ,
                Region = Region
                ,
                Name = Name
            };
        }
    }
}
