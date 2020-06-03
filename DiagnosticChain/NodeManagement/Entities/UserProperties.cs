using System;
using System.Collections.Generic;
using System.Text;
using static Shared.EncryptionHandler;

namespace NodeManagement.Entities
{
    public class UserProperties
    {
        public string Username { get; set; }
        public Guid UserAddress { get; set; }
        public KeyPair Keys { get; set; }
    }
}
