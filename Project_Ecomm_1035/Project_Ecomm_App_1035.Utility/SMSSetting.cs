using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Project_Ecomm_App_1035.Utility
{
    public class SMSSetting:ISMSsenderServices
    {
        private readonly TwilioSetting _twilio;
        public SMSSetting(IOptions<TwilioSetting> twilio)
        {
            _twilio = twilio.Value;
        }

        public MessageResource Send(string mobileNumber, string body)
        {
            TwilioClient.Init(_twilio.AccountSID, _twilio.AuthToken);
            var result = MessageResource.Create(
                body: body,
                //from:new Twilio.Types.PhoneNumber(_twilio.TwilioPhoneNumber),
                from: _twilio.TwilioPhoneNumber,
                to: mobileNumber
                );
            return result;
        }
    }
}
