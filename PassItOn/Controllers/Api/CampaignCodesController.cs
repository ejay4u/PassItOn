using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using PassItOn.Models;
using PassItOn.ViewModels;

namespace PassItOn.Controllers.Api
{
    public class CampaignCodesController : ApiController
    {
        private readonly DataAccess _dataAccess;

        public CampaignCodesController()
        {
            _dataAccess = new DataAccess();
        }

        // GET: api/campaignCodes
        [HttpGet]
        public Task<IEnumerable<CampaignCode>> Get()
        {
            return GetCampaignCodes();
        }

        private async Task<IEnumerable<CampaignCode>> GetCampaignCodes()
        {
            return await _dataAccess.GetAllCampaignCodes();
        }

        // POST: api/campaignCodes
        [HttpPost]
        public async void AddCampaign(CodeViewModel codeViewModel)
        {
            await _dataAccess.AddCampaignCode(codeViewModel);
        }

        //DELETE api/campaignCodes/1
        [HttpDelete]
        public async Task<bool> RemoveCampaignCode(string id)
        {
            return await _dataAccess.RemoveCampaignCode(id);
        }
    }
}
