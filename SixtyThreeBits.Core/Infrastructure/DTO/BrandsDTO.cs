using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.DTO
{
    public class BrandDTO
    {
        #region Properties
        public int? BrandID { get; set; }
        public string BrandName { get; set; }
        public string BrandNameEng { get; set; }
        public string BrandImageFilename { get; set; }
        public DateTime? BrandDateCreated { get; set; }
        #endregion
    }
}
