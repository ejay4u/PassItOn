using System.Collections.Generic;
using MaxMind.GeoIP2;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Microsoft.Ajax.Utilities;
using MongoDB.Bson;
using MongoDB.Driver;
using PassItOn.Models;
using PassItOn.ViewModels;

namespace PassItOn.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        readonly MongoContext _dbContext;
        public HomeController()
        {
            _dbContext = new MongoContext();
        }

        public ActionResult Index()
        {
            //Only For Development
            //Get Country AdInfo
            var builder = Builders<AdInfo>.Filter;
            var filter = builder.Type("CampaignId", BsonType.Null) & builder.Eq("AdCountry", "Ghana") &
                         builder.Eq("AdStatus", true);
            var passAd = _dbContext.AdInfos.Find(filter).ToList();

            List<PassAd> pAd = new List<PassAd>();
            foreach (var adInfo in passAd)
            {
                if (adInfo.AdType.Equals("Background-Image"))
                {
                    var ad = new PassAd
                    {
                        AdType = adInfo.AdType,
                        ImageUrl = adInfo.AdMedia.GetElement("ImageUrl").Value.ToString()
                    };
                    pAd.Add(ad);
                }
                else if (adInfo.AdType.Equals("Background-Audio"))
                {
                    var ad = new PassAd
                    {
                        AdType = adInfo.AdType,
                        ImageUrl = adInfo.AdMedia.GetElement("AudioUrl").Value.ToString()
                    };
                    pAd.Add(ad);
                }
                else if (adInfo.AdType.Equals("Background-Video(mute)"))
                {
                    var ad = new PassAd
                    {
                        AdType = adInfo.AdType,
                        VideoUrl = adInfo.AdMedia.GetElement("VideoUrl").Value.ToString(),
                        VideoHost = adInfo.AdMedia.GetElement("VideoHost").Value.ToString()
                    };
                    pAd.Add(ad);
                }
                else if (adInfo.AdType.Equals("PassItCode-Image"))
                {
                    var ad = new PassAd
                    {
                        AdType = adInfo.AdType,
                        ImageUrl = adInfo.AdMedia.GetElement("ImageUrl").Value.ToString()
                    };
                    pAd.Add(ad);
                }
                else if (adInfo.AdType.Equals("PassItCode-Video"))
                {
                    var ad = new PassAd
                    {
                        AdType = adInfo.AdType,
                        VideoUrl = adInfo.AdMedia.GetElement("VideoUrl").Value.ToString(),
                        VideoHost = adInfo.AdMedia.GetElement("VideoHost").Value.ToString()
                    };
                    pAd.Add(ad);
                }
                else if (adInfo.AdType.Equals("PhoneNo-Image(Default)"))
                {
                    var ad = new PassAd
                    {
                        AdType = adInfo.AdType,
                        ImageUrl = adInfo.AdMedia.GetElement("ImageUrl").Value.ToString()
                    };
                    pAd.Add(ad);
                }
                else if (adInfo.AdType.Equals("PhoneNo-Video(Default)"))
                {
                    var ad = new PassAd
                    {
                        AdType = adInfo.AdType,
                        VideoUrl = adInfo.AdMedia.GetElement("VideoUrl").Value.ToString(),
                        VideoHost = adInfo.AdMedia.GetElement("VideoHost").Value.ToString()
                    };
                    pAd.Add(ad);
                }

            }

            var passitonViewModel = new PassItOnViewModel()
            {
                PassItAds = pAd
            };

            return View(passitonViewModel);
            /*var xip = Request.Headers["X-Forwarded-For"].Split(new char[] { ',' }).FirstOrDefault();
            using (var reader = new DatabaseReader(HostingEnvironment.MapPath("~/assets/GeoIP2/GeoLite2-City.mmdb")))
            {
                // Get the city from the IP Address
                var city = reader.City(xip);
            }*/

            //Uncomment Before Hosting
            /*using (var reader = new DatabaseReader(HostingEnvironment.MapPath("~/assets/GeoIP2/GeoLite2-City.mmdb")))
            {
                // Determine the IP Address of the request
                var ipAddress = Request.UserHostAddress;

                // Get the city from the IP Address
                var city = reader.City(ipAddress);

                if (city != null)
                {
                    //Get Country AdInfo
                    var builder = Builders<AdInfo>.Filter;
                    var filter = builder.Type("CampaignId", BsonType.Null) & builder.Eq("AdCountry", city.Country) &
                                 builder.Eq("AdStatus", true);
                    var passAd = _dbContext.AdInfos.Find(filter).ToList();

                    List<PassAd> pAd = new List<PassAd>();
                    foreach (var adInfo in passAd)
                    {
                        if (adInfo.AdType.Equals("Background-Image"))
                        {
                            var ad = new PassAd
                            {
                                AdType = adInfo.AdType,
                                ImageUrl = adInfo.AdMedia.GetElement("ImageUrl").Value.ToString()
                            };
                            pAd.Add(ad);
                        }
                        else if (adInfo.AdType.Equals("Background-Audio"))
                        {
                            var ad = new PassAd
                            {
                                AdType = adInfo.AdType,
                                ImageUrl = adInfo.AdMedia.GetElement("AudioUrl").Value.ToString()
                            };
                            pAd.Add(ad);
                        }
                        else if (adInfo.AdType.Equals("Background-Video(mute)"))
                        {
                            var ad = new PassAd
                            {
                                AdType = adInfo.AdType,
                                VideoUrl = adInfo.AdMedia.GetElement("VideoUrl").Value.ToString(),
                                VideoHost = adInfo.AdMedia.GetElement("VideoHost").Value.ToString()
                            };
                            pAd.Add(ad);
                        }
                        else if (adInfo.AdType.Equals("PassItCode-Image"))
                        {
                            var ad = new PassAd
                            {
                                AdType = adInfo.AdType,
                                ImageUrl = adInfo.AdMedia.GetElement("ImageUrl").Value.ToString()
                            };
                            pAd.Add(ad);
                        }
                        else if (adInfo.AdType.Equals("PassItCode-Video"))
                        {
                            var ad = new PassAd
                            {
                                AdType = adInfo.AdType,
                                VideoUrl = adInfo.AdMedia.GetElement("VideoUrl").Value.ToString(),
                                VideoHost = adInfo.AdMedia.GetElement("VideoHost").Value.ToString()
                            };
                            pAd.Add(ad);
                        }
                        else if (adInfo.AdType.Equals("PhoneNo-Image(Default)"))
                        {
                            var ad = new PassAd
                            {
                                AdType = adInfo.AdType,
                                ImageUrl = adInfo.AdMedia.GetElement("ImageUrl").Value.ToString()
                            };
                            pAd.Add(ad);
                        }
                        else if (adInfo.AdType.Equals("PhoneNo-Video(Default)"))
                        {
                            var ad = new PassAd
                            {
                                AdType = adInfo.AdType,
                                VideoUrl = adInfo.AdMedia.GetElement("VideoUrl").Value.ToString(),
                                VideoHost = adInfo.AdMedia.GetElement("VideoHost").Value.ToString()
                            };
                            pAd.Add(ad);
                        }

                    }

                    var passitonViewModel = new PassItOnViewModel()
                    {
                        PassItAds = pAd
                    };

                    return View(passitonViewModel);
                }
                else
                {
                    //Get Country AdInfo
                    var builder = Builders<AdInfo>.Filter;
                    var filter = builder.Type("CampaignId", BsonType.Null) & builder.Eq("AdCountry", "Ghana") &
                                 builder.Eq("AdStatus", true);
                    var passAd = _dbContext.AdInfos.Find(filter).ToList();

                    List<PassAd> pAd = new List<PassAd>();
                    foreach (var adInfo in passAd)
                    {
                        if (adInfo.AdType.Equals("Background-Image"))
                        {
                            var ad = new PassAd
                            {
                                AdType = adInfo.AdType,
                                ImageUrl = adInfo.AdMedia.GetElement("ImageUrl").Value.ToString()
                            };
                            pAd.Add(ad);
                        }
                        else if (adInfo.AdType.Equals("Background-Audio"))
                        {
                            var ad = new PassAd
                            {
                                AdType = adInfo.AdType,
                                ImageUrl = adInfo.AdMedia.GetElement("AudioUrl").Value.ToString()
                            };
                            pAd.Add(ad);
                        }
                        else if (adInfo.AdType.Equals("Background-Video(mute)"))
                        {
                            var ad = new PassAd
                            {
                                AdType = adInfo.AdType,
                                VideoUrl = adInfo.AdMedia.GetElement("VideoUrl").Value.ToString(),
                                VideoHost = adInfo.AdMedia.GetElement("VideoHost").Value.ToString()
                            };
                            pAd.Add(ad);
                        }
                        else if (adInfo.AdType.Equals("PassItCode-Image"))
                        {
                            var ad = new PassAd
                            {
                                AdType = adInfo.AdType,
                                ImageUrl = adInfo.AdMedia.GetElement("ImageUrl").Value.ToString()
                            };
                            pAd.Add(ad);
                        }
                        else if (adInfo.AdType.Equals("PassItCode-Video"))
                        {
                            var ad = new PassAd
                            {
                                AdType = adInfo.AdType,
                                VideoUrl = adInfo.AdMedia.GetElement("VideoUrl").Value.ToString(),
                                VideoHost = adInfo.AdMedia.GetElement("VideoHost").Value.ToString()
                            };
                            pAd.Add(ad);
                        }
                        else if (adInfo.AdType.Equals("PhoneNo-Image(Default)"))
                        {
                            var ad = new PassAd
                            {
                                AdType = adInfo.AdType,
                                ImageUrl = adInfo.AdMedia.GetElement("ImageUrl").Value.ToString()
                            };
                            pAd.Add(ad);
                        }
                        else if (adInfo.AdType.Equals("PhoneNo-Video(Default)"))
                        {
                            var ad = new PassAd
                            {
                                AdType = adInfo.AdType,
                                VideoUrl = adInfo.AdMedia.GetElement("VideoUrl").Value.ToString(),
                                VideoHost = adInfo.AdMedia.GetElement("VideoHost").Value.ToString()
                            };
                            pAd.Add(ad);
                        }

                    }

                    var passitonViewModel = new PassItOnViewModel()
                    {
                        PassItAds = pAd
                    };

                    return View(passitonViewModel);
                }
            }*/
        }

        public ActionResult About()
        {
            //Only For Development
            var xip = Request.Headers["X-Forwarded-For"].Split(new char[] {','}).FirstOrDefault();
            using (var reader = new DatabaseReader(HostingEnvironment.MapPath("~/assets/GeoIP2/GeoLite2-City.mmdb")))
            {
                // Get the city from the IP Address
                var city = reader.City(xip);

                //Get Country AdInfo
                var builder = Builders<AdInfo>.Filter;
                var filter = builder.Type("CampaignId", BsonType.Null) & builder.Eq("AdCountry", city.Country) &
                             builder.Eq("AdStatus", true);
                var passAd = _dbContext.AdInfos.Find(filter).ToList();

                List <PassAd> pAd = new List<PassAd>();
                foreach (var adInfo in passAd)
                {
                    var ad = new PassAd
                    {
                        AdType = adInfo.AdType,
                        ImageUrl = adInfo.AdMedia.GetElement("ImageUrl").Value.ToString(),
                        VideoUrl = adInfo.AdMedia.GetElement("VideoUrl").Value.ToString(),
                        VideoHost = adInfo.AdMedia.GetElement("VideoHost").Value.ToString()
                    };

                    pAd.Add(ad);
                }

                var passitonViewModel = new PassItOnViewModel()
                {
                    PassItAds = pAd
                };

                return View(passitonViewModel);
            }
        }
    }
}