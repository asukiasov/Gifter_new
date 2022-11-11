using System.Collections.Generic;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Abstractions
{
    public interface IEmail
    {
        #region Methods        
        Task<EmailSendResult> Send(string To, string Subject, string Body, string ReplyTo, IEnumerable<EmailAttachment> Attachments, IEnumerable<string> CCs, IEnumerable<string> BCCs);
        #endregion
    }

    public class EmailAttachment
    {
        #region Properties
        public string Filename { get; set; }
        public byte[] FileBytes { get; set; }
        #endregion
    }

    public class EmailSendResult
    {
        #region Properties
        public bool IsSent { get; set; }
        public string EmailMessageID { get; set; }
        public string ResponseContent { get; set; }
        public long? EmailLogID { get; set; }
        #endregion
    }
}
