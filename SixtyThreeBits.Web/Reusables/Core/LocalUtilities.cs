using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Reusables.Core
{
    public class LocalUtilities
    {                 
        public static WebProjectModelBase GetWebProjectModelBaseFromController(object Controller)
        {
            dynamic C = Controller;
            var Model = C.Model as WebProjectModelBase;
            return Model;
        }        

        public static void SetLayoutViewModel<T>(ViewDataDictionary ViewData, T ViewModel, string Key)
        {
            ViewData[Key] = ViewModel;            
        }

        public static T GetLayoutViewModel<T>(ViewDataDictionary ViewData, string Key = null)
        {
            return (T)ViewData[Key];
        }
    }    
}