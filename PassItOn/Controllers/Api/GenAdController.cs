using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using PassItOn.Models;
using PassItOn.ViewModels;

namespace PassItOn.Controllers.Api
{
    public class GenAdController : ApiController
    {
        private readonly DataAccess _dataAccess;

        public GenAdController()
        {
            _dataAccess = new DataAccess();
        }

        // GET: api/GenAds
        [HttpGet]
        public Task<IEnumerable<Ad>> Get()
        {
            return GetGenAds();
        }

        private async Task<IEnumerable<Ad>> GetGenAds()
        {
            return await _dataAccess.GetAllGenAds();
        }

        // GET: api/GenAd/1
        [HttpGet]
        public Task<AdInfo> Get(string id)
        {
            return GetGenAd(id);
        }

        private async Task<AdInfo> GetGenAd(string id)
        {
            return await _dataAccess.GetGenAds(id) ?? new AdInfo();
        }

        // POST: api/updateGenAds
        [HttpPost]
        public async void UpdateGenAds(AdInfo adInfo)
        {
            await _dataAccess.UpdateGenAd(adInfo);
        }

        //DELETE api/removeGenAd/1
        [HttpDelete]
        public async Task<bool> RemoveGenAd(string id)
        {
            return await _dataAccess.RemoveGenAd(id);
        }
    }
}