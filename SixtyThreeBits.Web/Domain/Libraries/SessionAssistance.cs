using Microsoft.AspNetCore.Http;
using SixtyThreeBits.Core.Abstractions.Web;
using SixtyThreeBits.Libraries.Extensions;
using System.Linq;

namespace SixtyThreeBits.Web.Domain.Libraries
{

    public class SessionAssistance : ISessionAssistance
    {
        #region Properties
        readonly ISession _session;
        #endregion

        #region Constructors
        public SessionAssistance(ISession session)
        {
            _session = session;
        }
        #endregion

        #region Methods
        public void Clear()
        {
            _session.Clear();
        }

        public T Get<T>(string key)
        {
            return HasKey(key) ? _session.GetString(key).DeserializeJsonTo<T>() : default;
        }

        public string GetSessionID()
        {
            return _session.Id;
        }

        public bool HasKey(string key)
        {
            return _session.Keys.Contains(key);
        }

        public void Set<T>(string key, T value)
        {
            _session.SetString(key, value.ToJson());
        }

        public void Remove(string key)
        {
            _session.Remove(key);
        }
        #endregion
    }
}
