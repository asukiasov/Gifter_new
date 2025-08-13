using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Base;
using System;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class ErrorLogModel : ModelBase
    {
        #region Methods
        public ViewModel GetViewModel()
        {
            var viewModel = new ViewModel();
            viewModel.UrlClear = Url.RouteUrl(ControllerActionRouteNames.Admin.ErrorLogController.Clear);
            var ErrorLogFile = $"{AppDomain.CurrentDomain.BaseDirectory}App_Data\\ErrorLog.txt";
            if (System.IO.File.Exists(ErrorLogFile))
            {
                viewModel.ErrorLogContents = System.IO.File.ReadAllText(ErrorLogFile);
            }
            return viewModel;
        }

        public AjaxResponse ClearLogs()
        {
            var viewModel = new AjaxResponse();
            var errorLogFilePath = $"{AppDomain.CurrentDomain.BaseDirectory}App_Data\\ErrorLog.txt";
            if (System.IO.File.Exists(errorLogFilePath))
            {
                System.IO.File.WriteAllText(errorLogFilePath, "");
            }
            
            viewModel.IsSuccess = true;
            return viewModel;
        }

        public class ViewModel
        {
            #region Properties
            public string ErrorLogContents { get; set; }
            public string UrlClear { get; set; }
            public readonly string TextLogClear = Resources.TextLogClear;
            public readonly string TextLogClearConfirm = Resources.TextLogClearConfirm;
            #endregion
        }
        #endregion
    }
}
