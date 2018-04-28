using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PassItOn.ViewModels;

namespace PassItOn.Models
{
    public class Business
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("AccountId")]
        public string AccountId { get; set; }

        [BsonRequired]
        [BsonElement("BusinessName")]
        public string BusinessName { get; set; }

        [BsonElement("Address")]
        public string Address { get; set; }

        [BsonElement("Telephone")]
        public string Telephone { get; set; }

        [BsonElement("Email")]
        public string Email { get; set; }

        [BsonElement("Country")]
        public string Country { get; set; }

        [BsonElement("ContactName")]
        public string ContactName { get; set; }

        [BsonElement("ContactTel")]
        public string ContactTel { get; set; }

        [BsonElement("ContactPhone")]
        public string ContactPhone { get; set; }

        [BsonElement("ContactEmail")]
        public string ContactEmail { get; set; }

        [BsonElement("Status")]
        [BsonDefaultValue(0)]
        public bool Status { get; set; }

        [BsonElement("CreatedBy")]
        public string CreatedBy { get; set; }

        [BsonElement("TimeCreated")]
        [BsonDateTimeOptions]
        public DateTime TimeCreated { get; set; }

        [BsonElement("UpdatedBy")]
        public string UpdatedBy { get; set; }

        [BsonElement("TimeUpdated")]
        public DateTime TimeUpdated { get; set; }
    }
}