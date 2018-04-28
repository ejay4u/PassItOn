using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using PassItOn.Models;
using PusherServer;
using WebGrease.Css.Ast;

namespace PassItOn.Controllers.Api
{
    public class CodeFailureController : ApiController
    {
        private readonly DataAccess _dataAccess;

        public CodeFailureController()
        {
            _dataAccess = new DataAccess();
        }

        // POST api/PassItOn/
        [HttpGet]
        public async Task<object> CodeFailure()
        {
            return await _dataAccess.GetCodeFailures();
        }

        // POST api/PassItOn/id
        [HttpGet]
        public async Task<object> CodeFailure(string codeFailure)
        {
            return await _dataAccess.GetCodeFailure(codeFailure);
        }

        //DELETE api/gencards/1
        [HttpDelete]
        public async Task<bool> RemoveCodeFailure(string id)
        {
            return await _dataAccess.RemoveCodeFailure(id);
        }

    }
}
