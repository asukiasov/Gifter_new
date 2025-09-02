namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
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
        #endregion
    }   
}
