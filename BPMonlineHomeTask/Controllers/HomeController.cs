using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using BPMonlineHomeTask.BPMonlineServiceRef;
using BPMonlineHomeTask.Models.Authenticate;
using BPMonlineHomeTask.Models.ServiceBPMonline;

namespace BPMonlineHomeTask.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        private const string _authServiceUri = "http://178.159.246.209:1410/ServiceModel/AuthService.svc/Login";
        private const string _serviceUri = "http://178.159.246.209:1410/0/ServiceModel/EntityDataService.svc/";

        private static readonly XNamespace ds = "http://schemas.microsoft.com/ado/2007/08/dataservices";
        private static readonly XNamespace dsmd = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";
        private static readonly XNamespace atom = "http://www.w3.org/2005/Atom"; 
        
        private int itemsPerPage = 5;
        private IBPMonlineServiceManager _serviceManager;
        private IUserAuthModel _userModel;
        private IUserAuthManager _userAuthManager;

        public HomeController(IUserAuthModel userModel, IUserAuthManager userManager, IBPMonlineServiceManager serviceManager)
        {
            userModel.SetNewUser("Arkady", "102938");
            userManager.SetNewAuthManager(_authServiceUri,userModel,new CookieContainer());
            serviceManager.SetNewBPMonlineServiceManager(_serviceUri,userManager,new BPMonline(new Uri(_serviceUri)));

            _userModel = userModel;
            _userAuthManager = userManager;
            _serviceManager = serviceManager;
        }

        public ActionResult Index(int id = 0)
        {
            bool flag = _serviceManager.InitialUser();
            BPMonline service = _serviceManager.GetBPMonlineService();
            
            int contactsCount = service.ContactCollection.Count();
            var contacts = service.ContactCollection.OrderBy(x => x.CreatedOn).Skip(id * itemsPerPage).Take(itemsPerPage);

            var list = contacts.ToList();

            ViewBag.itemsPerPageNum = itemsPerPage; // count of items per page
            ViewBag.pageFirstItemNum = id*itemsPerPage; // calculate number of first item on page
            ViewBag.pageTotalItemsNum = contactsCount; //total count of ContactCollectioin
            ViewBag.pageTotalPagesNum = (contactsCount / itemsPerPage)-1; // total count of pages
            ViewBag.pageCurrPageNum = id; // number of current page
            

            return View(list);
        }

        public ActionResult Create()
        {
            return View(new BPMonlineServiceRef.Contact());
        }

        [HttpPost]
        public string CreateContact(BPMonlineServiceRef.Contact contact)
        {
            contact.CreatedOn = DateTime.Now;
            DateTime newDateTime;
            DateTime.TryParse(contact.BirthDate.ToString(), out newDateTime);

            var content = new XElement(dsmd + "properties",
                  //new XElement(ds + "Id", contact.Id),
                  new XElement(ds + "Name", contact.Name),
                  new XElement(ds + "MobilePhone", contact.MobilePhone),
                  new XElement(ds + "Dear", contact.Dear),
                  new XElement(ds + "JobTitle", contact.JobTitle),
                  new XElement(ds + "CreatedOn", contact.CreatedOn),
                  new XElement(ds + "BirthDate", newDateTime));
            
            var entry = new XElement(atom + "entry",
                new XElement(atom + "content",
                new XAttribute("type", "application/xml"), content));

            var request = (HttpWebRequest)HttpWebRequest.Create(_serviceUri + "ContactCollection/");
            request.Credentials = new NetworkCredential(_userModel.GetName(), _userModel.GetPass());
            request.Method = "POST";
            request.Accept = "application/atom+xml";
            request.ContentType = "application/atom+xml;type=entry";

            using (var writer = XmlWriter.Create(request.GetRequestStream()))
            {
                entry.WriteTo(writer);
            }
            // Получение ответа от сервиса о результате выполнения операции.
            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    if (((HttpWebResponse)response).StatusCode == HttpStatusCode.Created)
                    {
                        ViewBag.Message = response.Headers;
                        // Обработка результата выполнения операции.
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }


            return "Item created!";
        }

        
        [HttpPost]
        //[ValidateAntiForgeryToken()]
        public string DeleteContact(string id)
        {
            string result = "";
            // Создание запроса к сервису, который будет удалять данные.
            var str = _serviceUri + "ContactCollection(guid'" + id + "')";

            var request = (HttpWebRequest)HttpWebRequest.Create(str);
            request.Credentials = new NetworkCredential(_userModel.GetName(), _userModel.GetPass());
            request.Method = "DELETE";
            // Получение ответа от сервися о результате выполненя операции.
            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    ViewBag.Message = response.Headers;
                    result = response.Headers.ToString();
                    // Обработка результата выполнения операции.
                }

            }
            catch (Exception ex)
            {

                return ex.Message;
            }
            return "Item deleted!";
        }

        [HttpPost]
        public string UpdateContact(BPMonlineServiceRef.Contact contact)
        {
            DateTime newDateTime;
            DateTime.TryParse(contact.BirthDate.ToString(), out newDateTime);
            // Создание сообщения xml, содержащего данные об изменяемом объекте.
            var content = new XElement(dsmd + "properties",
                  //new XElement(ds + "Id", contact.Id),
                  new XElement(ds + "Name", contact.Name),
                  new XElement(ds + "MobilePhone", contact.MobilePhone),
                  new XElement(ds + "Dear", contact.Dear),
                  new XElement(ds + "JobTitle", contact.JobTitle),
                  new XElement(ds + "BirthDate", newDateTime));
            var entry = new XElement(atom + "entry",
                    new XElement(atom + "content",
                            new XAttribute("type", "application/xml"),
                            content)
                    );
            // Создание запроса к сервису, который будет изменять данные объекта.
            var str = _serviceUri + "ContactCollection(guid'" + contact.Id + "')";
            var request = (HttpWebRequest)HttpWebRequest.Create(str);
            request.Credentials = new NetworkCredential(_userModel.GetName(), _userModel.GetPass());
            request.Method = "PUT";
            request.Accept = "application/atom+xml";
            request.ContentType = "application/atom+xml;type=entry";
            // Запись сообщения xml в поток запроса.
            using (var writer = XmlWriter.Create(request.GetRequestStream()))
            {
                entry.WriteTo(writer);
            }
            // Получение ответа от сервиса о результате выполнения операции.
            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    ViewBag.Message = response.Headers.AllKeys.ToString();
                    // Обработка результата выполнения операции.
                }
            }
            catch (Exception ex)
            {
                
                return ex.Message;
            }

            return "Item updated!";
        }


    }
}
