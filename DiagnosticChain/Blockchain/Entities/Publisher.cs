using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Blockchain.Entities
{
    public class Publisher
    {
        public Guid Address { get; set; }
        public RSAParameters PublicKey { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string EntityName { get; set; }

        public Publisher Clone()
        {
            return new Publisher()
            {
                Address = Address
                ,
                PublicKey = PublicKey
                ,
                Country = Country
                ,
                Region = Region
                ,
                EntityName = EntityName
            };
        }
    }
}
