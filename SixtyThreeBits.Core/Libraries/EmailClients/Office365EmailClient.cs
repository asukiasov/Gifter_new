using SixtyThreeBits.Core.Abstractions;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Libraries.EmailClients
{
    public class Office365EmailClient : IEmailClient
    {
        #region Properties        
        readonly MicrosoftGraphServices _service;
        #endregion

        #region Constructors
        public Office365EmailClient(string tenant, string clientID, string clientSecret, string microsoftUserID)
        {
            _service = new MicrosoftGraphServices(
                tenant: tenant,
                clientID: clientID,
                clientSecret: clientSecret,
                microsoftUserID: microsoftUserID
            );
        }
        #endregion

        #region Methods
        public async Task<EmailSendResultDTO> SendEmail(string emailTo, string subject, string body, string replyTo = null, IEnumerable<EmailAttachmentDTO> attachments = null, IEnumerable<string> cc = null, IEnumerable<string> bcc = null)
        {
            var result = new EmailSendResultDTO();
            var sendEmailResult = await _service.SendEmail(
                emailTo: emailTo,
                subject: subject,
                body: body,
                replyTo: replyTo,
                attachments: attachments,
                cc: cc,
                bcc: bcc
            );
            if (sendEmailResult.IsSuccess)
            {
                result.IsSent = true;
                result.EmailMessageID = Guid.NewGuid().ToString();
            }
            result.ResponseMessage = sendEmailResult.ResponseContent;
            return result;
        }
        #endregion
    }
}
