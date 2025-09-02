namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
{
    public record EmailTemplatesListDTO
    {
        #region Properties
        public int? EmailTemplateID { get; init; }
        public string EmailTemplateName { get; init; }
        #endregion
    }
}
