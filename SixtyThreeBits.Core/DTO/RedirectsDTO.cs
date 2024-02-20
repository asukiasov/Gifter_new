using System;

namespace SixtyThreeBits.Core.DTO
{
    public class RedirectDTO
    {
        #region Properties
        public int? RedirectID { get; set; }
        public string RedirectFrom { get; set; }
        public string RedirectTo { get; set; }
        public DateTime? RedirectDateCreated { get; set; }
        #endregion
    }
}
