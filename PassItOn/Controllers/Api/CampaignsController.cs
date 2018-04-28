using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using MongoDB.Bson;
using PassItOn.Models;
using PassItOn.ViewModels;

namespace PassItOn.Controllers.Api
{
    public class CampaignsController : ApiController
    {
        private readonly DataAccess _dataAccess;

        public CampaignsController()
        {
            _dataAccess = new DataAccess();
        }

        // GET: api/campaigns
        [HttpGet]
        public Task<IEnumerable<Campaign>> Get()
        {
            return GetCampaigns();
        }

        private async Task<IEnumerable<Campaign>> GetCampaigns()
        {

            return await _dataAccess.GetAllCampaigns();
        }

        // GET: api/campaigns/1
        [HttpGet]
        public Task<Campaign> Get(string id)
        {
            return GetCampaign(id);
        }

        private async Task<Campaign> GetCampaign(string id)
        {
            return await _dataAccess.GetCampaign(id) ?? new Campaign();
        }

        // POST: api/campaigns
        [HttpPost]
        public async void AddCampaign(CampaignViewModel campaignViewModel)
        {
            await _dataAccess.AddCampaign(campaignViewModel);
        }


        //PUT api/campaigns/1
        [HttpPut]
        public async void UpdateCampaign(string id, Campaign campaign)
        {
            var recId = new ObjectId(id);
            await _dataAccess.UpdateCampaign(recId, campaign);
        }

        //DELETE api/campaigns/1
        [HttpDelete]
        public async Task<bool> RemoveCampaign(string id)
        {
            return await _dataAccess.RemoveCampaign(id);
        }
    }
}
