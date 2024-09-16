using Imageflow.Fluent;
using System.IO;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Libraries
{
    public class ThumbnailGenerator
    {
        #region Properties
        readonly string _filePhysicalPath;
        readonly string _fileExtension;
        readonly bool _isImage;

        #endregion

        #region Constructors
        public ThumbnailGenerator() { }

        public ThumbnailGenerator(string filePhysicalPath)
        {
            _filePhysicalPath = filePhysicalPath;
            _fileExtension = Path.GetExtension(filePhysicalPath);
            _isImage = _fileExtension == ".gif" || _fileExtension == ".jpg" || _fileExtension == ".jpeg" || _fileExtension == ".png";
        }
        #endregion

        #region Methods
        public static string GetThumbnailnameFromFilename(string filename)
        {
            var filenameWithoutExtension = Path.GetFileNameWithoutExtension(filename);
            var extension = Path.GetExtension(filename);
            var thumbnailName = $"{filenameWithoutExtension}_thumbnail{extension}";
            return thumbnailName;
        }

        public string GetThumbnailFilename()
        {
            var filenameWithoutExtension = Path.GetFileNameWithoutExtension(_filePhysicalPath);
            var extension = Path.GetExtension(_filePhysicalPath);
            var thumbnailName = $"{filenameWithoutExtension}_thumbnail{extension}";
            return thumbnailName;
        }

        public async Task<byte[]> GetThumbnail(uint width, uint heigth)
        {
            var thumbnailBytes = default(byte[]);
            if (_isImage)
            {
                thumbnailBytes = await GetImageThumbnail(width, heigth);
            }
            return thumbnailBytes;
        }

        async Task<byte[]> GetImageThumbnail(uint width, uint heigth)
        {
            byte[] imageByteArray = File.ReadAllBytes(_filePhysicalPath);

            using (var image = new ImageJob())
            {
                var ms1 = new MemorySource(imageByteArray);
                var fileInfo = await ImageJob.GetImageInfoAsync(ms1, SourceLifetime.Borrowed);
                var fileExtension = fileInfo.PreferredExtension.ToLower();
                ms1.Dispose();
                BuildJobResult result;
                var ms2 = new MemorySource(imageByteArray);
                if (fileExtension is "gif")
                {
                    result = await image
                    .Decode(ms2, commands: new DecodeCommands { IgnoreColorProfileErrors = true })
                    .ResizerCommands($"width={width}&height={heigth}")
                    .EncodeToBytes(new GifEncoder())
                    .Finish()
                    .InProcessAsync();
                }
                if (fileExtension is "png")
                {
                    result = await image
                    .Decode(ms2, commands: new DecodeCommands { IgnoreColorProfileErrors = true })
                    .ResizerCommands($"width={width}&height={heigth}&mode=crop")
                    .EncodeToBytes(new LodePngEncoder())
                    .Finish()
                    .InProcessAsync();
                }
                else if (fileExtension is "webp")
                {
                    result = await image
                    .Decode(ms2, commands: new DecodeCommands { IgnoreColorProfileErrors = true })
                    .ResizerCommands($"width={width}&height={heigth}&mode=crop")
                    .EncodeToBytes(new WebPLossyEncoder(80))
                    .Finish()
                    .InProcessAsync();
                }
                else
                {
                    result = await image
                    .Decode(ms2, commands: new DecodeCommands { IgnoreColorProfileErrors = true })
                    .ResizerCommands($"width={width}&height={heigth}&mode=crop")
                    .EncodeToBytes(new MozJpegEncoder(80))
                    .Finish()
                    .InProcessAsync();
                }
                ms2.Dispose();
                return result.First.TryGetBytes()?.ToArray();
            }
        }
        #endregion
    }
}