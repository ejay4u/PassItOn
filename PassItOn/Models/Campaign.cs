using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PassItOn.Models
{
    public class Campaign
    {
        public Campaign()
        {
            CampaignCountry = new List<string>();
            CampaignNetwork = new List<string>();
        }

        [BsonId]
        public ObjectId Id { get; set; }
        public string AccountId { get; set; }
        public string CampaignTitle { get; set; }
        public List<string> CampaignCountry { get; set; }
        public List<string> CampaignNetwork { get; set; }
        public int CampaignCodeQty { get; set; }
        public int UsageLimit { get; set; }
        public bool CampaignStatus { get; set; }
        public string CampaignPrize { get; set; }
        public BsonDocument AdMedia { get; set; }
        public string CreatedBy { get; set; }
        public DateTime TimeCreated { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime TimeUpdated { get; set; }
    }
}