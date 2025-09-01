using Microsoft.AspNetCore.StaticFiles;

namespace SixtyThreeBits.Web.Domain.ViewModels.Shared
{
    public class FileDownloadViewModel
    {
        #region Properties
        public byte[] FileBytes { get; set; }
        public string MimeType =>GetMimeType(Filename);
        public string Filename { get; set; }
        #endregion

        #region Methods
        public string GetMimeType(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return "application/octet-stream"; // Default MIME type
            }
            else
            {
                var provider = new FileExtensionContentTypeProvider();

                if (!provider.TryGetContentType(fileName, out var contentType))
                {
                    contentType = "application/octet-stream";
                }

                return contentType;
            }
        }
        #endregion
    }
}
