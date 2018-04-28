using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PassItOn.Models;

namespace PassItOn.ViewModels
{
    public class AdViewModel
    {
        public string ImageUrl { get; set; }
        public string AudioUrl { get; set; }
        public string VideoUrl { get; set; }
        public string VideoHost { get; set; }
        public string Id { get; set; }
        public AdInfo AdInfo { get; set; }
        public IEnumerable<SelectListItem> VideoHostList { get; set; }
        public IEnumerable<SelectListItem> CountryList { get; set; }
        public IEnumerable<SelectListItem> CampaignList { get; set; }
        public IEnumerable<SelectListItem> AddTypeList { get; set; }
    }
}