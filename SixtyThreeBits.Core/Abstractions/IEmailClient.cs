using SixtyThreeBits.Core.Infrastructure.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Abstractions
{
    public interface IEmailClient
    {
        #region Methods        
        Task<EmailSendResultDTO> SendEmail(string emailTo, string subject, string body, string replyTo = null, IEnumerable<EmailAttachmentDTO> attachments = null, IEnumerable<string> cc = null, IEnumerable<string> bcc = null);
        #endregion
    }        
}
