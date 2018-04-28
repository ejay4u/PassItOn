using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PassItOn.Models
{
    public class CampaignCode
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string CampaignId { get; set; }
        public string Code { get; set; }
        public int Usage { get; set; }
        public bool CodeStatus { get; set; }
        public bool Printed { get; set; }
        public string CreatedBy { get; set; }
        public DateTime TimeCreated { get; set; }
        public DateTime TimeUpdated { get; set; }
    }
}