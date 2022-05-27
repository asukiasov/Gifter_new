using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CG.Web.MegaApiClient;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;

namespace SixtyThreeBits.Core.Services
{
    public class MegaService : SixtyThreeBitsDataObject
    {
        #region Properties
        static string Username = AppSettings.MegaApiUsername;
        static string Password = AppSettings.MegaApiPassword;
        #endregion

        public void UploadFile(string FilePath, string UploadFolderName)
        {
            var Client = new MegaApiClient();
            Client.Login(Username, Password);

            IEnumerable<INode> Nodes = Client.GetNodes();
            INode UploadFolder = Nodes.Single(x => x.Name == UploadFolderName); 

            Client.UploadFile(FilePath, UploadFolder);
            
            Client.Logout();
        }

        public IEnumerable<INode> GetFiles(string FolderName)
        {
            var Client = new MegaApiClient();
            Client.Login(Username, Password);

            IEnumerable<INode> Nodes = Client.GetNodes();
            INode Folder = Nodes.Single(x => x.Name == FolderName);
            IEnumerable<INode> Files = Nodes.Where(n => n.ParentId == Folder.Id);

            Client.Logout();

            return Files;
        }

        public void DeleteFile(string Filename, string UploadFolderName, bool MoveToTrash = false)
        {
            var Client = new MegaApiClient();
            Client.Login(Username, Password);

            IEnumerable<INode> Nodes = Client.GetNodes();
            INode UploadFolder = Nodes.Single(x => x.Name == UploadFolderName);
            INode FileToDelete = Nodes.Single(x => x.Name == Filename && x.ParentId == UploadFolder.Id);

            Client.Delete(FileToDelete, MoveToTrash);

            Client.Logout();
        }
    }
}
