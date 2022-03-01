
   
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace acme_order.Models
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Date { get; set; }
        public string paid { get; set; }
        public string userid  { get; set; }
        public string firstname  { get; set; }
        public string  lastname { get; set; }
        public string  total  { get; set; }    
        //public string  address = Column(JSONB)
        public string  email  { get; set; }
        public string  delivery  { get; set; }
        // card=Column(JSONB)
        
        // cart=Column(JSONB)

    }
}


