using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using MongoDB.Bson;
using PassItOn.Models;

namespace PassItOn.Controllers.Api
{
    public class BusinessesController : ApiController
    {
        private readonly DataAccess _dataAccess;

        public BusinessesController()
        {
            _dataAccess = new DataAccess();
        }

        

        // GET: api/businesses
        [HttpGet]
        public Task<IEnumerable<Business>> Get()
        {
            return GetBusinesses();
        }

        private async Task<IEnumerable<Business>> GetBusinesses()
        {

            return await _dataAccess.GetAllBusinesses();
        }

        // GET: api/businesses/1
        [HttpGet]
        public Task<Business> Get(string id)
        {
            return GetBusiness(id);
        }

        private async Task<Business> GetBusiness(string id)
        {
            return await _dataAccess.GetBusiness(id) ?? new Business();
        }

        // POST: api/businesses
        [HttpPost]
        public async void AddBusiness(Business business)
        {
            await _dataAccess.AddBusiness(business);
        }


        //PUT api/businesses/1
        [HttpPut]
        public async void UpdateBusiness(string id, Business business)
        {
            var recId = new ObjectId(id);
            await _dataAccess.UpdateBusiness(recId, business);
        }

        //DELETE api/businesses/1
        [HttpDelete]
        public async Task<bool> RemoveBusiness(string id)
        {
            return await _dataAccess.RemoveBusiness(id);
        }
    }
}
