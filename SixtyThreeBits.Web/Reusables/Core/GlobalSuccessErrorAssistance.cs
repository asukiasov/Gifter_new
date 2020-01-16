using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using System;

namespace SixtyThreeBits.Web.Reusables.Core
{
    public class SuccessErrorPartialViewAssistance
    {
        #region Methods
        public static T SetSuccessErrorMessageInLayoutModel<T>(ISessionAssistance SessionAssistance,T ViewModel) where T : LayoutViewModelBase
        {
            var ErrorMessage = SessionAssistance.Get<string>(Constants.Session.SuccessErrorMessage.Error);            
            if (ErrorMessage != null)
            {
                ViewModel.SuccessErrorViewModel = new SuccessErrorPartialViewModel
                {
                    ShowError = true,
                    Message = ErrorMessage
                };

                SessionAssistance.Remove(Constants.Session.SuccessErrorMessage.Error);
            }
            else
            {
                var SuccessMessage = SessionAssistance.Get<string>(Constants.Session.SuccessErrorMessage.Success);
                if (SuccessMessage != null)
                {
                    ViewModel.SuccessErrorViewModel = new SuccessErrorPartialViewModel
                    {
                        ShowSuccess = true,
                        Message = SuccessMessage
                    };
                    SessionAssistance.Remove(Constants.Session.SuccessErrorMessage.Success);
                }
            }

            return ViewModel;
        }

        public static void InitErrorMessage<T>(string Message = null, ViewDataDictionary ViewData = null, ISessionAssistance SessionAssistance = null) where T : LayoutViewModelBase
        {
            if (Message == null)
            {
                Message = Resources.TextError;
            }

            if (ViewData == null && SessionAssistance != null)
            {
                SessionAssistance.Set(Constants.Session.SuccessErrorMessage.Error, Message);
            }
            else if (ViewData != null)
            {
                var Model = LocalUtilities.GetLayoutViewModel<T>(ViewData:ViewData,Key: Constants.ViewData.LayoutViewModel);
                
                if (Model != null)
                {
                    Model.SuccessErrorViewModel = new SuccessErrorPartialViewModel
                    {
                        ShowError = true,
                        Message = Message
                    };

                    LocalUtilities.SetLayoutViewModel(ViewData: ViewData, ViewModel: Model, Key: Constants.ViewData.LayoutViewModel);
                }
            }
        }

        public static void InitSuccessMessage(string Message = null, ISessionAssistance Session = null)
        {
            if (Session != null)
            {
                if (Message == null)
                {
                    Message = Resources.TextSuccess;
                }
                Session.Set(Constants.Session.SuccessErrorMessage.Success, Message);
            }
        }

        public static void PrepareSuccessErrorMessageForJavascript<T>(ViewDataDictionary ViewData)  where T : LayoutViewModelBase
        {
            if (ViewData != null)
            {
                var Model = LocalUtilities.GetLayoutViewModel<T>(ViewData: ViewData, Key: Constants.ViewData.LayoutViewModel);

                if (Model != null && Model.SuccessErrorViewModel == null)
                {
                    Model.SuccessErrorViewModel = new SuccessErrorPartialViewModel
                    {
                        IsSuccessErrorObjectGenerate = true
                    };

                    LocalUtilities.SetLayoutViewModel(ViewData:ViewData, ViewModel: Model, Key: Constants.ViewData.LayoutViewModel);
                }
            }
        } 
        #endregion
    }

    [Serializable]
    public class SuccessErrorPartialViewModel
    {
        #region Properties
        public bool IsTop { set; get; }
        public string Message { set; get; }
        public bool ShowError { set; get; }
        public bool ShowSuccess { set; get; }
        public bool IsSuccessErrorObjectGenerate { get; set; }
        #endregion
    }
}