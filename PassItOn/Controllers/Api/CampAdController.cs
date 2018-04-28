using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using PassItOn.Models;

namespace PassItOn.Controllers.Api
{
    public class CampAdController : ApiController
    {
        private readonly DataAccess _dataAccess;

        public CampAdController()
        {
            _dataAccess = new DataAccess();
        }

        // GET: api/CampAds
        [HttpGet]
        public Task<IEnumerable<Ad>> Get()
        {
            return GetCampAds();
        }

        private async Task<IEnumerable<Ad>> GetCampAds()
        {
            return await _dataAccess.GetAllCampAds();
        }

        // GET: api/CampAd/1
        [HttpGet]
        public Task<AdInfo> Get(string id)
        {
            return GetCampAd(id);
        }

        private async Task<AdInfo> GetCampAd(string id)
        {
            return await _dataAccess.GetCampAd(id) ?? new AdInfo();
        }

        // POST: api/updateCampAds
        [HttpPost]
        public async void UpdateCampAds(AdInfo adInfo)
        {
            await _dataAccess.UpdateCampAd(adInfo);
        }

        //DELETE api/removeCampAd/1
        [HttpDelete]
        public async Task<bool> RemoveCampAd(string id)
        {
            return await _dataAccess.RemoveCampAd(id);
        }
    }
}