using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infobip.Api.Client;
using Infobip.Api.Model.Sms.Mt.Send;
using Infobip.Api.Model.Sms.Mt.Send.Textual;

namespace PassItOn.Models
{
    class SingleTextualSmsExample : Example
    {
        public override Task RunExampleAsync()
        {
            throw new NotImplementedException();
        }

        public async Task RunExampleAsync(List<string> to)
        {
            string messageId = await SendSingleTextualSmsAsync(to);

            System.Threading.Thread.Sleep(2000);

            await GetSmsReportAsync(messageId);
        }

        private static async Task<string> SendSingleTextualSmsAsync(List<string> to)
        {
            Console.WriteLine("-------------------------------");
            Console.WriteLine("Sending single textual message...");

            SendSingleTextualSms smsClient = new SendSingleTextualSms(BASIC_AUTH_CONFIGURATION);
            SMSTextualRequest request = new SMSTextualRequest
            {
                From = FROM,
                To = to,
                Text = MESSAGE_TEXT
            };
            SMSResponse smsResponse = await smsClient.ExecuteAsync(request);

            Console.WriteLine("Sending single textual message complete.");

            SMSResponseDetails sentMessageInfo = smsResponse.Messages[0];
            Console.WriteLine("-------------------------------");
            Console.WriteLine("Message ID: " + sentMessageInfo.MessageId);
            Console.WriteLine("Receiver: " + sentMessageInfo.To);
            Console.WriteLine("Message status: " + sentMessageInfo.Status.Name);
            Console.WriteLine("-------------------------------");

            return sentMessageInfo.MessageId;
        }
    }
}
