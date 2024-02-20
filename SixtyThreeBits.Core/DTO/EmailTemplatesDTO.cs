using SixtyThreeBits.Libraries;
using System.Collections.Generic;

namespace SixtyThreeBits.Core.DTO
{
    public class EmailTemplateDTO
    {
        #region Properties
        public int? EmailTemplateID { get; set; }
        public string EmailTemplateName { get; set; }
        public string EmailTemplateSubject { get; set; }
        public string EmailTemplateBody { get; set; }
        public string EmailTemplateSubjectEng { get; set; }
        public string EmailTemplateBodyEng { get; set; }
        public List<KeyValueTuple<string, string>> EmailTemplatesPlaceHolders { get; set; }
        #endregion
    }
}
