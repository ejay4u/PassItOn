using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PassItOn.Models
{
    public class PassItOn
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string PassitCode { get; set; }
        public string MobileNo { get; set; }
        public string CampaignId { get; set; }
        public string RecipientName { get; set; }
        public string ServiceCode { get; set; }
        public string Amount { get; set; }
        public string Reference { get; set; }
        public string Message { get; set; }
        public bool Status { get; set; }
        public DateTime TimeCreated { get; set; }
        public DateTime TimeUpdated { get; set; }
    }
}