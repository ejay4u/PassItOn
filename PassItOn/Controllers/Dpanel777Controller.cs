using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using MongoDB.Bson;
using MongoDB.Driver;
using PassItOn.Models;
using PassItOn.ViewModels;
using System.IO;
using ImageMagick;

namespace PassItOn.Controllers
{
    public class Dpanel777Controller : Controller
    {
        readonly MongoContext _dbContext;

        public Dpanel777Controller()
        {
            _dbContext = new MongoContext();
        }

        // GET: Dpanel777
        public ActionResult Index()
        {
            return View();
        }

        //GET: Dpanel777/Business
        public ActionResult Business()
        {
            List<string> country = new List<string>();
            CultureInfo[] cInfoList = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            foreach (CultureInfo cInfo in cInfoList)
            {
                RegionInfo R = new RegionInfo(cInfo.LCID);
                if (!(country.Contains(R.EnglishName)))
                {
                    country.Add(R.EnglishName);
                }
            }
            if (!(country.Contains("Ghana")))
            {
                country.Add("Ghana");
            }
            country.Sort();

            // Build a List<SelectListItem>
            var countryList = country.Select(x => new SelectListItem() { Value = x, Text = x }).ToList();

            var businessViewModel = new BusinessViewModel()
            {
                CountryList = countryList
            };
            return View(businessViewModel);
        }


        //POST: Dpanel777/Business
        [HttpPost]
        public ActionResult Business(Business business, BusinessViewModel businessViewModel)
        {
            if(businessViewModel.Id.IsNullOrWhiteSpace())
            {
                business.AccountId = new AccountNoGen().RandomAccountNo();
                business.TimeCreated = DateTime.Now;
                business.TimeUpdated = DateTime.Now;
                //business.Status = true;
                var builder = Builders<Business>.Filter;
                var filter = builder.Eq("AccountId", business.AccountId) & builder.Eq("BusinessName", business.BusinessName);
                var query = _dbContext.Businesses.Find(filter).ToList();

                if (query.Count == 0)
                {
                    _dbContext.Businesses.InsertOne(business);
                }
                else
                {
                    //TempData["Message"] = "Business Already Exists";
                    List<string> country = new List<string>();
                    CultureInfo[] cInfoList = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
                    foreach (CultureInfo cInfo in cInfoList)
                    {
                        RegionInfo R = new RegionInfo(cInfo.LCID);
                        if (!(country.Contains(R.EnglishName)))
                        {
                            country.Add(R.EnglishName);
                        }
                    }
                    if (!(country.Contains("Ghana")))
                    {
                        country.Add("Ghana");
                    }
                    country.Sort();

                    // Build a List<SelectListItem>
                    var countryList = country.Select(x => new SelectListItem() { Value = x, Text = x }).ToList();

                    var busViewModel = new BusinessViewModel()
                    {
                        CountryList = countryList,
                        Business = business
                    };
                    return View("Business", busViewModel);
                }
                return RedirectToAction("Business");
            }
            else
            {
                business.Id = ObjectId.Parse(businessViewModel.Id);
                business.TimeUpdated = DateTime.Now;
                var builder = Builders<Business>.Filter;
                var filter = builder.Eq("_id", business.Id);
                /*var query = _dbContext.Businesses.Find(filter).ToEnumerable();
                business.Id = query.ElementAt(0).Id;*/

                //Update Business
                _dbContext.Businesses.ReplaceOne(filter, business, new UpdateOptions { IsUpsert = false });

                if (!business.Status)
                {
                    var cbuilder = Builders<Campaign>.Filter;
                    var cfilter = cbuilder.Eq("AccountId", business.AccountId);
                    var update = Builders<Campaign>.Update.Set("CampaignStatus", false).CurrentDate("TimeUpdated");
                    _dbContext.Campaigns.UpdateMany(cfilter, update);
                }

                return RedirectToAction("Business");
            }
            
        }

        public ActionResult EditBusiness(string id)
        {
            var filter = Builders<Business>.Filter.Eq("_id", ObjectId.Parse(id));

            var business = _dbContext.Businesses.Find(filter).First();

            List<string> country = new List<string>();
            CultureInfo[] cInfoList = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            foreach (CultureInfo cInfo in cInfoList)
            {
                RegionInfo R = new RegionInfo(cInfo.LCID);
                if (!(country.Contains(R.EnglishName)))
                {
                    country.Add(R.EnglishName);
                }
            }
            if (!(country.Contains("Ghana")))
            {
                country.Add("Ghana");
            }
            country.Sort();

            // Build a List<SelectListItem>
            var countryList = country.Select(x => new SelectListItem() { Value = x, Text = x }).ToList();

            var businessViewModel = new BusinessViewModel()
            {
                Business = business,
                CountryList = countryList
            };

            return View("Business", businessViewModel);
        }

        public ActionResult BusinessDetails(string id)
        {
            var filter = Builders<Business>.Filter.Eq("_id", ObjectId.Parse(id));

            var business = _dbContext.Businesses.Find(filter).First();

            return View("BusinessDetails", business);
        }

        public ActionResult Campaign()
        {
            var filter = Builders<Business>.Filter.Empty;
            var projection = Builders<Business>.Projection.Include("AccountId").Include("BusinessName");
            var businesses = _dbContext.Businesses.Find(filter).Project(projection).ToEnumerable();

            List<SelectListItem> businessname = new List<SelectListItem>();
            foreach (var bus in businesses)
            {
                SelectListItem bname = new SelectListItem()
                {
                    Text = bus.GetElement(2).Value.AsString,
                    Value = bus.GetElement(1).Value.AsString
                };
                businessname.Add(bname);
            }

            List<string> country = new List<string>();
            CultureInfo[] cInfoList = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            foreach (CultureInfo cInfo in cInfoList)
            {
                RegionInfo R = new RegionInfo(cInfo.LCID);
                if (!(country.Contains(R.EnglishName)))
                {
                    country.Add(R.EnglishName);
                }
            }
            if (!(country.Contains("Ghana")))
            {
                country.Add("Ghana");
            }
            country.Sort();

            // Build a List<SelectListItem>
            var countryList = country.Select(x => new SelectListItem() { Value = x, Text = x }).ToList();

            //Create campaignNetwork List
            List<string> campaignNetwork = new List<string>(new[] { "MTN", "VODAFONE", "AIRTEL", "TIGO" });
            var campaignNetworkList = campaignNetwork.Select(x => new SelectListItem() { Value = x, Text = x }).ToList();

            var viewModel = new CampaignViewModel
            {
                BusinessName = businessname,
                CountryList = countryList,
                CampaignNetwork = campaignNetworkList
            };

            return View(viewModel);
        }

        //POST: Dpanel777/AddCampaign
        [HttpPost]
        public ActionResult Campaign(Campaign campaign, CampaignViewModel campaignViewModel)
        {
            if (campaignViewModel.Id.IsNullOrWhiteSpace())
            {
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
                campaign.TimeUpdated = DateTime.Now;
                _dbContext.Campaigns.InsertOne(campaign);
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

                    _dbContext.CampaignCodes.InsertOne(codes);
                }

                return RedirectToAction("Campaign");
            }
            else
            {
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

                campaign.Id = ObjectId.Parse(campaignViewModel.Id);
                campaign.TimeUpdated = DateTime.Now;

                //Update Campaign
                var builder = Builders<Campaign>.Filter;
                var filter = builder.Eq("_id", ObjectId.Parse(campaignViewModel.Id));
                _dbContext.Campaigns.ReplaceOne(filter, campaign, new UpdateOptions { IsUpsert = false });

                return RedirectToAction("Campaign");
            }

            
        }

        public ActionResult EditCampaign(string id)
        {
            var filter = Builders<Campaign>.Filter.Eq("_id", ObjectId.Parse(id));
            var campaign = _dbContext.Campaigns.Find(filter).First();

            var filter2 = Builders<Business>.Filter.Empty;
            var projection = Builders<Business>.Projection.Include("AccountId").Include("BusinessName");
            var businesses = _dbContext.Businesses.Find(filter2).Project(projection).ToEnumerable();

            List<SelectListItem> businessname = new List<SelectListItem>();
            foreach (var bus in businesses)
            {
                SelectListItem bname = new SelectListItem()
                {
                    Text = bus.GetElement(2).Value.AsString,
                    Value = bus.GetElement(1).Value.AsString
                };
                businessname.Add(bname);
            }

            List<string> country = new List<string>();
            CultureInfo[] cInfoList = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            foreach (CultureInfo cInfo in cInfoList)
            {
                RegionInfo R = new RegionInfo(cInfo.LCID);
                if (!(country.Contains(R.EnglishName)))
                {
                    country.Add(R.EnglishName);
                }
            }
            if (!(country.Contains("Ghana")))
            {
                country.Add("Ghana");
            }
            country.Sort();

            // Build a List<SelectListItem>
            var countryList = country.Select(x => new SelectListItem() { Value = x, Text = x }).ToList();

            //Create campaignNetwork List
            List<string> campaignNetwork = new List<string>(new[] { "MTN", "VODAFONE", "AIRTEL", "TIGO" });
            var campaignNetworkList = campaignNetwork.Select(x => new SelectListItem() { Value = x, Text = x }).ToList();


            //Get all selected Networks
            List<string> selcampaignList = new List<string>();
            if (campaign.CampaignNetwork != null)
            {
                foreach (var network in campaign.CampaignNetwork)
                {
                    if (network.Contains("024") || network.Contains("054") || network.Contains("055"))
                    {
                        selcampaignList.Add("MTN");
                    }

                    if (network.Contains("020") || network.Contains("050"))
                    {
                        selcampaignList.Add("VODAFONE");
                    }

                    if (network.Contains("027") || network.Contains("057"))
                    {
                        selcampaignList.Add("TIGO");
                    }

                    if (network.Contains("026"))
                    {
                        selcampaignList.Add("AIRTEL");

                    }
                }
            }

            var campaignviewModel = new CampaignViewModel
            {
                Campaign = campaign,
                BusinessName = businessname,
                CountryList = countryList,
                CampaignNetwork = campaignNetworkList,
                SelectedCampaignNetwork = selcampaignList,
                Id = campaign.Id.ToString()
            };

            return View("Campaign",campaignviewModel);
        }

        public ActionResult CampaignDetails(string id)
        {
            var filter = Builders<Campaign>.Filter.Eq("_id", ObjectId.Parse(id));

            var campaign = _dbContext.Campaigns.Find(filter).First();

            return View("CampaignDetails", campaign);
        }

        //GET: Dpanel777/GenerateCodes
        public ActionResult GenerateCodes()
        {
            List<string> country = new List<string>();
            CultureInfo[] cInfoList = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            foreach (CultureInfo cInfo in cInfoList)
            {
                RegionInfo R = new RegionInfo(cInfo.LCID);
                if (!(country.Contains(R.EnglishName)))
                {
                    country.Add(R.EnglishName);
                }
            }
            if (!(country.Contains("Ghana")))
            {
                country.Add("Ghana");
            }
            country.Sort();

            // Build a List<SelectListItem>
            var countryList = country.Select(x => new SelectListItem() { Value = x, Text = x }).ToList();

            var viewModel = new CodeViewModel()
            {
                CountryList = countryList
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult GenerateCodes(CampaignCode campaignCode, Campaign campaign)
        {
            var builder = Builders<Campaign>.Filter;
            var filter = builder.Eq("_id", ObjectId.Parse(campaignCode.CampaignId));
            var query = _dbContext.Campaigns.Find(filter).FirstOrDefault();

            if (query != null)
            {
                try
                {
                    var update = Builders<Campaign>.Update.Set("CampaignCodeQty", query.CampaignCodeQty + campaign.CampaignCodeQty).CurrentDate("TimeUpdated");
                    _dbContext.Campaigns.UpdateOne(filter, update);

                    int codeQty = campaign.CampaignCodeQty;
                    campaign.TimeCreated = DateTime.Now;
                    campaign.TimeUpdated = DateTime.Now;
                    campaign.CampaignStatus = true;

                    for (int i = 0; i < codeQty; i++)
                    {
                        var codes = new CampaignCode();
                        {
                            codes.CampaignId = campaignCode.CampaignId;
                            codes.CodeStatus = true;
                            codes.TimeCreated = DateTime.Now;
                            codes.Code = Guid.NewGuid().ToString().ToUpper().Substring(0, 11).Replace("-", string.Empty);
                        };

                        _dbContext.CampaignCodes.InsertOne(codes);
                    }
                }
                catch (Exception ex)
                {
                    // log or manage the exception
                    throw ex;
                }

            }
            return RedirectToAction("GenerateCodes");
        }

        public ActionResult CountryListView()
        {
            List<string> country = new List<string>();
            CultureInfo[] cInfoList = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            foreach (CultureInfo cInfo in cInfoList)
            {
                RegionInfo R = new RegionInfo(cInfo.LCID);
                if (!(country.Contains(R.EnglishName)))
                {
                    country.Add(R.EnglishName);
                }
            }
            if (!(country.Contains("Ghana")))
            {
                country.Add("Ghana");
            }
            country.Sort();

            // Build a List<SelectListItem>
            var countryList = country.Select(x => new SelectListItem() { Value = x, Text = x }).ToList();

            var countryViewModel = new AdViewModel()
            {
                CountryList = countryList
            };

            return View(countryViewModel);
        }

        public ActionResult BusinessListView(string country)
        {
            var builder = Builders<Business>.Filter;
            var filter = builder.Eq("Country", country);
            var projection = Builders<Business>.Projection.Include("AccountId").Include("BusinessName");
            var businesses = _dbContext.Businesses.Find(filter).Project(projection).ToEnumerable();

            List<SelectListItem> businessList = new List<SelectListItem>();
            foreach (var business in businesses)
            {
                SelectListItem bname = new SelectListItem()
                {
                    Text = business.GetElement(2).Value.AsString,
                    Value = business.GetElement(1).Value.AsString
                };
                businessList.Add(bname);
            }

            var viewModel = new CodeViewModel()
            {
                BusinessList = businessList
            };

            return View(viewModel);
        }

        public ActionResult CodeCampaignListView(string accountid)
        {
            var builder = Builders<Campaign>.Filter;
            var filter = builder.Eq("AccountId", accountid) & builder.Eq("CampaignStatus", true); ;
            var projection = Builders<Campaign>.Projection.Include("CampaignTitle");
            var campaigns = _dbContext.Campaigns.Find(filter).Project(projection).ToEnumerable();

            List<SelectListItem> campaignList = new List<SelectListItem>();
            foreach (var campaign in campaigns)
            {
                SelectListItem bname = new SelectListItem()
                {
                    Text = campaign.GetElement(1).Value.AsString,
                    Value = campaign.GetElement(0).Value.AsObjectId.ToString()
                };
                campaignList.Add(bname);
            }

            var codeViewModel = new CodeViewModel()
            {
                CampaignList = campaignList
            };

            return View(codeViewModel);
        }

        //[Route("Dpanel777/CampaignListView/{country?}/{accountid?}")]
        public ActionResult CampaignListView(string country)
        {
            var builder = Builders<Campaign>.Filter;
            var filter = builder.Eq("CampaignCountry", country) & builder.Eq("CampaignStatus", true);
            var projection = Builders<Campaign>.Projection.Include("CampaignTitle");
            var campaigns = _dbContext.Campaigns.Find(filter).Project(projection).ToEnumerable();

            List<SelectListItem> campaignList = new List<SelectListItem>();
            foreach (var campaign in campaigns)
            {
                SelectListItem bname = new SelectListItem()
                {
                    Text = campaign.GetElement(1).Value.AsString,
                    Value = campaign.GetElement(0).Value.AsObjectId.ToString()
                };
                campaignList.Add(bname);
            }

            var campaignViewModel = new AdViewModel()
            {
                CampaignList = campaignList
            };

            return View(campaignViewModel);
        }

        public ActionResult GeneralAd()
        {
            List<string> country = new List<string>();
            CultureInfo[] cInfoList = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            foreach (CultureInfo cInfo in cInfoList)
            {
                RegionInfo R = new RegionInfo(cInfo.LCID);
                if (!(country.Contains(R.EnglishName)))
                {
                    country.Add(R.EnglishName);
                }
            }
            if (!(country.Contains("Ghana")))
            {
                country.Add("Ghana");
            }
            country.Sort();

            // Build a List<SelectListItem>
            var countryList = country.Select(x => new SelectListItem() { Value = x, Text = x }).ToList();

            //Create adType List
            List<string> addType = new List<string>(new[] { "Background-Image", "Background-Video(mute)", "Background-Audio", "PassItCode-Image", "PassItCode-Video", "PhoneNo-Image(Default)", "PhoneNo-Video(Default)" });
            var addTypeList = addType.Select(x => new SelectListItem() { Value = x, Text = x }).ToList();

            //Create videoHost List
            List<string> videoHost = new List<string>(new[] { "YouTube", "Vimeo", "Dailymotion" });
            var videoHostList = videoHost.Select(x => new SelectListItem() { Value = x, Text = x }).ToList();

            var generalAdView = new AdViewModel()
            {
                CountryList = countryList,
                AddTypeList = addTypeList,
                VideoHostList = videoHostList
            };
            return View(generalAdView);
        }

        [HttpPost]
        public ActionResult GeneralAd(AdInfo adInfo, AdViewModel adViewModel)
        {
            if (adViewModel.Id.IsNullOrWhiteSpace())
            {
                adInfo.AdCountry = adViewModel.AdInfo.AdCountry;
                adInfo.AdType = adViewModel.AdInfo.AdType;

                if (!adViewModel.ImageUrl.IsNullOrWhiteSpace())
                {
                    //Get The File Posted
                    HttpPostedFileBase image = Request.Files["ImageUrl"];
                    //Generate a filename to store the file on the server
                    if (image != null)
                    {
                        var fileName = Guid.NewGuid() + Path.GetFileName(image.FileName);
                        {
                            var imageTypes = new[] { "jpg", "jpeg", "png" };
                            var extension = Path.GetExtension(Path.GetFileName(image.FileName));
                            if (extension != null)
                            {
                                var fileExt = extension.Substring(1).ToLower();
                                if (imageTypes.Contains(fileExt))
                                {
                                    var imagePath = Path.Combine(Server.MapPath("~/assets/image"), fileName);

                                    // store the uploaded file on the file system
                                    image.SaveAs(imagePath);

                                    //Return final filepath to database
                                    adInfo.AdMedia = new BsonDocument
                                    {
                                        { "ImageUrl", "/assets/image/" + fileName },
                                        {"VideoUrl", BsonNull.Value},
                                        {"VideoHost", BsonNull.Value},
                                        {"AudioUrl", BsonNull.Value }
                                    };
                                }
                            }
                        }
                    }
                }
                else if (!adViewModel.AudioUrl.IsNullOrWhiteSpace())
                {
                    //Get The File Posted
                    HttpPostedFileBase audio = Request.Files["AudioUrl"];

                    if (audio != null)
                    {
                        var fileName = Guid.NewGuid() + Path.GetFileName(audio.FileName);
                        {
                            var audioTypes = new[] { "mp3" };
                            var extension = Path.GetExtension(Path.GetFileName(audio.FileName));
                            if (extension != null)
                            {
                                var fileExt = extension.Substring(1).ToLower();
                                if (audioTypes.Contains(fileExt))
                                {
                                    var imagePath = Path.Combine(Server.MapPath("~/assets/audio"), fileName);

                                    // store the uploaded file on the file system
                                    audio.SaveAs(imagePath);

                                    //Return final filepath to database
                                    adInfo.AdMedia = new BsonDocument
                                    {
                                        { "AudioUrl", "/assets/audio/" + fileName },
                                        {"VideoUrl", BsonNull.Value},
                                        {"VideoHost", BsonNull.Value},
                                        {"ImageUrl", BsonNull.Value }
                                    };
                                }
                            }
                        }
                    }
                }
                else if (!adViewModel.VideoUrl.IsNullOrWhiteSpace() && !adViewModel.VideoHost.IsNullOrWhiteSpace())
                {
                    adInfo.AdMedia = new BsonDocument
                    {
                        { "VideoUrl", adViewModel.VideoUrl },
                        { "VideoHost", adViewModel.VideoHost },
                        {"ImageUrl", BsonNull.Value },
                        {"AudioUrl", BsonNull.Value }
                    };
                }

                //Check if Add exists for same country and deactivate NB: BG-image and Bg-video are mutually exclusive
                if (adViewModel.AdInfo.AdType.Equals("Background-Image") ||
                    adViewModel.AdInfo.AdType.Equals("Background-Video(mute)"))
                {
                    var builder = Builders<AdInfo>.Filter;
                    var filter = builder.Type("CampaignId", BsonType.Null) &
                                 builder.Or(builder.Eq("AdType", "Background-Image"),
                                     builder.Eq("AdType", "Background-Video(mute)")) &
                                 builder.Eq("AdCountry", adViewModel.AdInfo.AdCountry) &
                                 builder.Eq("AdStatus", true);
                    var query = _dbContext.AdInfos.Find(filter).FirstOrDefaultAsync();
                    if (query != null)
                    {
                        try
                        {
                            var update = Builders<AdInfo>.Update.Set("AdStatus", false).CurrentDate("TimeUpdated");
                            _dbContext.AdInfos.UpdateOne(filter, update);
                        }
                        catch (Exception ex)
                        {
                            // log or manage the exception
                            throw ex;
                        }
                    }
                }
                else
                {
                    var builder = Builders<AdInfo>.Filter;
                    var filter = builder.Type("CampaignId", BsonType.Null) &
                                 builder.Eq("AdType", adViewModel.AdInfo.AdType) &
                                 builder.Eq("AdCountry", adViewModel.AdInfo.AdCountry) &
                                 builder.Eq("AdStatus", true);
                    var query = _dbContext.AdInfos.Find(filter).FirstOrDefaultAsync();
                    if (query != null)
                    {
                        try
                        {
                            var update = Builders<AdInfo>.Update.Set("AdStatus", false).CurrentDate("TimeUpdated");
                            _dbContext.AdInfos.UpdateOne(filter, update);
                        }
                        catch (Exception ex)
                        {
                            // log or manage the exception
                            throw ex;
                        }
                    }
                }


                //Insert the new Add.
                adInfo.AdStatus = adViewModel.AdInfo.AdStatus;
                adInfo.TimeCreated = DateTime.Now;
                adInfo.TimeUpdated = DateTime.Now;
                _dbContext.AdInfos.InsertOneAsync(adInfo);
            }
            else
            {
                adInfo.AdCountry = adViewModel.AdInfo.AdCountry;
                adInfo.AdType = adViewModel.AdInfo.AdType;

                if (!adViewModel.ImageUrl.IsNullOrWhiteSpace())
                {
                    //Get The File Posted
                    HttpPostedFileBase image = Request.Files["ImageUrl"];
                    //Generate a filename to store the file on the server
                    if (image != null)
                    {
                        var fileName = Guid.NewGuid() + Path.GetFileName(image.FileName);
                        {
                            var imageTypes = new[] {"jpg", "jpeg", "png"};
                            var extension = Path.GetExtension(Path.GetFileName(image.FileName));
                            if (extension != null)
                            {
                                var fileExt = extension.Substring(1).ToLower();
                                if (imageTypes.Contains(fileExt))
                                {
                                    var imagePath = Path.Combine(Server.MapPath("~/assets/image"), fileName);

                                    // store the uploaded file on the file system
                                    image.SaveAs(imagePath);

                                    //Return final filepath to database
                                    adInfo.AdMedia = new BsonDocument
                                    {
                                        {"ImageUrl", "/assets/image/" + fileName},
                                        {"VideoUrl", BsonNull.Value},
                                        {"VideoHost", BsonNull.Value},
                                        {"AudioUrl", BsonNull.Value }
                                    };
                                }
                            }
                        }
                    }
                }
                else if (!adViewModel.AudioUrl.IsNullOrWhiteSpace())
                {
                    //Get The File Posted
                    HttpPostedFileBase audio = Request.Files["AudioUrl"];

                    if (audio != null)
                    {
                        var fileName = Guid.NewGuid() + Path.GetFileName(audio.FileName);
                        {
                            var audioTypes = new[] { "mp3" };
                            var extension = Path.GetExtension(Path.GetFileName(audio.FileName));
                            if (extension != null)
                            {
                                var fileExt = extension.Substring(1).ToLower();
                                if (audioTypes.Contains(fileExt))
                                {
                                    var imagePath = Path.Combine(Server.MapPath("~/assets/audio"), fileName);

                                    // store the uploaded file on the file system
                                    audio.SaveAs(imagePath);

                                    //Return final filepath to database
                                    adInfo.AdMedia = new BsonDocument
                                    {
                                        { "AudioUrl", "/assets/audio/" + fileName },
                                        {"VideoUrl", BsonNull.Value},
                                        {"VideoHost", BsonNull.Value},
                                        {"ImageUrl", BsonNull.Value }
                                    };
                                }
                            }
                        }
                    }
                }
                else if (!adViewModel.VideoUrl.IsNullOrWhiteSpace() && !adViewModel.VideoHost.IsNullOrWhiteSpace())
                {
                    adInfo.AdMedia = new BsonDocument
                    {
                        { "VideoUrl", adViewModel.VideoUrl },
                        { "VideoHost", adViewModel.VideoHost },
                        {"ImageUrl", BsonNull.Value },
                        {"AudioUrl", BsonNull.Value }
                    };
                }

                //Update the Ad.
                adInfo.Id = ObjectId.Parse(adViewModel.Id);
                adInfo.TimeUpdated = DateTime.Now;
                var builder = Builders<AdInfo>.Filter;
                var filter = builder.Eq("_id", adInfo.Id);
                _dbContext.AdInfos.ReplaceOne(filter, adInfo, new UpdateOptions { IsUpsert = false });
            }

            return RedirectToAction("GeneralAd");
        }

        public ActionResult EditGeneralAd(string id)
        {
            var filter = Builders<AdInfo>.Filter.Eq("_id", ObjectId.Parse(id));
            var generalAd = _dbContext.AdInfos.Find(filter).First();

            List<string> country = new List<string>();
            CultureInfo[] cInfoList = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            foreach (CultureInfo cInfo in cInfoList)
            {
                RegionInfo R = new RegionInfo(cInfo.LCID);
                if (!(country.Contains(R.EnglishName)))
                {
                    country.Add(R.EnglishName);
                }
            }
            if (!(country.Contains("Ghana")))
            {
                country.Add("Ghana");
            }
            country.Sort();

            // Build a List<SelectListItem>
            var countryList = country.Select(x => new SelectListItem() { Value = x, Text = x }).ToList();

            //Create adType List
            List<string> addType = new List<string>(new[] { "Background-Image", "Background-Video(mute)", "Background-Audio", "PassItCode-Image", "PassItCode-Video", "PhoneNo-Image(Default)", "PhoneNo-Video(Default)" });
            var addTypeList = addType.Select(x => new SelectListItem() { Value = x, Text = x }).ToList();

            //Create videoHost List
            List<string> videoHost = new List<string>(new[] { "YouTube", "Vimeo", "Dailymotion" });
            var videoHostList = videoHost.Select(x => new SelectListItem() { Value = x, Text = x }).ToList();

            var generalAdView = new AdViewModel()
            {
                AdInfo = generalAd,
                CountryList = countryList,
                AddTypeList = addTypeList,
                VideoHostList = videoHostList,
                VideoUrl = generalAd.AdMedia.GetElement("VideoUrl").Value.ToString(),
                VideoHost = generalAd.AdMedia.GetElement("VideoHost").Value.ToString(),
                ImageUrl = generalAd.AdMedia.GetElement("ImageUrl").Value.ToString(),
                AudioUrl = generalAd.AdMedia.GetElement("AudioUrl").Value.ToString()
            };

            return View("GeneralAd",generalAdView);
        }

        public ActionResult CampaignAd()
        {
            //Create addType List
            List<string> addType = new List<string>(new[] { "PhoneNo-Image", "PhoneNo-Video" });
            var addTypeList = addType.Select(x => new SelectListItem() { Value = x, Text = x }).ToList();

            //Create videoHost List
            List<string> videoHost = new List<string>(new[] { "YouTube", "Vimeo", "Dailymotion" });
            var videoHostList = videoHost.Select(x => new SelectListItem() { Value = x, Text = x }).ToList();

            var campaignAdView = new AdViewModel()
            {
                AddTypeList = addTypeList,
                VideoHostList = videoHostList
            };
            return View(campaignAdView);
        }

        public ActionResult EditCampaignAd(string id)
        {
            var filter = Builders<AdInfo>.Filter.Eq("_id", ObjectId.Parse(id));
            var campAd = _dbContext.AdInfos.Find(filter).First();

            //Create addType List
            List<string> addType = new List<string>(new[] { "PhoneNo-Image", "PhoneNo-Video" });
            var addTypeList = addType.Select(x => new SelectListItem() { Value = x, Text = x }).ToList();

            //Create videoHost List
            List<string> videoHost = new List<string>(new[] { "YouTube", "Vimeo", "Dailymotion" });
            var videoHostList = videoHost.Select(x => new SelectListItem() { Value = x, Text = x }).ToList();

            var campaignAdView = new AdViewModel()
            {
                AdInfo = campAd,
                AddTypeList = addTypeList,
                VideoHostList = videoHostList,
                VideoUrl = campAd.AdMedia.GetElement("VideoUrl").Value.ToString(),
                VideoHost = campAd.AdMedia.GetElement("VideoHost").Value.ToString(),
                ImageUrl = campAd.AdMedia.GetElement("ImageUrl").Value.ToString(),
            };
            return View("CampaignAd",campaignAdView);
        }

        [HttpPost]
        public ActionResult CampaignAd(AdInfo adInfo, AdViewModel adViewModel)
        {
            if (adViewModel.Id.IsNullOrWhiteSpace())
            {
                adInfo.CampaignId = adViewModel.AdInfo.CampaignId;
                adInfo.AdCountry = adViewModel.AdInfo.AdCountry;
                adInfo.AdType = adViewModel.AdInfo.AdType;
                if (!adViewModel.ImageUrl.IsNullOrWhiteSpace())
                {
                    //Get The File Posted
                    HttpPostedFileBase image = Request.Files["ImageUrl"];
                    //Generate a filename to store the file on the server
                    if (image != null)
                    {
                        var imageTypes = new[] {"jpg", "jpeg", "png"};
                        var extension = Path.GetExtension(Path.GetFileName(image.FileName));
                        if (extension != null)
                        {
                            var fileExt = extension.Substring(1).ToLower();
                            if (imageTypes.Contains(fileExt))
                            {
                                var fileName = Guid.NewGuid() + Path.GetFileName(image.FileName);
                                var imagePath = Path.Combine(Server.MapPath("~/assets/image"), fileName);

                                // store the uploaded file on the file system
                                image.SaveAs(imagePath);

                                //Return final filepath to database
                                adInfo.AdMedia = new BsonDocument
                                {
                                    {"ImageUrl", "/assets/image/" + fileName},
                                    {"VideoUrl", BsonNull.Value},
                                    {"VideoHost", BsonNull.Value},
                                };
                            }
                        }
                    }
                }
                else if (!adViewModel.VideoUrl.IsNullOrWhiteSpace() && !adViewModel.VideoHost.IsNullOrWhiteSpace())
                {
                    adInfo.AdMedia = new BsonDocument
                    {
                        {"VideoUrl", adViewModel.VideoUrl},
                        {"VideoHost", adViewModel.VideoHost},
                        {"ImageUrl", BsonNull.Value},
                    };
                }


                //Check if Add exists for same country and deactivate
                var builder1 = Builders<AdInfo>.Filter;
                var filter1 = builder1.Eq("CampaignId", adViewModel.AdInfo.CampaignId) &
                              builder1.Eq("AdType", adViewModel.AdInfo.AdType) &
                              builder1.Eq("AdCountry", adViewModel.AdInfo.AdCountry) &
                              builder1.Eq("AdStatus", true);
                var query1 = _dbContext.AdInfos.Find(filter1).FirstOrDefaultAsync();
                if (query1 != null)
                {
                    try
                    {
                        var update = Builders<AdInfo>.Update.Set("AdStatus", false).CurrentDate("TimeUpdated");
                        _dbContext.AdInfos.UpdateOne(filter1, update);
                    }
                    catch (Exception ex)
                    {
                        // log or manage the exception
                        throw ex;
                    }
                }

                //Insert the new Add.
                adInfo.AdStatus = adViewModel.AdInfo.AdStatus;
                adInfo.TimeCreated = DateTime.Now;
                adInfo.TimeUpdated = DateTime.Now;
                var id = _dbContext.AdInfos.InsertOneAsync(adInfo);
            }
            else
            {
                adInfo.CampaignId = adViewModel.AdInfo.CampaignId;
                adInfo.AdCountry = adViewModel.AdInfo.AdCountry;
                adInfo.AdType = adViewModel.AdInfo.AdType;
                if (!adViewModel.ImageUrl.IsNullOrWhiteSpace())
                {
                    //Get The File Posted
                    HttpPostedFileBase image = Request.Files["ImageUrl"];
                    //Generate a filename to store the file on the server
                    if (image != null)
                    {
                        var imageTypes = new[] { "jpg", "jpeg", "png" };
                        var extension = Path.GetExtension(Path.GetFileName(image.FileName));
                        if (extension != null)
                        {
                            var fileExt = extension.Substring(1).ToLower();
                            if (imageTypes.Contains(fileExt))
                            {
                                var fileName = Guid.NewGuid() + Path.GetFileName(image.FileName);
                                var imagePath = Path.Combine(Server.MapPath("~/assets/image"), fileName);

                                // store the uploaded file on the file system
                                image.SaveAs(imagePath);

                                //Return final filepath to database
                                adInfo.AdMedia = new BsonDocument
                            {
                                { "ImageUrl", "/assets/image/" + fileName },
                                { "VideoUrl", BsonNull.Value },
                                { "VideoHost", BsonNull.Value },
                            };
                            }
                        }
                    }
                }
                else if (!adViewModel.VideoUrl.IsNullOrWhiteSpace() && !adViewModel.VideoHost.IsNullOrWhiteSpace())
                {
                    adInfo.AdMedia = new BsonDocument
                {
                    { "VideoUrl", adViewModel.VideoUrl },
                    { "VideoHost", adViewModel.VideoHost },
                    { "ImageUrl", BsonNull.Value },
                };
                }


                //Check if Add exists for same country and deactivate
                var builder1 = Builders<AdInfo>.Filter;
                var filter1 = builder1.Eq("CampaignId", adViewModel.AdInfo.CampaignId) & builder1.Eq("AdType", adViewModel.AdInfo.AdType) & builder1.Eq("AdCountry", adViewModel.AdInfo.AdCountry) &
                             builder1.Eq("AdStatus", true);
                var query1 = _dbContext.AdInfos.Find(filter1).FirstOrDefaultAsync();
                if (query1 != null)
                {
                    try
                    {
                        var update = Builders<AdInfo>.Update.Set("AdStatus", false).CurrentDate("TimeUpdated");
                        _dbContext.AdInfos.UpdateOne(filter1, update);
                    }
                    catch (Exception ex)
                    {
                        // log or manage the exception
                        throw ex;
                    }
                }

                //Update the Campaign Add.
                adInfo.Id = ObjectId.Parse(adViewModel.Id);
                adInfo.TimeUpdated = DateTime.Now;
                var builder = Builders<AdInfo>.Filter;
                var filter = builder.Eq("_id", adInfo.Id);
                _dbContext.AdInfos.ReplaceOne(filter, adInfo, new UpdateOptions { IsUpsert = false });
            }
            

            //Update Campaign with AdMedia
            /*if (id != null)
            {
                var builder = Builders<Campaign>.Filter;
                var filter = builder.Eq("_id", ObjectId.Parse(adInfo.CampaignId));
                var query = _dbContext.Campaigns.Find(filter).FirstOrDefault();

                if (query != null)
                {
                    var update = Builders<Campaign>.Update.Set("AdMedia", adInfo.AdMedia).CurrentDate("TimeUpdated");
                    _dbContext.Campaigns.UpdateOne(filter, update);
                }
            }*/
            return RedirectToAction("CampaignAd");
        }

        // GET: GenerateCards
        public ActionResult GenerateCards()
        {
            var generateCardsView = new CardViewModel();

            return View(generateCardsView);
        }

        public ActionResult GenerateCardsDetails(string id)
        {
            var filter = Builders<CodeCard>.Filter.Eq("_id", ObjectId.Parse(id));

            var codecard = _dbContext.CodeCard.Find(filter).First();

            return View("GenerateCardsDetails", codecard);
        }

        [HttpPost]
        public ActionResult GenerateCards(AdInfo adInfo, CardViewModel cardViewModel)
        {
            var fileName = "";
            int Quantity;
            bool qty = int.TryParse(cardViewModel.CardQuantity, out Quantity);
            adInfo.CampaignId = cardViewModel.AdInfo.CampaignId;
            adInfo.AdCountry = cardViewModel.AdInfo.AdCountry;
            if (!cardViewModel.ImageUrl.IsNullOrWhiteSpace())
            {
                //Get The File Posted
                HttpPostedFileBase image = Request.Files["ImageUrl"];
                //Generate a filename to store the file on the server
                if (image != null)
                {
                    var imageTypes = new[] { "jpg", "jpeg", "png" };
                    var extension = Path.GetExtension(Path.GetFileName(image.FileName));
                    if (extension != null)
                    {
                        var fileExt = extension.Substring(1).ToLower();
                        if (imageTypes.Contains(fileExt))
                        {
                            fileName = "ctemplate." + fileExt;
                            var imagePath = Path.Combine(Server.MapPath("~/assets/image"), fileName);

                            // store the uploaded file on the file system
                            image.SaveAs(imagePath);
                        }
                    }
                }
            }

            //Select All Codes
            var builder = Builders<CampaignCode>.Filter;
            var filter = builder.Eq("CampaignId", adInfo.CampaignId) & builder.Ne("Printed", true);
            var result = _dbContext.CampaignCodes.Find(filter).ToList();

            if (result.Count() >= Quantity && !Quantity.Equals(null))
            {
                //Grab Image Template
                FileInfo fileInfo = new FileInfo(Path.Combine(Server.MapPath("~/assets/image"), fileName));

                List<int> cardList = new List<int>();

                using (MagickImageCollection pdfCollection = new MagickImageCollection())
                {
                    for (int i = 0; i < Quantity; i = i + 8)
                    {
                        if (Quantity - i >= 8 || Quantity - i == 0)
                        {
                            cardList.Add(i);
                            MagickImage card = new MagickImage(fileInfo);
                            Drawables cDrawable = new Drawables();
                            cDrawable.FontPointSize(20);
                            cDrawable.FillColor(MagickColors.White);
                            //cDrawable.StrokeColor(MagickColors.White);
                            cDrawable.Font("Segoe UI");
                            cDrawable.TextAlignment(TextAlignment.Center);
                            cDrawable.TextInterwordSpacing(2);

                            //Write Code 1
                            cDrawable.Text(187, 172, result.ElementAt(i).Code);
                            cDrawable.Draw(card);
                            filter = builder.Eq("Code", result.ElementAt(i).Code);
                            var update1 = Builders<CampaignCode>.Update.Set("Printed", true).CurrentDate("TimeUpdated");
                            _dbContext.CampaignCodes.UpdateOne(filter, update1);

                            //Write Code 2
                            cDrawable.Text(513, 172, result.ElementAt(i + 1).Code);
                            cDrawable.Draw(card);
                            filter = builder.Eq("Code", result.ElementAt(i + 1).Code);
                            var update2 = Builders<CampaignCode>.Update.Set("Printed", true).CurrentDate("TimeUpdated");
                            _dbContext.CampaignCodes.UpdateOne(filter, update2);

                            //Write Code 3
                            cDrawable.Text(187, 384, result.ElementAt(i + 2).Code);
                            cDrawable.Draw(card);
                            filter = builder.Eq("Code", result.ElementAt(i + 2).Code);
                            var update3 = Builders<CampaignCode>.Update.Set("Printed", true).CurrentDate("TimeUpdated");
                            _dbContext.CampaignCodes.UpdateOne(filter, update3);

                            //Write Code 4
                            cDrawable.Text(513, 384, result.ElementAt(i + 3).Code);
                            cDrawable.Draw(card);
                            filter = builder.Eq("Code", result.ElementAt(i + 3).Code);
                            var update4 = Builders<CampaignCode>.Update.Set("Printed", true).CurrentDate("TimeUpdated");
                            _dbContext.CampaignCodes.UpdateOne(filter, update4);

                            //Write Code 5
                            cDrawable.Text(187, 600, result.ElementAt(i + 4).Code);
                            cDrawable.Draw(card);
                            filter = builder.Eq("Code", result.ElementAt(i + 4).Code);
                            var update5 = Builders<CampaignCode>.Update.Set("Printed", true).CurrentDate("TimeUpdated");
                            _dbContext.CampaignCodes.UpdateOne(filter, update5);

                            //Write Code 6
                            cDrawable.Text(513, 600, result.ElementAt(i + 5).Code);
                            cDrawable.Draw(card);
                            filter = builder.Eq("Code", result.ElementAt(i + 5).Code);
                            var update6 = Builders<CampaignCode>.Update.Set("Printed", true).CurrentDate("TimeUpdated");
                            _dbContext.CampaignCodes.UpdateOne(filter, update6);

                            //Write Code 7
                            cDrawable.Text(187, 812, result.ElementAt(i + 6).Code);
                            cDrawable.Draw(card);
                            filter = builder.Eq("Code", result.ElementAt(i + 6).Code);
                            var update7 = Builders<CampaignCode>.Update.Set("Printed", true).CurrentDate("TimeUpdated");
                            _dbContext.CampaignCodes.UpdateOne(filter, update7);

                            //Write Code 8
                            cDrawable.Text(513, 812, result.ElementAt(i + 7).Code);
                            cDrawable.Draw(card);
                            filter = builder.Eq("Code", result.ElementAt(i + 7).Code);
                            var update8 = Builders<CampaignCode>.Update.Set("Printed", true).CurrentDate("TimeUpdated");
                            _dbContext.CampaignCodes.UpdateOne(filter, update8);


                            //Write the final image
                            System.IO.Directory.CreateDirectory(
                                Server.MapPath("~/assets/image/" + cardViewModel.AdInfo.CampaignId));
                            card.Write(Path.Combine(
                                Server.MapPath("~/assets/image/" + cardViewModel.AdInfo.CampaignId), i + ".jpg"));
                        }
                        else
                        {
                            //int remainder = Quantity - i;

                            MagickImage card = new MagickImage(fileInfo);
                            Drawables cDrawable = new Drawables();
                            cDrawable.FontPointSize(20);
                            cDrawable.FillColor(MagickColors.White);
                            //cDrawable.StrokeColor(MagickColors.White);
                            cDrawable.Font("Segoe UI");
                            cDrawable.TextAlignment(TextAlignment.Center);
                            cDrawable.TextInterwordSpacing(2);

                            //Write Code 1
                            cDrawable.Text(157, 172, result.ElementAt(i).Code);
                            cDrawable.Draw(card);
                            filter = builder.Eq("Code", result.ElementAt(i).Code);
                            var update1 = Builders<CampaignCode>.Update.Set("Printed", true).CurrentDate("TimeUpdated");
                            _dbContext.CampaignCodes.UpdateOne(filter, update1);

                            if (++i < Quantity)
                            {
                                //Write Code 2
                                cDrawable.Text(513, 172, result.ElementAt(i + 1).Code);
                                cDrawable.Draw(card);
                                filter = builder.Eq("Code", result.ElementAt(i + 1).Code);
                                var update2 =
                                    Builders<CampaignCode>.Update.Set("Printed", true).CurrentDate("TimeUpdated");
                                _dbContext.CampaignCodes.UpdateOne(filter, update2);

                                if (++i < Quantity)
                                {
                                    //Write Code 3
                                    cDrawable.Text(187, 384, result.ElementAt(i + 2).Code);
                                    cDrawable.Draw(card);
                                    filter = builder.Eq("Code", result.ElementAt(i + 2).Code);
                                    var update3 =
                                        Builders<CampaignCode>.Update.Set("Printed", true).CurrentDate("TimeUpdated");
                                    _dbContext.CampaignCodes.UpdateOne(filter, update3);

                                    if (++i < Quantity)
                                    {
                                        //Write Code 4
                                        cDrawable.Text(513, 384, result.ElementAt(i + 3).Code);
                                        cDrawable.Draw(card);
                                        filter = builder.Eq("Code", result.ElementAt(i + 3).Code);
                                        var update4 =
                                            Builders<CampaignCode>.Update.Set("Printed", true)
                                                .CurrentDate("TimeUpdated");
                                        _dbContext.CampaignCodes.UpdateOne(filter, update4);

                                        if (i++ < Quantity)
                                        {
                                            //Write Code 5
                                            cDrawable.Text(187, 600, result.ElementAt(i + 4).Code);
                                            cDrawable.Draw(card);
                                            filter = builder.Eq("Code", result.ElementAt(i + 4).Code);
                                            var update5 =
                                                Builders<CampaignCode>.Update.Set("Printed", true)
                                                    .CurrentDate("TimeUpdated");
                                            _dbContext.CampaignCodes.UpdateOne(filter, update5);

                                            if (i++ < Quantity)
                                            {
                                                //Write Code 6
                                                cDrawable.Text(513, 600, result.ElementAt(i + 5).Code);
                                                cDrawable.Draw(card);
                                                filter = builder.Eq("Code", result.ElementAt(i + 5).Code);
                                                var update6 =
                                                    Builders<CampaignCode>.Update.Set("Printed", true)
                                                        .CurrentDate("TimeUpdated");
                                                _dbContext.CampaignCodes.UpdateOne(filter, update6);

                                                if (i++ < Quantity)
                                                {
                                                    //Write Code 7
                                                    cDrawable.Text(187, 812, result.ElementAt(i + 6).Code);
                                                    cDrawable.Draw(card);
                                                    filter = builder.Eq("Code", result.ElementAt(i + 6).Code);
                                                    var update7 =
                                                        Builders<CampaignCode>.Update.Set("Printed", true)
                                                            .CurrentDate("TimeUpdated");
                                                    _dbContext.CampaignCodes.UpdateOne(filter, update7);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            //Write the final image
                            System.IO.Directory.CreateDirectory(
                                Server.MapPath("~/assets/image/" + cardViewModel.AdInfo.CampaignId));
                            card.Write(Path.Combine(
                                Server.MapPath("~/assets/image/" + cardViewModel.AdInfo.CampaignId), i + ".jpg"));
                            cardList.Add(i);
                        }
                    }

                    //Create PDF from Images
                    for (int p = 0; p <= cardList.Max(); p++)
                    {
                        if (cardList.Contains(p))
                        {
                            if (
                                System.IO.File.Exists(
                                    Path.Combine(Server.MapPath("~/assets/image/" + cardViewModel.AdInfo.CampaignId),
                                        p + ".jpg")))
                            {
                                pdfCollection.Add(
                                    new MagickImage(
                                        Path.Combine(
                                            Server.MapPath("~/assets/image/" + cardViewModel.AdInfo.CampaignId),
                                            p + ".jpg")));
                            }
                        }

                    }

                    pdfCollection.Write(Path.Combine(
                        Server.MapPath("~/assets/image/" + cardViewModel.AdInfo.CampaignId), "CardImages.pdf"));
                }

                var generateCardsView = new CardViewModel()
                {
                    DownloadUrl = cardViewModel.AdInfo.CampaignId + "/CardImages.pdf",
                };

                //Insert into CodeCard
                var codeCard = new CodeCard();
                codeCard.CampaignId = cardViewModel.AdInfo.CampaignId;
                codeCard.Quantity = cardViewModel.CardQuantity;
                codeCard.TimeCreated = DateTime.Now;
                codeCard.TimeUpdated = DateTime.Now;
                _dbContext.CodeCard.InsertOneAsync(codeCard);

                return View(generateCardsView);
            }
            else
            {
                string errMsg = "You have only " + result.Count + " outstanding codes.";
                var generateCardsView = new CardViewModel()
                {
                    ErrorMsg = errMsg
                };
                return View(generateCardsView);
            }

            
        }

        // GET: Passitions
        public ActionResult Passiton()
        {
            return View();
        }

        public ActionResult PassitonDetails(string id)
        {
            var filter = Builders<Models.PassItOn>.Filter.Eq("_id", ObjectId.Parse(id));

            var business = _dbContext.PassItOns.Find(filter).First();

            return View("PassitonDetails", business);
        }

        public ActionResult CodeFailure()
        {
            return View();
        }

        public ActionResult CodeFailureDetails(string id)
        {
            var filter = Builders<CodeFailure>.Filter.Eq("_id", ObjectId.Parse(id));

            var business = _dbContext.CodeFailure.Find(filter).First();

            return View("CodeFailureDetails", business);
        }
    }
}