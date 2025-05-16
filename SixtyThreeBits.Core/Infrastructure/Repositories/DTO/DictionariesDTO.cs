using System;

namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
{
    public record DictionariesDTO
    {
        #region Properties
        public int? DictionaryID { get; init; }
        public int? DictionaryParentID { get; init; }
        public string DictionaryCaption { get; init; }
        public string DictionaryCaptionEng { get; init; }
        public int? DictionaryLevel { get; init; }
        public string DictionaryStringCode { get; init; }
        public int? DictionaryIntCode { get; init; }
        public decimal? DictionaryDecimalValue { get; init; }
        public int? DictionaryCode { get; init; }
        public bool DictionaryIsDefault { get; init; }
        public bool DictionaryIsVisible { get; init; }
        public int? DictionarySortIndex { get; init; }
        public DateTime? DictionaryDateCreated { get; init; }
        #endregion
    }

    public record DictionarieIudDTO
    {
        #region Properties
        public int? DictionaryID { get; init; }
        public int? DictionaryParentID { get; init; }
        public string DictionaryCaption { get; init; }
        public string DictionaryCaptionEng { get; init; }
        public int? DictionaryCode { get; init; }        
        public int? DictionaryIntCode { get; init; }
        public string DictionaryStringCode { get; init; }
        public decimal? DictionaryDecimalValue { get; init; }
        public bool? DictionaryIsVisible { get; init; }
        public bool? DictionaryIsDefault { get; init; }
        public int? DictionarySortIndex { get; init; }
        #endregion
    }
}
