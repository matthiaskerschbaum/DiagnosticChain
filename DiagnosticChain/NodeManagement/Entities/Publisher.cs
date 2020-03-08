using System;

namespace NodeManagement.Entities
{
    public class Publisher
    {
        public Guid PublisherAddress { get; set; }
        public Uri Location { get; set; }
        public string PublicKey { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string EntityName { get; set; }
        public string Upvotes { get; set; }
    }
}
