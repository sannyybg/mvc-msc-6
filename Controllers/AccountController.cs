using muscshop.Context;
using muscshop.Models;
using muscshop.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace muscshop.Controllers
{
    public class AccountController : Controller
    {
        private StoreContext _storeContext = new StoreContext();

        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(RegUser newUser)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }

            var result = _storeContext.Users.Where(x => x.Username == newUser.Username).FirstOrDefault();

            if (result != null)
            { 
                ModelState.AddModelError("Username", "Username already Exists");
                return View();
            }

            User user = new User()
            {
                Email = newUser.Email,
                Username = newUser.Username,
                Password = newUser.Password,
                Confirmation = Guid.NewGuid(),
                PassRecovery = Guid.NewGuid()
            };

            Uri uri = new Uri(Request.Url.AbsoluteUri);

            var urlHost = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;

            var text = $"რეგისტრაციის დასასრულებლად გადადით ლინკზე: {urlHost}/Account/Confirmation/{user.Confirmation}";
            SendConfirmation(user.Email, text);
            

            _storeContext.Users.Add(user);
            _storeContext.SaveChanges();



            return RedirectToAction("login");
        }

        
        private void SendConfirmation(string to, string text)
        {
            string filename = @"C:\Users\99555\source\repos\muscshop\Content\Registration.txt";

            if(System.IO.File.Exists(filename))
            {
                System.IO.File.Delete(filename);
            }

            using(System.IO.FileStream fs = System.IO.File.Create(filename))
            {
                byte[] innerText = new UTF8Encoding(true).GetBytes(text);
                fs.Write(innerText, 0, innerText.Length);
            }
        }

        public ActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogIn(User user)
        {

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Incorrect Username or Password");
                return View();
            }

            var result = _storeContext.Users.Where(x => x.Username == user.Username && x.Password == user.Password);
            if (result == null)
            {
                ModelState.AddModelError("", "Incorrect Username or Password");
                return View();
            }


            return RedirectToAction("index", "store");


        }

        public ActionResult Confirmation(string id)
        {
            var user = _storeContext.Users.Where(x => x.Confirmation.ToString().ToLower() == id.ToString().ToLower()).FirstOrDefault();
            user.Active = true;
            _storeContext.SaveChanges();

            return RedirectToAction("login");
        }

        




        public void SendRecovery(string email)
        {
            var user = _storeContext.Users.Where(x => x.Email == email).FirstOrDefault();
            SendRecovery2(user);

         
        }

       

        private ActionResult SendRecovery2(User user)
        {
            Uri uri = new Uri(Request.Url.AbsoluteUri);

            var urlHost = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;

            var text = $"პაროლის აღსადგენად გადადით ბმულზე: {urlHost}/Account/Recovery/{user.PassRecovery}";
            SendtoFile(user.Email, text);

            return RedirectToAction("login");
        }

        private void SendtoFile(string email, string text)
        {
            string filename = @"C:\Users\99555\source\repos\muscshop\Content\Recovery.txt";

            if (System.IO.File.Exists(filename))
            {
                System.IO.File.Delete(filename);
            }

            using (System.IO.FileStream fs = System.IO.File.Create(filename))
            {
                byte[] innerText = new UTF8Encoding(true).GetBytes(text);
                fs.Write(innerText, 0, innerText.Length);
            }
        }





        public ActionResult Recovery(string id)
        {
            
            var user = _storeContext.Users.Where(x => x.PassRecovery.ToString().ToLower() == id.ToString().ToLower()).FirstOrDefault();

            return View(user);
        }


        [HttpPost]
        public ActionResult Recovery(User userpass)
        {

            var olduserpass = _storeContext.Users.Where(x => x.UserId == userpass.UserId).FirstOrDefault();

            olduserpass.Password = userpass.Password;
            _storeContext.SaveChanges();
            return RedirectToAction("login");
            
        }


        
    }
}