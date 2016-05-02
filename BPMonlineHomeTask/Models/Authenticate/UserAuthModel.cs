using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Ajax.Utilities;

namespace BPMonlineHomeTask.Models.Authenticate
{
    public interface IUserAuthModel
    {
        string GetName();
        string GetPass();
        void SetNewUser(string userName, string userPass);
    }

    public class UserAuthModel : IUserAuthModel
    {
        private string _userName;
        private string _userPassword;

        public UserAuthModel()
        {
            /*
            _userName = "Arkady";
            _userPassword = "102938";
             */
        }

        public UserAuthModel(string userName, string userPassword)
        {
            _userName = userName;
            _userPassword = userPassword;
        }

        public void SetNewUser(string userName, string userPass)
        {
            _userName = userName;
            _userPassword = userPass;
        }

        public string GetName()
        {
            return _userName;
        }

        public string GetPass()
        {
            return _userPassword;
        }
    }
}