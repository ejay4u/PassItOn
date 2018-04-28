using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PassItOn.Models
{
    public class PassAd
    {
        public string AdType { get; set; }
        public string ImageUrl { get; set; }
        public string AudioUrl { get; set; }
        public string VideoUrl { get; set; }
        public string VideoHost { get; set; }
    }
}