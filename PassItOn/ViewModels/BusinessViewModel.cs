using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PassItOn.Models;

namespace PassItOn.ViewModels
{
    public class BusinessViewModel
    {
        public IEnumerable<SelectListItem> CountryList { get; set; }
        public Business Business { get; set; }
        public string Id { get; set; }
    }
}