using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DB;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Modules
{
    public class CategoriesDataAccess : DataAccessBase
    {
        #region Properties
        UtilityCollection Utilities;
        #endregion

        #region Constructors
        public CategoriesDataAccess(ConnectionFactory ConnectionFactory, UtilityCollection Utilities) : base(ConnectionFactory)
        {
            this.Utilities = Utilities;
        }
        #endregion

        #region Methods
        public async Task DeleteRecursive(int? CategoryID)
        {
            await TryExecuteAsyncTask($"{nameof(DeleteRecursive)}({nameof(CategoryID)} = {CategoryID})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    var DBItems = db.CategoriesListForDeleteRecursive(CategoryID).ToList();
                    foreach (var Item in DBItems)
                    {
                        Utilities.DeleteUploadedFile(Item.CategoryImageFilename);
                    }
                    await db.CategoriesDeleteRecursive(CategoryID);
                }
            });
        }

        public async Task<int?> CategoriesIUD(Enums.DatabaseActions DatabaseAction, int? CategoryID = null, int? CategoryParentID = null, string CategoryName = null, string CategoryNameEng = null, string CategorynameRus = null, string CategoryImageFilename = null, string CategoryDescriptionShort = null, string CategoryDescriptionShortEng = null, string CategoryDescriptionShortRus = null)
        {
            return await TryToReturnAsyncTask($"{nameof(CategoriesIUD)}({nameof(DatabaseAction)} = {DatabaseAction}, {nameof(CategoryID)} = {CategoryID}, {nameof(CategoryParentID)} = {CategoryParentID}, {nameof(CategoryName)} = {CategoryName}, {nameof(CategoryNameEng)} = {CategoryNameEng}, {nameof(CategorynameRus)} = {CategorynameRus}, {nameof(CategoryImageFilename)} = {CategoryImageFilename}, {nameof(CategoryDescriptionShort)} = {CategoryDescriptionShort}, {nameof(CategoryDescriptionShortEng)} = {CategoryDescriptionShortEng}, {nameof(CategoryDescriptionShortRus)} = {CategoryDescriptionShortRus} )", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    CategoryID = await db.CategoriesIUD(DatabaseAction, CategoryID, CategoryParentID, CategoryName, CategoryNameEng, CategorynameRus, CategoryImageFilename, CategoryDescriptionShort, CategoryDescriptionShortEng, CategoryDescriptionShortRus);
                    return CategoryID;
                }
            });
        }

        public async Task<Category> GetSingleCategoryByID(int? CategoryID)
        {
            return await TryToReturnAsyncTask($"{nameof(GetSingleCategoryByID)}({nameof(CategoryID)} = {CategoryID})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    var Result = await db.CategoriesGetSingleByID(CategoryID);
                    return Result?.DeserializeTo<Category>();
                }
            });
        }

        public async Task<Category> GetSingleCategoryBySlug(string CategorySlug)
        {
            return await TryToReturnAsyncTask($"{nameof(GetSingleCategoryBySlug)}({nameof(CategorySlug)} = {CategorySlug})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    var Result = await db.CategoriesGetSingleBySlug(CategorySlug);
                    return Result?.DeserializeTo<Category>();
                }
            });
        }

        public async Task<List<Category>> ListCategories(int? CategoryParentID = null)
        {
            return await TryToReturnAsyncTask($"{nameof(ListCategories)}({nameof(CategoryParentID)} = {CategoryParentID})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return (await db.CategoriesList(CategoryParentID).OrderBy(Item => Item.CategorySortIndex).ToListAsync())?.Select(Item => new Category
                    {
                        CategoryID = Item.CategoryID,
                        CategoryParentID = Item.CategoryParentID,
                        CategorySlug = Item.CategorySlug,
                        CategoryName = Item.CategoryName,
                        CategoryNameEng = Item.CategoryNameEng,
                        CategoryNameRus = Item.CategoryNameRus,
                        CategorySortIndex = Item.CategorySortIndex,
                        CategoryImageFilename = Item.CategoryImageFilename
                    }).ToList();
                }
            });
        }

        public async Task<List<Category>> ListCategoriesWithTitlePaddindHierarchy(char PadChar = ' ')
        {
            var Result = new List<Category>();

            Action<Category, int, List<Category>> InitCategoryNameByHierarchy = null;
            InitCategoryNameByHierarchy = (Category Parent, int PadCount, List<Category> CategorysList) =>
            {
                if (PadCount > 0)
                {
                    Parent.CategoryName = Parent.CategoryName.PadLeft(Parent.CategoryName.Length + PadCount, PadChar);
                    Result.Add(Parent);
                }
                else
                {
                    Result.Add(Parent);
                }

                var Children = CategorysList.Where(Item => Item.CategoryParentID == Parent.CategoryID).ToList();
                foreach (var Category in Children)
                {
                    InitCategoryNameByHierarchy(Category, PadCount + 4, CategorysList);
                }
            };

            var Categories = await ListCategories();
            if (Categories?.Count > 0)
            {
                var Parents = Categories.Where(Item => Item.CategoryParentID == null).OrderBy(Item => Item.CategorySortIndex).ToList();
                foreach (var Item in Parents)
                {
                    InitCategoryNameByHierarchy(Item, 0, Categories);
                }
            }

            return Result;
        }

        public async Task SyncParentsAndSortIndexes(List<SyncSortIndexesItem> SortIndexes)
        {
            await TryExecuteAsyncTask($"{nameof(SyncParentsAndSortIndexes)}({nameof(SortIndexes)} = {SortIndexes.ToXml()})", async () =>
            {                
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    await db.CategoriesSyncParentsAndSortIndexes(SortIndexes.ToXml());
                }
            });
        }
        #endregion

        #region Sub Classes
        public class Category
        {
            #region Properties
            public int? CategoryID { get; set; }
            public int? CategoryParentID { get; set; }
            public string CategorySlug { get; set; }
            public string CategoryName { get; set; }
            public string CategoryNameEng { get; set; }
            public string CategoryNameRus { get; set; }
            public string CategoryImageFilename { get; set; }
            public int? CategorySortIndex { get; set; }
            public string CategoryDescriptionShort { get; set; }
            public string CategoryDescriptionShortEng { get; set; }
            public string CategoryDescriptionShortRus { get; set; }
            public DateTime? CategoryDateCreated { get; set; }
            #endregion

            #region Methods
            public override string ToString()
            {
                return CategoryName;
            }
            #endregion
        }
        #endregion
    }
}
