using System;
using MongoDB.Bson;

namespace PassItOn.Models
{
    public class Ad
    {
        public ObjectId Id { get; set; }
        public string CampaignId { get; set; }
        public string AdCountry { get; set; }
        public string AdType { get; set; }
        public bool AdStatus { get; set; }
        public string CreatedBy { get; set; }
        public DateTime TimeCreated { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime TimeUpdated { get; set; }
    }
}