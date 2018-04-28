using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using PassItOn.Models;

namespace PassItOn.ViewModels
{
    public class PassItOnViewModel
    {
        public PassAd PassAd;

        public IEnumerable<PassAd> PassItAds;

        public List<PassAd> PassAds;
    }
}