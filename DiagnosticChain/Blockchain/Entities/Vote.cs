using System;
using System.Collections.Generic;
using System.Text;

namespace Blockchain.Entities
{
    internal class Vote
    {
        public Guid VoteFor { get; set; }
        public Guid VoteFrom { get; set; }
        public bool Confirmed { get; set; } //1=Confirmed,0=Denied

        public Vote Clone()
        {
            return new Vote()
            {
                VoteFor = VoteFor
                ,
                VoteFrom = VoteFrom
                ,
                Confirmed = Confirmed
            };
        }
    }
}
