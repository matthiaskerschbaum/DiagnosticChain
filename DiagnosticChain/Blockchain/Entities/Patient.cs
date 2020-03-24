using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain.Entities
{
    internal class Patient
    {
        public Guid Address { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string Birthyear { get; set; }

        public Patient Clone()
        {
            return new Patient()
            {
                Address = Address
                ,
                Country = Country
                ,
                Region = Region
                ,
                Birthyear = Birthyear
            };
        }
    }
}
