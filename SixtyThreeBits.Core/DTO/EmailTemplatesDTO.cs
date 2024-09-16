using SixtyThreeBits.Libraries;
using System.Collections.Generic;

namespace SixtyThreeBits.Core.DTO
{
    public record EmailTemplateDTO
    {
        #region Properties
        public int? EmailTemplateID { get; init; }
        public string EmailTemplateName { get; init; }
        public string EmailTemplateSubject { get; init; }
        public string EmailTemplateBody { get; init; }
        public string EmailTemplateSubjectEng { get; init; }
        public string EmailTemplateBodyEng { get; init; }
        public List<KeyValueTuple<string, string>> EmailTemplatesPlaceHolders { get; init; }
        #endregion
    }

    public record EmailTemplateIudDTO
    {
        #region Properties
        public int? EmailTemplateID { get; init; }
        public string EmailTemplateName { get; init; }
        public string EmailTemplateSubject { get; init; }
        public string EmailTemplateSubjectEng { get; init; }
        public string EmailTemplateBody { get; init; }
        public string EmailTemplateBodyEng { get; init; }
        #endregion
    }

    public record EmailTemplatesListDTO
    {
        #region Properties
        public int? EmailTemplateID { get; init; }
        public string EmailTemplateName { get; init; }
        #endregion
    }
}
