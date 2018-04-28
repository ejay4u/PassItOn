using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using PassItOn.Models;

namespace PassItOn
{
    public class MongoContext
    {
        private readonly IMongoDatabase _database;
        public MongoContext()        //constructor   
        {
            // Reading credentials from Web.config file   
            var mongoDatabaseName = ConfigurationManager.AppSettings["MongoDatabaseName"]; //CarDatabase  
            var mongoUsername = ConfigurationManager.AppSettings["MongoUsername"]; //demouser  
            var mongoPassword = ConfigurationManager.AppSettings["MongoPassword"]; //Pass@123  
            var mongoPort = ConfigurationManager.AppSettings["MongoPort"];  //27017  
            var mongoHost = ConfigurationManager.AppSettings["MongoHost"];  //localhost  

            // Creating credentials  
            var credential = MongoCredential.CreateMongoCRCredential
                            (mongoDatabaseName,
                             mongoUsername,
                             mongoPassword);

            // Creating MongoClientSettings  
            var settings = new MongoClientSettings
            {
                Credentials = new[] { credential },
                Server = new MongoServerAddress(mongoHost, Convert.ToInt32(mongoPort))
            };
             var client = new MongoClient(settings);
            _database = client.GetDatabase("PassItOn");
        }

        public IMongoCollection<Business> Businesses
        {
            get
            {
                return _database.GetCollection<Business>("Business");
            }
        }

        public IMongoCollection<Campaign> Campaigns
        {
            get
            {
                return _database.GetCollection<Campaign>("Campaign");
            }
        }

        public IMongoCollection<CampaignCode> CampaignCodes
        {
            get
            {
                return _database.GetCollection<CampaignCode>("CampaignCode");
            }
        }

        public IMongoCollection<Models.PassItOn> PassItOns
        {
            get
            {
                return _database.GetCollection<Models.PassItOn>("PassItOn");
            }
        }

        public IMongoCollection<AdInfo> AdInfos
        {
            get
            {
                return _database.GetCollection<AdInfo>("AdInfo");
            }
        }

        public IMongoCollection<CodeCard> CodeCard
        {
            get
            {
                return _database.GetCollection<CodeCard>("CodeCard");
            }
        }

        public IMongoCollection<CodeFailure> CodeFailure
        {
            get
            {
                return _database.GetCollection<CodeFailure>("CodeFailure");
            }
        }
    }
}