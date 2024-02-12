using SixtyThreeBits.Core.Infrastructure.Libraries.FileStorages.Core;
using SixtyThreeBits.Core.Infrastructure.Utilities;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;

namespace SixtyThreeBits.Core.Infrastructure.Libraries.FileStorages
{
    public static class FileStorageManager
    {
        public static readonly FrozenDictionary<string, FileStorageModule> Modules = new Dictionary<string, FileStorageModule>
        {
            { Enums.FileManagerModules.Blog, new FileStorageModule(ModuleName: Enums.FileManagerModules.Blog, FolderName: "blog", ThumbnailFolderPath: "blog/thumbnails") },
            { Enums.FileManagerModules.News, new FileStorageModule(ModuleName: Enums.FileManagerModules.News, FolderName: "news", ThumbnailFolderPath: "news/thumbnails")},
            { Enums.FileManagerModules.Pages, new FileStorageModule(ModuleName: Enums.FileManagerModules.Pages, FolderName: "pages", ThumbnailFolderPath: "pages/thumbnails") },
            { Enums.FileManagerModules.Products, new FileStorageModule(ModuleName: Enums.FileManagerModules.Products, FolderName: "products", ThumbnailFolderPath: "products/thumbnails") },
        }.ToFrozenDictionary();
    }        
}
