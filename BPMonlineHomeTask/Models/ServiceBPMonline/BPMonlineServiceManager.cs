using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Net;
using System.Web;
using BPMonlineHomeTask.BPMonlineServiceRef;
using BPMonlineHomeTask.Models.Authenticate;

namespace BPMonlineHomeTask.Models.ServiceBPMonline
{
    public interface IBPMonlineServiceManager
    {
        bool InitialUser();
        bool InitialUser(string userName, string userPass);
        bool InitialUser(UserAuthModel user);
        BPMonline GetBPMonlineService();
        void SetNewBPMonlineServiceManager(string serviceUri, IUserAuthManager userAuthManager, BPMonline service);
    }

    public class BPMonlineServiceManager : IBPMonlineServiceManager
    {
        private BPMonline _service;
        private IUserAuthManager _authManager;
        private string _serviceUri;

        public BPMonlineServiceManager()
        {
            /*
            _serviceUri = "http://178.159.246.209:1410/0/ServiceModel/EntityDataService.svc/";
            _authManager = new UserAuthManager(new UserAuthModel());
            _service = new BPMonline(new Uri(_serviceUri));
            _service.SendingRequest += new EventHandler<SendingRequestEventArgs>(OnSendingRequestCookie);
            */
        }

        public BPMonlineServiceManager(string serviceUri,IUserAuthManager userAuthManager, BPMonline service)
        {
            _serviceUri = serviceUri;
            _authManager = userAuthManager;
            _service = service;
            _service.SendingRequest += new EventHandler<SendingRequestEventArgs>(OnSendingRequestCookie);
            
        }

        public void SetNewBPMonlineServiceManager(string serviceUri, IUserAuthManager userAuthManager, BPMonline service)
        {
            _serviceUri = serviceUri;
            _authManager = userAuthManager;
            _service = service;
            _service.SendingRequest += new EventHandler<SendingRequestEventArgs>(OnSendingRequestCookie);
            
        }

        public BPMonline GetBPMonlineService()
        {
            return _service;
        }

        public bool InitialUser()
        {
            return _authManager.TryLogin();
        }

        public bool InitialUser(string userName,string userPass)
        {
            _authManager.SetUser(new UserAuthModel(userName,userPass));
            return _authManager.TryLogin();
        }

        public bool InitialUser(UserAuthModel user)
        {
            _authManager.SetUser(user);
            return _authManager.TryLogin();
        }

        private void OnSendingRequestCookie(object sender, SendingRequestEventArgs e)
        {
            if (_authManager.GetCookie() == null)
            {
                _authManager.TryLogin();
            }

            var req = e.Request as HttpWebRequest;
            req.CookieContainer = _authManager.GetCookie();

            e.Request = req;
        }
    }
}