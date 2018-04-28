using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PassItOn.Models;

namespace PassItOn.ViewModels
{
    public class CodeViewModel
    {
        public IEnumerable<SelectListItem> CountryList { get; set; }
        public IEnumerable<SelectListItem> CampaignList { get; set; }
        public IEnumerable<SelectListItem> BusinessList { get; set; }
        public IEnumerable<CampaignCode> CampaignCodes { get; set; }
        public CampaignCode CampaignCode { get; set; }
        public Campaign Campaign { get; set; }
    }
}