using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using PassItOn.ViewModels;

namespace PassItOn.Models
{
    public class DataAccess
    {
        private MongoContext _dbContext;
        public DataAccess()
        {
            _dbContext = new MongoContext();
        }

        #region CompanyTasks
        public async Task<IEnumerable<Business>> GetAllBusinesses()
        {
            try
            {
                return await _dbContext.Businesses.Find(_ => true).ToListAsync();
                //return await Mapper.Map<IEnumerable<BusinessDto>>(_dbContext.Businesses.Find(_ => true).ToListAsync()).Select(Mapper.Map<Business, BusinessDto>);
                //return await _dbContext.Businesses.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }

        }

        public async Task<Business> GetBusiness(string id)
        {
            var filter = Builders<Business>.Filter.Eq("Id", id);

            try
            {
                return await _dbContext.Businesses
                                .Find(filter)
                                .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task AddBusiness(Business business)
        {
            business.AccountId = new AccountNoGen().RandomAccountNo();
            var builder = Builders<Business>.Filter;
            var filter = builder.Eq("AccountId", business.AccountId) & builder.Eq("BusinessName", business.BusinessName);
            var query = await _dbContext.Businesses
                                .Find(filter)
                                .ToListAsync();
            try
            {
                if(query.Count <= 0)
                    await _dbContext.Businesses.InsertOneAsync(business);
                else
                {

                }

            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<bool> UpdateBusiness(ObjectId id, Business business)
        {
            try
            {
                ReplaceOneResult actionResult = await _dbContext.Businesses
                                                .ReplaceOneAsync(n => n.Id.Equals(id)
                                                                , business
                                                                , new UpdateOptions { IsUpsert = true });
                return actionResult.IsAcknowledged
                    && actionResult.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<bool> RemoveBusiness(string id)
        {
            try
            {
                //Request Business Information
                var query = _dbContext.Businesses.Find(Builders<Business>.Filter.Eq("_id", ObjectId.Parse(id))).ToEnumerable();
                if (query != null)
                {
                    var projection = Builders<Campaign>.Projection.Include("AccountId");
                    var businesses = query as IList<Business> ?? query.ToList();
                    var campaings = _dbContext.Campaigns.Find(Builders<Campaign>.Filter.Eq("AccountId", businesses.ElementAt(0).AccountId)).Project(projection).ToEnumerable();
                    if(campaings != null)
                    {
                        //Delete All CampaignCode For Campaigns By Business
                        foreach(var campaign in campaings)
                        {
                            await _dbContext.CampaignCodes.DeleteManyAsync(Builders<CampaignCode>.Filter.Eq("CampaignId", campaign.GetElement(0).Value.AsString));

                            //Delete Campaign Winners
                            await _dbContext.PassItOns.DeleteManyAsync(Builders<PassItOn>.Filter.Eq("CampaignId", campaign.GetElement(0).Value.AsString));

                            //Delete Card Generation
                            await _dbContext.CodeCard.DeleteManyAsync(Builders<CodeCard>.Filter.Eq("CampaignId", campaign.GetElement(0).Value.AsString));
                        }

                        //Delete All Campaigns By Business
                        await _dbContext.Campaigns.DeleteManyAsync(Builders<Campaign>.Filter.Eq("AccountId", businesses.ElementAt(0).AccountId));
                    }
                    
                }

                //Delete the Business
                DeleteResult actionResult = await _dbContext.Businesses.DeleteOneAsync(
                     Builders<Business>.Filter.Eq("_id", ObjectId.Parse(id)));

                return actionResult.IsAcknowledged
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<bool> RemoveAllBusinesses()
        {
            try
            {
                DeleteResult actionResult = await _dbContext.Businesses.DeleteManyAsync(new BsonDocument());

                return actionResult.IsAcknowledged
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        #endregion

        #region CampaignTasks
        public async Task<IEnumerable<Campaign>> GetAllCampaigns()
        {
            try
            {
                return await _dbContext.Campaigns.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }

        }

        public async Task<Campaign> GetCampaign(string id)
        {
            var filter = Builders<Campaign>.Filter.Eq("Id", id);

            try
            {
                return await _dbContext.Campaigns
                                .Find(filter)
                                .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task AddCampaign(CampaignViewModel campaignViewModel)
        {
            var campaign = campaignViewModel.Campaign;

            if (campaignViewModel.SelectedCampaignNetwork != null)
            {
                foreach (var network in campaignViewModel.SelectedCampaignNetwork)
                {
                    if (network == "MTN")
                    {
                        campaign.CampaignNetwork.Add("024");
                        campaign.CampaignNetwork.Add("054");
                        campaign.CampaignNetwork.Add("055");
                    }

                    if (network == "VODAFONE")
                    {
                        campaign.CampaignNetwork.Add("020");
                        campaign.CampaignNetwork.Add("050");
                    }

                    if (network == "TIGO")
                    {
                        campaign.CampaignNetwork.Add("027");
                        campaign.CampaignNetwork.Add("057");
                    }

                    if (network == "AIRTEL")
                    {
                        campaign.CampaignNetwork.Add("026");
                    }
                }
            }

            int codeQty = campaign.CampaignCodeQty;
            campaign.TimeCreated = DateTime.Now;
            campaign.CampaignStatus = true;
            await _dbContext.Campaigns.InsertOneAsync(campaign);
            string campagnId = campaign.Id.ToString();

            for (int i = 0; i < codeQty; i++)
            {
                var codes = new CampaignCode();
                {
                    codes.CampaignId = campagnId;
                    codes.CodeStatus = true;
                    codes.TimeCreated = DateTime.Now;
                    codes.Code = Guid.NewGuid().ToString().ToUpper().Substring(0, 11).Replace("-", string.Empty);
                };

                await _dbContext.CampaignCodes.InsertOneAsync(codes);
            }
        }

        public async Task<bool> UpdateCampaign(ObjectId id, Campaign campaign)
        {
            try
            {
                ReplaceOneResult actionResult = await _dbContext.Campaigns
                                                .ReplaceOneAsync(n => n.Id.Equals(id)
                                                                , campaign
                                                                , new UpdateOptions { IsUpsert = true });
                return actionResult.IsAcknowledged
                    && actionResult.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<bool> RemoveCampaign(string id)
        {
            try
            {
                DeleteResult actionResult = await _dbContext.Campaigns.DeleteOneAsync(
                     Builders<Campaign>.Filter.Eq("_id", ObjectId.Parse(id)));

                if (actionResult.IsAcknowledged && actionResult.DeletedCount > 0)
                {
                    //Delete Campaign Codes
                    await _dbContext.CampaignCodes.DeleteManyAsync(Builders<CampaignCode>.Filter.Eq("CampaignId", id));

                    //Delete Campaign Winners
                    await _dbContext.PassItOns.DeleteManyAsync(Builders<PassItOn>.Filter.Eq("CampaignId", id));

                    //Delete Card Generation
                    await _dbContext.CodeCard.DeleteManyAsync(Builders<CodeCard>.Filter.Eq("CampaignId", id));
                }

                return actionResult.IsAcknowledged
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }
        #endregion

        #region CampaignCodesTasks
        public async Task<IEnumerable<CampaignCode>> GetAllCampaignCodes()
        {
            try
            {
                return await _dbContext.CampaignCodes.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }

        }

        public async Task AddCampaignCode(CodeViewModel codeViewModel)
        {
            var builder = Builders<Campaign>.Filter;
            var filter = builder.Eq("_id", ObjectId.Parse(codeViewModel.CampaignCode.CampaignId));
            var query = _dbContext.Campaigns.Find(filter).FirstOrDefault();

            if (query != null)
            {
                try
                {
                    var update = Builders<Campaign>.Update.Set("CampaignCodeQty", query.CampaignCodeQty + codeViewModel.Campaign.CampaignCodeQty).CurrentDate("TimeUpdated");
                    await _dbContext.Campaigns.UpdateOneAsync(filter, update);

                    int codeQty = codeViewModel.Campaign.CampaignCodeQty;
                    codeViewModel.Campaign.TimeCreated = DateTime.Now;
                    codeViewModel.Campaign.CampaignStatus = true;

                    for (int i = 0; i < codeQty; i++)
                    {
                        var codes = new CampaignCode();
                        {
                            codes.CampaignId = codeViewModel.CampaignCode.CampaignId;
                            codes.CodeStatus = true;
                            codes.TimeCreated = DateTime.Now;
                            codes.Code = Guid.NewGuid().ToString().ToUpper().Substring(0, 11).Replace("-", string.Empty);
                        };

                        await _dbContext.CampaignCodes.InsertOneAsync(codes);
                    }
                }
                catch (Exception ex)
                {
                    // log or manage the exception
                    throw ex;
                }

            }
        }

        public async Task<bool> RemoveCampaignCode(string codeId)
        {
            try
            {
                DeleteResult actionResult = await _dbContext.CampaignCodes.DeleteOneAsync(Builders<CampaignCode>.Filter.Eq("_id", ObjectId.Parse(codeId)));

                return actionResult.IsAcknowledged
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        #endregion

        #region PassItOnTasks
        public async Task<Campaign> PassItOn(string passitCode, string mobileNo)
        {
            var builder = Builders<CampaignCode>.Filter;
            var filter = builder.Eq("Code", passitCode) & builder.Eq("CodeStatus", true);
            var query = await _dbContext.CampaignCodes.Find(filter).FirstOrDefaultAsync();

            if (query != null)
            {
                //Delete Any outstanding PhoneNo blocks
                await _dbContext.CodeFailure.DeleteManyAsync(Builders<CodeFailure>.Filter.And(Builders<CodeFailure>.Filter.Eq("MobileNo", mobileNo), Builders<CodeFailure>.Filter.Eq("NumberStatus", true)));

                var builder2 = Builders<Campaign>.Filter;
                var filter2 = builder2.Eq("_id", ObjectId.Parse(query.CampaignId)) & builder2.Gt("UsageLimit", query.Usage) & builder2.Eq("CampaignStatus", true);
                return await _dbContext.Campaigns.Find(filter2).FirstOrDefaultAsync();
            }

            return null;

            /*var lookup = await _dbContext.Campaigns.Aggregate().Lookup("CampaignCode", "_id", "CampaignId", "CampaignResult");
            var filter = Builders<BsonDocument>.Filter.Eq("CampaignResult", new BsonDocument { { "Code", passitCode } });
            var result = lookup.Find(filter).ToList();
            */
        }

        public async Task<string> CodeFailure(string mobileNo)
        {
            //Check If Number already is entered for invalid code entry
            var builder = Builders<CodeFailure>.Filter;
            var filter = builder.Eq("MobileNo", mobileNo) & builder.Eq("NumberStatus", true) & builder.Lt("FailureCount", 3);
            var query = await _dbContext.CodeFailure.Find(filter).FirstOrDefaultAsync();

            if (query == null)
            {
                //If no entry, Do first entry
                var codeFailure = new CodeFailure()
                {
                    MobileNo = mobileNo,
                    FailureCount = 1,
                    NumberStatus = true,
                    TimeCreated = DateTime.Now,
                    ReactivationTime = DateTime.Now
                };

                await _dbContext.CodeFailure.InsertOneAsync(codeFailure);
            }
            else
            {
                //If entry exist, do updates to entry
                if (query.FailureCount + 1 == 3)
                {
                    var update = Builders<CodeFailure>.Update.Set("FailureCount", query.FailureCount + 1).Set("NumberStatus", false).Set("ReactivationTime", DateTime.Now.AddHours(24)).Set("SuspendedFor", 24).CurrentDate("TimeUpdated");

                    UpdateResult actionResult = await _dbContext.CodeFailure.UpdateOneAsync(filter, update);

                    if (actionResult.IsAcknowledged && actionResult.ModifiedCount > 0)
                    {
                        return "You phone number has been blocked for 24 hours due to invalid code entries";
                    }
                }
                else
                {
                    var update = Builders<CodeFailure>.Update.Set("FailureCount", query.FailureCount + 1).Set("NumberStatus", true).CurrentDate("TimeUpdated");

                    await _dbContext.CodeFailure.UpdateOneAsync(filter, update);

                    return "Invalid Code";
                }
                
            }

            return "Invalid Code";
        }

        public async Task<bool> NumberCheck(string mobileNo)
        {
            //Check If Number already is entered for invalid code entry
            var builder = Builders<CodeFailure>.Filter;
            var filter = builder.Eq("MobileNo", mobileNo) & builder.Eq("NumberStatus", false) & builder.Gte("FailureCount", 3) & builder.Gt("ReactivationTime", DateTime.Now);
            var query = await _dbContext.CodeFailure.Find(filter).FirstOrDefaultAsync();

            if (query != null)
            {
                return false;
            }
            //DeleteResult actionResult = await _dbContext.CodeFailure.DeleteOneAsync(Builders<CodeFailure>.Filter.Eq("MobileNo", mobileNo));
            return true;
        }

        public async Task<PassItOn> PassItOnCheck(string passitCode ,string mobileNo, string campaignId)
        {
            var builder = Builders<PassItOn>.Filter;
            var filter = builder.Eq("MobileNo", mobileNo) & builder.Eq("CampaignId", campaignId) & builder.Eq("PassitCode", passitCode);
            return await _dbContext.PassItOns.Find(filter).FirstOrDefaultAsync();
        }

        public async Task AddPassItOn(string passitCode, string mobileNo, string campaignId, string prize, string network)
        {
            PassItOn passItOn = new PassItOn();
            passItOn.PassitCode = passitCode;
            passItOn.MobileNo = mobileNo;
            passItOn.CampaignId = campaignId;
            passItOn.RecipientName = "PIO Winner";
            passItOn.Reference = mobileNo;
            passItOn.Amount = prize;
            passItOn.Message = "PassItOn Reward";
            passItOn.Status = false;
            passItOn.TimeCreated = DateTime.Now;
            passItOn.TimeUpdated = DateTime.Now;
            if (network == "024" || network == "054" || network == "055")
            {
                passItOn.ServiceCode = "mtn-money";
            }

            if (network == "020" || network == "050")
            {
                passItOn.ServiceCode = "vodafone-cash";
            }

            if (network == "027" || network == "057")
            {
                passItOn.ServiceCode = "tigo-cash";
            }

            if (network == "026")
            {
                passItOn.ServiceCode = "airtel-money";
            }

            await _dbContext.PassItOns.InsertOneAsync(passItOn);
        }

        public async Task<bool> UpdateCampaignCode(string passitCode, int usageLimit)
        {
            try
            {
                bool codeStatus = true;
                var filter = Builders<CampaignCode>.Filter.Eq("Code", passitCode);
                var query = await _dbContext.CampaignCodes.Find(filter).FirstOrDefaultAsync();

                if (usageLimit <= query.Usage + 1)
                    codeStatus = false;

                var update = Builders<CampaignCode>.Update.Set("Usage", query.Usage + 1).Set("CodeStatus", codeStatus).CurrentDate("TimeUpdated");

                UpdateResult actionResult = await _dbContext.CampaignCodes.UpdateOneAsync(filter, update);

                return actionResult.IsAcknowledged
                    && actionResult.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<object> GetPassitonAd(string passitCode)
        {
            var builder = Builders<CampaignCode>.Filter;
            var filter = builder.Eq("Code", passitCode) & builder.Eq("CodeStatus", true);
            var query = await _dbContext.CampaignCodes.Find(filter).FirstOrDefaultAsync();

            if (query != null)
            {
                var adbuilder = Builders<AdInfo>.Filter;
                var adfilter = adbuilder.Eq("CampaignId", query.CampaignId) & adbuilder.Eq("AdStatus", true);
                var passAd = _dbContext.AdInfos.Find(adfilter).ToList();

                if (passAd != null)
                {
                    var pAd = new PassAd();
                    foreach (var adInfo in passAd)
                    {
                        if (adInfo.AdType.Equals("PhoneNo-Image"))
                        {
                            pAd.AdType = adInfo.AdType;
                            pAd.ImageUrl = adInfo.AdMedia.GetElement("ImageUrl").Value.ToString();
                        }
                        else if (adInfo.AdType.Equals("PhoneNo-Video"))
                        {
                            pAd.AdType = adInfo.AdType;
                            pAd.VideoUrl = adInfo.AdMedia.GetElement("VideoUrl").Value.ToString();
                            pAd.VideoHost = adInfo.AdMedia.GetElement("VideoHost").Value.ToString();
                        }
                    }
                    //var js = new JavaScriptSerializer();
                    //return js.Serialize(pAd);
                    return pAd;
                }
            }
            return null;
        }

        public async Task<IEnumerable<PassItOn>> GetAllPassItons()
        {
            try
            {
                return await _dbContext.PassItOns.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }

        }
        #endregion

        #region GeneralAdTasks
        public async Task<IEnumerable<Ad>> GetAllGenAds()
        {
            try
            {
                var builder = Builders<AdInfo>.Filter;
                var filter = builder.Type("CampaignId", BsonType.Null); //& builder.Eq("AdStatus", true);
                var genAds = await _dbContext.AdInfos.Find(filter).ToListAsync();

                List<Ad> adInfos = new List<Ad>();

                string adCountry = "";               
                foreach (var ad in genAds)
                {
                    var adInfo = new Ad();

                    adInfo.Id = ad.Id;
                    adInfo.AdType = ad.AdType;
                    
                    for (int i = 0; i < ad.AdCountry.Count; i++)
                    {
                        if (i == 0)
                        {
                            adCountry = ad.AdCountry[i];
                        }
                        else
                        {
                            adCountry += "," + ad.AdCountry[i];
                        }
                    }
                    adInfo.AdCountry = adCountry;
                    adInfo.AdStatus = ad.AdStatus;
                    adInfo.CreatedBy = ad.CreatedBy;
                    adInfo.TimeCreated = ad.TimeCreated;
                    adInfo.UpdatedBy = ad.UpdatedBy;
                    adInfo.TimeUpdated = ad.TimeUpdated;

                    adInfos.Add(adInfo);
                }
                
                return adInfos;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }

        }

        public async Task<AdInfo> GetGenAds(string id)
        {
            var filter = Builders<AdInfo>.Filter.Eq("_id", ObjectId.Parse(id));

            try
            {
                return await _dbContext.AdInfos
                                .Find(filter)
                                .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<bool> UpdateGenAd(AdInfo adInfo)
        {
            try
            {
                ReplaceOneResult actionResult = await _dbContext.AdInfos
                                                .ReplaceOneAsync(n => n.Id.Equals(adInfo.Id)
                                                                , adInfo
                                                                , new UpdateOptions { IsUpsert = true });
                return actionResult.IsAcknowledged
                    && actionResult.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<bool> RemoveGenAd(string id)
        {
            try
            {
                DeleteResult actionResult = await _dbContext.AdInfos.DeleteOneAsync(
                     Builders<AdInfo>.Filter.Eq("_id", ObjectId.Parse(id)));

                return actionResult.IsAcknowledged
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }
        #endregion

        #region CampaignAdTasks
        public async Task<IEnumerable<Ad>> GetAllCampAds()
        {
            try
            {
                //return await _dbContext.AdInfos.Find(Builders<AdInfo>.Filter.Ne("CampaignId", BsonNull.Value)).ToListAsync();

                var builder = Builders<AdInfo>.Filter;
                var filter = builder.Type("CampaignId", BsonType.String); //& builder.Eq("AdStatus", true);
                var campAds = await _dbContext.AdInfos.Find(filter).ToListAsync();

                List<Ad> adInfos = new List<Ad>();

                string adCountry = "";
                foreach (var ad in campAds)
                {
                    var adInfo = new Ad();

                    adInfo.Id = ad.Id;
                    adInfo.CampaignId = ad.CampaignId;
                    adInfo.AdType = ad.AdType;

                    for (int i = 0; i < ad.AdCountry.Count; i++)
                    {
                        if (i == 0)
                        {
                            adCountry = ad.AdCountry[i];
                        }
                        else
                        {
                            adCountry += "," + ad.AdCountry[i];
                        }
                    }
                    adInfo.AdCountry = adCountry;
                    adInfo.AdStatus = ad.AdStatus;
                    adInfo.CreatedBy = ad.CreatedBy;
                    adInfo.TimeCreated = ad.TimeCreated;
                    adInfo.UpdatedBy = ad.UpdatedBy;
                    adInfo.TimeUpdated = ad.TimeUpdated;

                    adInfos.Add(adInfo);
                }

                return adInfos;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }

        }

        public async Task<AdInfo> GetCampAd(string id)
        {
            var filter = Builders<AdInfo>.Filter.Eq("CampaignId", id);

            try
            {
                return await _dbContext.AdInfos
                                .Find(filter)
                                .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<bool> UpdateCampAd(AdInfo adInfo)
        {
            try
            {
                ReplaceOneResult actionResult = await _dbContext.AdInfos
                                                .ReplaceOneAsync(n => n.CampaignId.Equals(adInfo.CampaignId)
                                                                , adInfo
                                                                , new UpdateOptions { IsUpsert = true });
                return actionResult.IsAcknowledged
                    && actionResult.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<bool> RemoveCampAd(string id)
        {
            try
            {
                DeleteResult actionResult = await _dbContext.AdInfos.DeleteOneAsync(
                     Builders<AdInfo>.Filter.Eq("_id", ObjectId.Parse(id)));

                return actionResult.IsAcknowledged
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }
        #endregion

        #region GenerateCardTasks
        public async Task<IEnumerable<CodeCard>> GetAllGenCards()
        {
            try
            {
                return await _dbContext.CodeCard.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }

        }

        public async Task<CodeCard> GetGenCard(string id)
        {
            var filter = Builders<CodeCard>.Filter.Eq("Id", id);

            try
            {
                return await _dbContext.CodeCard.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<bool> RemoveCard(string id)
        {
            try
            {
                DeleteResult actionResult = await _dbContext.CodeCard.DeleteOneAsync(
                     Builders<CodeCard>.Filter.Eq("_id", ObjectId.Parse(id)));

                return actionResult.IsAcknowledged
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }
        #endregion

        #region CodeFailureTasks
        public async Task<IEnumerable<CodeFailure>> GetCodeFailures()
        {
            try
            {
                return await _dbContext.CodeFailure.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }

        }

        public async Task<CodeFailure> GetCodeFailure(string id)
        {
            var filter = Builders<CodeFailure>.Filter.Eq("Id", id);

            try
            {
                return await _dbContext.CodeFailure.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<bool> RemoveCodeFailure(string id)
        {
            try
            {
                DeleteResult actionResult = await _dbContext.CodeFailure.DeleteOneAsync(
                     Builders<CodeFailure>.Filter.Eq("_id", ObjectId.Parse(id)));

                return actionResult.IsAcknowledged
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }
        #endregion
    }
}