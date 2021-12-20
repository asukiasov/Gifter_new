using Microsoft.AspNetCore.Http;
using SixtyThreeBits.Core.Abstractions;
using SixtyThreeBits.Libraries;
using System.Linq;

namespace SixtyThreeBits.Web.Reusables
{
    
    public class SessionAssistance : ISessionAssistance
    {
        #region Properties
        ISession Session;
        #endregion

        #region Constructors
        public SessionAssistance(ISession Session)
        {
            this.Session = Session;                        
        }
        #endregion

        #region Methods
        public void Clear()
        {            
            Session.Clear();
        }

        public T Get<T>(string Key)
        {
            return HasKey(Key) ? Session.GetString(Key).DeserializeJsonTo<T>() : default(T);
        }

        public string GetSessionID()
        {
            return Session.Id;
        }

        public bool HasKey(string Key)
        {
            return Session.Keys.Contains(Key);
        }

        public void Set<T>(string Key, T Value)
        {            
            Session.SetString(Key, Value.ToJSON());
        }         

        public void Remove(string Key)
        {
            Session.Remove(Key);
        }
        #endregion
    }
}
