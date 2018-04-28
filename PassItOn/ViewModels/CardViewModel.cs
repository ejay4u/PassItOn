using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PassItOn.Models;

namespace PassItOn.ViewModels
{
    public class CardViewModel
    {
        public string ImageUrl { get; set; }
        public string CardQuantity { get; set; }
        public string DownloadUrl { get; set; }
        public string ErrorMsg { get; set; }
        public AdInfo AdInfo { get; set; }
        public CodeCard CodeCard { get; set; }
        public IEnumerable<SelectListItem> CountryList { get; set; }
        public IEnumerable<SelectListItem> CampaignList { get; set; }
    }
}