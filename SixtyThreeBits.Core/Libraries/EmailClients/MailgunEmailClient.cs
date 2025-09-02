using SixtyThreeBits.Core.Abstractions;
using SixtyThreeBits.Core.Infrastructure.Services;
using SixtyThreeBits.Core.Libraries.EmailClients.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Libraries.EmailClients
{
    public class MailgunEmailClient : IEmailClient
    {
        #region Properties
        MailgunService _service;
        #endregion

        #region Constructors
        public MailgunEmailClient(string baseUrl, string mailgunApiKey, string mailgunDomain, string mailgunFrom)
        {
            _service = new MailgunService(
                baseUrl: baseUrl,
                mailgunApiKey: mailgunApiKey,
                mailgunDomain: mailgunDomain,
                mailgunFrom: mailgunFrom
            );
        }
        #endregion

        #region Methods
        public async Task<EmailSendResultDTO> SendEmail(string emailTo, string subject, string body, string replyTo = null, IEnumerable<EmailAttachmentDTO> attachments = null, IEnumerable<string> cc = null, IEnumerable<string> bcc = null)
        {
            var result = new EmailSendResultDTO();
            var sendResult = await _service.SendEmail(
                emailTo: emailTo,
                subject: subject,
                body: body,
                replyTo: replyTo,
                attachments: attachments,
                cc: cc,
                bcc: bcc
            );
            if (sendResult.IsSuccess)
            {
                result.IsSent = true;
                result.EmailMessageID = sendResult.EmailMessageID;
            }
            result.ResponseMessage = sendResult.ResponseContent;

            return result;
        }
        #endregion
    }
}
