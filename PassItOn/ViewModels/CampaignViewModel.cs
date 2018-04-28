using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PassItOn.Models;

namespace PassItOn.ViewModels
{
    public class CampaignViewModel
    {
        public IEnumerable<SelectListItem>BusinessName { get; set; }
        public IEnumerable<SelectListItem> CountryList { get; set; }
        public IEnumerable<SelectListItem> CampaignNetwork { get; set; }
        public IEnumerable<string> SelectedCampaignNetwork { get; set; }
        public IEnumerable<Campaign> CampaignView { get; set; }
        public Campaign Campaign { get; set;}
        public string Id { get; set; }
    }
}