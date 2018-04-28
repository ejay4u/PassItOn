using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PassItOn.Models
{
    public class AdInfo
    {
        public AdInfo()
        {
            AdCountry = new List<string>(); 
        }

        [BsonId]
        public ObjectId Id { get; set; }
        public string CampaignId { get; set; }
        public List<string> AdCountry { get; set; }
        public string AdType { get; set; }
        public BsonDocument AdMedia { get; set; }
        public bool AdStatus { get; set; }
        public string CreatedBy { get; set; }
        public DateTime TimeCreated { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime TimeUpdated { get; set; }
    }
}