namespace SixtyThreeBits.Core.Infrastructure.DTO
{
    public record EmailSendResultDTO
    {
        #region Properties
        public bool IsSent { get; set; }
        public string EmailMessageID { get; set; }
        public string ResponseMessage { get; set; }
        #endregion
    }
}
