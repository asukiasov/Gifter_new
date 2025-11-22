namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        public static partial class Admin
        {
            public static class DictionariesController
            {
                public const string Dictionaries = $"{nameof(Admin)}{nameof(DictionariesController)}{nameof(Dictionaries)}";
                public const string Tree = $"{nameof(Admin)}{nameof(DictionariesController)}{nameof(Tree)}";
                public const string TreeAdd = $"{nameof(Admin)}{nameof(DictionariesController)}{nameof(TreeAdd)}";
                public const string TreeUpdate = $"{nameof(Admin)}{nameof(DictionariesController)}{nameof(TreeUpdate)}";
                public const string TreeDelete = $"{nameof(Admin)}{nameof(DictionariesController)}{nameof(TreeDelete)}";
            }
        }
    }
}
