using System.Data.Services.Client;
using System.IO;
using System.Net;
using BPMonlineHomeTask.BPMonlineServiceRef;

namespace BPMonlineHomeTask.Models.Authenticate
{
    public interface IUserAuthManager
    {
        bool TryLogin();
        IUserAuthModel GetUser();
        void SetUser(IUserAuthModel user);
        CookieContainer GetCookie();
        void SetNewAuthManager(string authUri, IUserAuthModel user, CookieContainer cookieContainer);
    }

    public class UserAuthManager : IUserAuthManager
    {
        private string _authServiceUri;
        private IUserAuthModel _user;
        private CookieContainer _authCookie;

        public UserAuthManager()
        {
            /*
            _authServiceUri = "http://178.159.246.209:1410/ServiceModel/AuthService.svc/Login";
            _authCookie = new CookieContainer();
            _user = user;
             */
        }

        public UserAuthManager(string authUri,IUserAuthModel user,CookieContainer cookieContainer)
        {
            _authServiceUri = authUri;
            _authCookie = cookieContainer;
            _user = user;
        }

        public void SetNewAuthManager(string authUri,IUserAuthModel user,CookieContainer cookieContainer)
        {
            _authServiceUri = authUri;
            _authCookie = cookieContainer;
            _user = user;
        }

        public bool TryLogin()
        {
            var authRequest = HttpWebRequest.Create(_authServiceUri) as HttpWebRequest;
            authRequest.Method = "POST";
            authRequest.ContentType = "application/json";
            authRequest.CookieContainer = _authCookie;

            using (var requestStrem = authRequest.GetRequestStream())
            {
                using (var writer = new StreamWriter(requestStrem))
                {
                    writer.Write(@"{
                        ""UserName"":""" + _user.GetName() + @""",
                        ""UserPassword"":""" + _user.GetPass() + @"""
                    }");
                }
            }

            using (var response = (HttpWebResponse)authRequest.GetResponse())
            {
                if (_authCookie.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public IUserAuthModel GetUser()
        {
            return _user;
        }

        public void SetUser(IUserAuthModel user)
        {
            _user = user;
        }

        public CookieContainer GetCookie()
        {
            return _authCookie;
        }
    }
}