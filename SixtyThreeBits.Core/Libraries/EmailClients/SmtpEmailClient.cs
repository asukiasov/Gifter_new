using SixtyThreeBits.Core.Abstractions;
using SixtyThreeBits.Core.Libraries;
using SixtyThreeBits.Core.Libraries.EmailClients.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Libraries.EmailClients
{
    public class SmtpEmailClient : IEmailClient
    {
        #region Properties        
        readonly SMTP _service;
        #endregion

        #region Constructors
        public SmtpEmailClient(string smtpAddress, int? smtpPort, string smtpUsername, string smtpPassword, bool? smtpUseSSL, string smtpFromName)
        {
            _service = new SMTP(
                smtpAddress: smtpAddress,
                smtpPort: smtpPort,
                smtpUsername: smtpUsername,
                smtpPassword: smtpPassword,
                smtpUseSSL: smtpUseSSL,
                smtpFromName: smtpFromName
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
            if (sendEmailResult.IsSent)
            {
                result.IsSent = true;
                result.EmailMessageID = Guid.NewGuid().ToString();
            }
            result.ResponseMessage = sendEmailResult.ErrorMessage;
            return result;
        }
        #endregion
    }
}
