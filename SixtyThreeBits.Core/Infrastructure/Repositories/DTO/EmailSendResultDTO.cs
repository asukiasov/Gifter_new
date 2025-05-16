namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
{
    public class EmailSendResultDTO
    {
        #region Properties
        public bool IsSent { get; set; }
        public string EmailMessageID { get; set; }
        public string ResponseMessage { get; set; }
        #endregion
    }
}
