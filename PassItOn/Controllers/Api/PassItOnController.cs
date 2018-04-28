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
    public class PassItOnController : ApiController
    {
        private readonly DataAccess _dataAccess;

        public PassItOnController()
        {
            _dataAccess = new DataAccess();
        }

        // POST api/PassItOn/
        [HttpGet]
        public async Task<object> PassItOn()
        {
            return await _dataAccess.GetAllPassItons();
        }

        // POST api/PassItOn/id
        [HttpGet]
        public async Task<object> PassItOn(string passitCode)
        {
            return await _dataAccess.GetPassitonAd(passitCode);
        }

        // POST api/passiton/
        [HttpPost]
        public async Task<string> PassItOn(string passitCode, Models.PassItOn passItOn)
        {
            if (await _dataAccess.NumberCheck(passItOn.MobileNo))
            {
                var campaignInfo = await _dataAccess.PassItOn(passitCode, passItOn.MobileNo);

                if (campaignInfo != null)
                {
                    var passItOnCheck = await _dataAccess.PassItOnCheck(passitCode, passItOn.MobileNo, campaignInfo.Id.ToString());
                    if (passItOnCheck == null)
                    {
                        string network = passItOn.MobileNo.Substring(0, 3);

                        if (campaignInfo.CampaignNetwork.Contains(network))
                        {
                            double prize;
                            if (double.TryParse(campaignInfo.CampaignPrize, out prize))
                            {
                                //Proceed with Payment

                                //Save Checkout Info
                                await _dataAccess.AddPassItOn(passitCode, passItOn.MobileNo, campaignInfo.Id.ToString(), campaignInfo.CampaignPrize, network);

                                //Update the CodeLimit
                                await _dataAccess.UpdateCampaignCode(passitCode, campaignInfo.UsageLimit);

                                //Send Text Message
                                var mobileNo = "233" + passItOn.MobileNo.TrimStart(Convert.ToChar("0"));
                                //List<string> phone = new List<string>(1) { mobileNo };
                                //new SingleTextualSmsExample().RunExampleAsync(phone).Wait(5000);
                                new AdvancedSmsExample().RunExampleAsync(mobileNo).Wait(5000);
                            }
                            return "Congratulations, you have successfully claimed your reward.";
                        }
                        return "Sorry, your mobile network is not supported for this campaign";
                    }
                    return "Sorry, you have already used this code.";
                }
                return await _dataAccess.CodeFailure(passItOn.MobileNo);
            }
            return "You phone number has been blocked for 24 hours due to invalid code entries";
        }
    }

    // POST api/businesses/1Q1Q2W2W8U/0100101101
    /*[HttpPost]
    public async Task<string> PassItOn(string passitCode, string mobileNo)
    {
        var campaignInfo = await _dataAccess.PassItOn(passitCode);

        if (campaignInfo != null)
        {
            string network = mobileNo.Substring(0, 3);

            if (campaignInfo.CampaignNetwork.Contains(network))
            {
                //Proceed with Payment

                //Update the CodeLimit
                await _dataAccess.UpdateCampaignCode(passitCode, campaignInfo.UsageLimit);

                return "Congratulations, you have successfully claimed your reward.";
            }
            else
            {
                return "Sorry, your mobile network is not supported for this campaign";
            }
        }
        else
        {
            return "Invalid Code";
        }
    }*/
}
