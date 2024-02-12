namespace SixtyThreeBits.Core.Infrastructure.DTO
{
    public class EmailAttachmentDTO
    {
        #region Properties
        public string Filename { get; set; }
        public byte[] FileBytes { get; set; }
        #endregion
    }
}
