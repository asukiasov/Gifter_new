using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.DTO
{
    public class DictionariesDTO
    {
        #region Properties
        public int? DictionaryID { get; set; }
        public int? DictionaryParentID { get; set; }
        public string DictionaryCaption { get; set; }
        public string DictionaryCaptionEng { get; set; }
        public int? DictionaryLevel { get; set; }
        public string DictionaryStringCode { get; set; }
        public int? DictionaryIntCode { get; set; }
        public decimal? DictionaryDecimalValue { get; set; }
        public int? DictionaryCode { get; set; }
        public bool DictionaryIsDefault { get; set; }
        public bool DictionaryIsVisible { get; set; }
        public int? DictionarySortIndex { get; set; }
        public DateTime? DictionaryDateCreated { get; set; }
        #endregion
    }
}
