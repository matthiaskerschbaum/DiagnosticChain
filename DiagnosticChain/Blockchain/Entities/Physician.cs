﻿using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Blockchain.Entities
{
    public class Physician : WebSerializable
    {
        public Guid Address { get; set; }
        public RSAParameters PublicKey { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string PhysicianIdentifier { get; set; }

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
                PhysicianIdentifier = PhysicianIdentifier
            };
        }
    }
}
