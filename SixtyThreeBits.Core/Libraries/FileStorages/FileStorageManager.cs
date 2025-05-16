using SixtyThreeBits.Core.Libraries.FileStorages.DTO;
using SixtyThreeBits.Core.Utilities;
using System.Collections.Frozen;
using System.Collections.Generic;

namespace SixtyThreeBits.Core.Libraries.FileStorages
{
    public static class FileStorageManager
    {
        public static readonly FrozenDictionary<string, FileStorageModuleDTO> Modules = new Dictionary<string, FileStorageModuleDTO>
        {
            { Enums.FileManagerModules.Blog, new FileStorageModuleDTO(ModuleName: Enums.FileManagerModules.Blog, FolderName: "blog", ThumbnailFolderPath: "blog/thumbnails") },
            { Enums.FileManagerModules.News, new FileStorageModuleDTO(ModuleName: Enums.FileManagerModules.News, FolderName: "news", ThumbnailFolderPath: "news/thumbnails")},
            { Enums.FileManagerModules.Pages, new FileStorageModuleDTO(ModuleName: Enums.FileManagerModules.Pages, FolderName: "pages", ThumbnailFolderPath: "pages/thumbnails") },
            { Enums.FileManagerModules.Products, new FileStorageModuleDTO(ModuleName: Enums.FileManagerModules.Products, FolderName: "products", ThumbnailFolderPath: "products/thumbnails") },
        }.ToFrozenDictionary();
    }
}
