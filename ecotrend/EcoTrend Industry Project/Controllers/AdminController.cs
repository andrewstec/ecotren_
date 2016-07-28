using EcoTrend_Industry_Project.BusinessLogic;
using EcoTrend_Industry_Project.Models;
using EcoTrend_Industry_Project.Repositories;
using EcoTrend_Industry_Project.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.Web.Security;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System;
using PagedList;

namespace EcoTrend_Industry_Project.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : BaseController
    {
        SalesRepRepo salesRepRepo = new SalesRepRepo();



        [HttpGet]
        public ActionResult EditContact(int id)
        {
            ContactRepo contactRepo = new ContactRepo();
            ContactVM contactVm = contactRepo.GetContactDetail(id);
            return View(contactVm);
        }


        public ActionResult AdminOnly()
        {
            return View();
        }
        [HttpGet]
        public ActionResult AddRole()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddRole(AspNetRole role)
        {
            EcotrendEntities context = new EcotrendEntities();
            context.AspNetRoles.Add(role);
            context.SaveChanges();
            return View();
        }

        [HttpPost]
        public ActionResult EditContact(ContactVM contactVM)
        {
            ContactRepo contactRepo = new ContactRepo();
            ContactVM editedContact = contactRepo.Update(contactVM);
            return RedirectToAction("ContactManagement");
        }
        /////

        [HttpGet]
        public ActionResult AddNote(int salesRepID, int contactId, string firstName, string lastName)
        {
            EcotrendEntities context = new EcotrendEntities();
            ViewBag.ContactId = contactId;
            ViewBag.FirstName = firstName;
            ViewBag.LastName = lastName;
            ViewBag.SalesRepID = salesRepID;
            return View();
        }
        [HttpPost]
        public ActionResult AddNote(int salesRepID, string title, string body, int contactId)
        {
            EcotrendEntities context = new EcotrendEntities();
            LogsRepo logsRepo = new LogsRepo();

            int parsedUserName;
            int.TryParse(User.Identity.Name, out parsedUserName);

            var salesRepName = logsRepo.GetSalesRepName(salesRepID);

            Log log = new Log();
            log = logsRepo.AddLog(title, body, contactId, salesRepName);
            log.salesRepID = salesRepID;

            try
            {
                context.Logs.Add(log);
                context.SaveChanges();
                ViewBag.Success = "Note added successfully";
                return View();
                //return RedirectToAction("Result", "Home", new { success = true, result = ViewBag.Success });
            }
            catch
            {
                ViewBag.Error = "Note was not added.";
                return View();
                //return RedirectToAction("Result", "Home", new { success = false, result = ViewBag.Error });
            }
        }

        [HttpGet]
        public ActionResult CreateContact()
        {
            StoreRepo storeRepo = new StoreRepo();
            SalesRepRepo salesRepRepo = new SalesRepRepo();
            ContactVM contactVM = storeRepo.GetStoresUnderContactVM();
            contactVM.SalesReps = salesRepRepo.GetAllSalesReps();
            return View(contactVM);
        }

        [Authorize(Roles = "admin, salesrep")]
        [HttpPost]
        public ActionResult CreateContact(ContactVM contactVM)
        {
            ContactRepo contactRepo = new ContactRepo();
            try
            {
                contactRepo.AddContact(contactVM);
                TempData["success"] = contactVM.FirstName + " " + contactVM.LastName + " has been successfully added!";
            }
            catch
            {
                TempData["error"] = "success";
            }
            return RedirectToAction("ContactManagement", "Admin");
        }

        public ActionResult ContactNotes(int salesRepID, int contactId, string firstName, string lastName)
        {
            LogsRepo logsRepo = new LogsRepo();
            //var logs = logsRepo.GetAllVM(id, contactId);
            ViewBag.FirstName = firstName;
            ViewBag.LastName = lastName;
            ViewBag.ContactId = contactId;
            ViewBag.SalesRepID = salesRepID;
            return View(logsRepo.GetLogsByContactID(contactId));
        }

        [Authorize(Roles = "admin, salesrep")]
        public ActionResult ContactManagement(string sortOrder, string currentFilter, string searchString, int? page)
        {

            ContactRepo contactRepo = new ContactRepo();
            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            IEnumerable<ContactVM> contacts = contactRepo.GetContacts(searchString, sortOrder);

            // Store current search and sort filter parameters.
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;

            // Provide toggle option for name sort.
            if (String.IsNullOrEmpty(sortOrder))
                ViewBag.NameSortParm = ContactRepo.LASTNAME;
            else
                ViewBag.NameSortParm = "";

            //// Provide toggle  optionfor date sort.
            //if (sortOrder == StudentRepository.DATE)
            //    ViewBag.DateSortParm = StudentRepository.DATE_DESC;
            //else
            //    ViewBag.DateSortParm = StudentRepository.DATE;
            const int PAGE_SIZE = 15;
            int pageNumber = (page ?? 1);

            // Truncate results to fit in the view provided.
            contacts = contacts.ToPagedList(pageNumber, PAGE_SIZE);

            return View(contacts);


        }

        [HttpGet]
        public ActionResult AddUserToRole()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddUserToRole(string userName, string roleName)
        {
            EcotrendEntities context = new EcotrendEntities();
            AspNetUser user = context.AspNetUsers
                             .Where(u => u.UserName == userName).FirstOrDefault();
            AspNetRole role = context.AspNetRoles
                             .Where(r => r.Name == roleName).FirstOrDefault();

            user.AspNetRoles.Add(role);
            context.SaveChanges();
            return View();
        }
        public ActionResult ListSalesReps(string sortOrder, string currentFilter, string searchString, int? page)
        {

            IEnumerable<SalesRepVM> salesReps = salesRepRepo.GetSalesRepsSort(searchString, sortOrder);

            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            // Store current sort filter parameter.
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentFilter = searchString;

            // Provide toggle option for name sort.
            if (String.IsNullOrEmpty(sortOrder))
                ViewBag.NameSortParm = SalesRepRepo.LASTNAME;
            else
                ViewBag.NameSortParm = "";

            // Provide toggle  optionfor date sort.
            if (sortOrder == SalesRepRepo.DATE)
                ViewBag.DateSortParm = SalesRepRepo.DATE_DESC;
            else
                ViewBag.DateSortParm = SalesRepRepo.DATE;

            const int PAGE_SIZE = 15;
            int pageNumber = (page ?? 1);

            // Truncate results to fit in the view provided.
            salesReps = salesReps.ToPagedList(pageNumber, PAGE_SIZE);


            return View(salesReps);
        }

        //[HttpGet]
        //public ActionResult EditContact(int id)
        //{
        //    ContactRepo contactRepo = new ContactRepo();
        //    ContactVM contactVm = contactRepo.GetContactDetail(id);
        //    return View(contactVm);
        //}
        //[HttpPost]
        //public ActionResult EditContact(ContactVM contactVM)
        //{
        //    ContactRepo contactRepo = new ContactRepo();
        //    ContactVM editedContact = contactRepo.Update(contactVM);
        //    return RedirectToAction("ContactManagement");
        //}

        [HttpGet]
        public ActionResult EditSalesRep(string salesRepID)
        {

            return View(salesRepRepo.GetSalesRep(Convert.ToInt32(salesRepID)));
        }

        [HttpPost]
        public ActionResult EditSalesRep(SalesRepVM salesRepVM)
        {
            SalesRepRepo salesRepRepo = new SalesRepRepo();
            try
            {
                salesRepRepo.UpdateSalesRep(salesRepVM);
                TempData["success"] = "Successfully updated sales representative!";
            }
            catch
            {
                TempData["error"] = "Uh oh... Something went wrong. Try again! <span class='fa fa-frown-o fa-fw'></span>";
            }

            return RedirectToAction("ListSalesReps", "Admin");
        }
        [HttpGet]
        public ActionResult LockoutSupplier(int? page)
        {
            SupplierRepo supplierRepo = new SupplierRepo();

            List<SupplierDetailsVM> list = supplierRepo.GetAllSuppliers().ToList();
            const int PAGE_SIZE = 15;
            int pageNumber = (page ?? 1);
            return View(list.ToPagedList(pageNumber, PAGE_SIZE));

        }
        [HttpGet]
        public ActionResult ListSuppliers(int? page)
        {
            SupplierRepo supplierRepo = new SupplierRepo();

            List<SupplierDetailsVM> list = supplierRepo.GetAllSuppliers().ToList();
            const int PAGE_SIZE = 15;
            int pageNumber = (page ?? 1);
            return View(list.ToPagedList(pageNumber, PAGE_SIZE));
        }
        [HttpGet]
        public ActionResult EditSupplier(string supplierID)
        {
            ViewBag.SupplierID = supplierID;
          //  TempData["SupplierID"] = supplierID;
            SupplierRepo supplierRepo = new SupplierRepo();
            return View(supplierRepo.GetSupplier(Convert.ToInt32(supplierID)));
        }
        [HttpPost]
        public ActionResult EditSupplier(SupplierDetailsVM supplierVM)
        {
            try
            {
            //    supplierVM.SupplierID = (int)TempData["SupplierID"];
                SupplierRepo supplierRepo = new SupplierRepo();
                supplierRepo.UpdateSupplier(supplierVM);
                TempData["success"] = "Supplier details have been updated!";
            }
            catch
            {
                ViewBag.Error = "error";
            }

            return RedirectToAction("ListSuppliers", "Admin");
        }
        [HttpGet]
        public ActionResult SupplierDetails(string supplierID)
        {
            SupplierRepo supplierRepo = new SupplierRepo();
            return View(supplierRepo.GetSupplier(Convert.ToInt32(supplierID)));
        }

        public ActionResult SalesRepDetails(string salesRepID, string sortOrder, string currentFilter, string searchString, int? page)
        {
            SalesRepDetailsVM salesRepDetails = salesRepRepo.GetSalesRepDetails(Convert.ToInt32(salesRepID));

            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            ContactRepo contactsRepo = new ContactRepo();


            // Store current search and sort filter parameters.
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;

            // Provide toggle option for name sort.
            if (String.IsNullOrEmpty(sortOrder))
                ViewBag.NameSortParm = SalesRepRepo.LASTNAME;
            else
                ViewBag.NameSortParm = "";

            // Provide toggle  optionfor date sort.
            if (sortOrder == SalesRepRepo.DATE)
                ViewBag.DateSortParm = SalesRepRepo.DATE_DESC;
            else
                ViewBag.DateSortParm = SalesRepRepo.DATE;

            const int PAGE_SIZE = 15;
            int pageNumber = (page ?? 1);




            ViewBag.SalesRepID = Convert.ToInt32(salesRepID);
            // Truncate results to fit in the view provided.
            // salesRepDetails.RepsContactsPagedList = salesRepDetails.RepsContacts.ToPagedList(pageNumber, PAGE_SIZE);
            salesRepDetails.RepsContactsPagedList = contactsRepo.GetContacts(searchString, sortOrder).ToPagedList<ContactVM>(pageNumber, PAGE_SIZE);

            return View(salesRepDetails);
        }

        public ActionResult SalesRepAdded()
        {
            return View();
        }

        public ActionResult Logs(int? page)
        {
            EcotrendEntities context = new EcotrendEntities();

            var query = (from l in context.Logs
                         join c in context.Contacts on l.contactID equals c.contactID
                         select new LogContactVM
                         {
                             logID = l.logID,
                             logDate = l.logDate,
                             salesRepID = l.salesRepID,
                             contactID = l.contactID,
                             title = l.title,
                             body = l.body,
                             storeName = c.Store.name,
                             contactName = c.firstName + " " + c.lastName,
                             salesRepName = l.salesRepName
                         });
            List<LogContactVM> list = query.ToList();
            const int PAGE_SIZE = 15;
            int pageNumber = (page ?? 1);
            return View(list.ToPagedList(pageNumber, PAGE_SIZE));
        }
        [HttpGet]
        public ActionResult EditLog(int id)
        {
            LogContactVM logVM = new LogContactVM();
            LogsRepo logsRepo = new LogsRepo();
            logVM = logsRepo.GetLogVM(id);

            return View(logVM);
        }
        [HttpPost]
        public ActionResult EditLog(int id, string body, string title)
        {
            EcotrendEntities context = new EcotrendEntities();
            try
            {
                var log = context.Logs.Find(id);
                log.title = title;
                log.body = body;

                context.SaveChanges();
                //ViewBag.Success = "Log successfully updated.";

                //return RedirectToAction("Result", "Home", new { success = true, result = ViewBag.Success });
                TempData["success"] = "Log successfully updated.";
                return RedirectToAction("Logs", "Admin");
            }
            catch
            {
                //ViewBag.Error = "Uh oh... Could not update log. Try again later.";
                //return RedirectToAction("Result", "Home", new { success = false, result = ViewBag.Error });
                TempData["error"] = "error";
                return RedirectToAction("Logs", "Admin");
            }
        }
        //[HttpGet]
        //public ActionResult DeleteLog(int id)
        //{
        //    LogContactVM logVM = new LogContactVM();
        //    LogsRepo logsRepo = new LogsRepo();
        //    logVM = logsRepo.GetLogVM(id);

        //    return View(logVM);
        //}
        //[HttpPost]
        public ActionResult DeleteLog(int id, string result)
        {
            if (result == "Yes")
            {
                try
                {
                    EcotrendEntities context = new EcotrendEntities();
                    Log log = context.Logs.Find(id);
                    context.Logs.Remove(log);
                    context.SaveChanges();
                    //ViewBag.Success = "Log successfully deleted.";
                    TempData["success"] = "Log has been successfully deleted!";
                    //return RedirectToAction("Result", "Home", new { success = true, result = ViewBag.Success });
                    return RedirectToAction("Logs", "Admin");
                }
                catch
                {
                    TempData["error"] = "Uh oh... Unable to delete log.";
                    //return RedirectToAction("Result", "Home", new { success = false, result = ViewBag.Error });
                    return RedirectToAction("Logs", "Admin");
                }
            }
            return RedirectToAction("Logs", "Admin");
        }

        public ActionResult LockUser(string from, string email)
        {
            try
            {
                salesRepRepo.LockUser(email);
                if (from == "SalesRep")
                {
                    return RedirectToAction("LockoutSalesRep", "Admin");
                }
                else if (from == "Supplier")
                {
                    return RedirectToAction("LockoutSupplier", "Admin");
                }
                else
                {
                    return RedirectToAction("ListSalesReps", "Admin");
                }
            }
            catch
            {
                return RedirectToAction("LockoutSalesRep", "Admin");
            }

        }

        public ActionResult UnlockUser(string from, string email)
        {
            try
            {
                salesRepRepo.UnlockUser(email);
                if (from == "SalesRep")
                {
                    return RedirectToAction("LockoutSalesRep", "Admin");
                }
                else if (from == "Supplier")
                {
                    return RedirectToAction("LockoutSupplier", "Admin");
                }
                else
                {
                    return RedirectToAction("ListSalesReps", "Admin");
                }
            }
            catch
            {
                return RedirectToAction("LockoutSalesRep", "Admin");
            }

        }

        public ActionResult LockoutSalesRep(string sortOrder, string currentFilter, string searchString, int? page)
        {

            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            IEnumerable<SalesRepsLockoutVM> salesReps = salesRepRepo.GetAllSalesRepsLockoutStatusSorted(searchString, sortOrder);

            // Store current search and sort filter parameters.
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;

            // Provide toggle option for name sort.
            if (String.IsNullOrEmpty(sortOrder))
                ViewBag.NameSortParm = SalesRepRepo.LASTNAME;
            else
                ViewBag.NameSortParm = "";

            // Provide toggle  optionfor date sort.
            if (sortOrder == SalesRepRepo.DATE)
                ViewBag.DateSortParm = SalesRepRepo.DATE_DESC;
            else
                ViewBag.DateSortParm = SalesRepRepo.DATE;

            const int PAGE_SIZE = 15;
            int pageNumber = (page ?? 1);

            // Truncate results to fit in the view provided.
            salesReps = salesReps.ToPagedList(pageNumber, PAGE_SIZE);

            return View(salesReps);
        }
        public ActionResult EmailAllSubscribed()
        {
            return View();
        }
        public ActionResult EmailBySalesRep()
        {
            return View();
        }
        public ActionResult test()
        {
            return View();
        }
        [HttpGet]
        public ActionResult ResetPassword()
        {
            SalesAndSupplierRepo salesAndSupplierRepo = new SalesAndSupplierRepo();
            return View(salesAndSupplierRepo.GetAllSalesRepsPasswordReset());
        }
        [HttpGet]
        public ActionResult ResetPasswordByEmail(string email)
        {
            MailHelper mailHelper = new MailHelper();

            bool match = false;

            var userStore = new UserStore<IdentityUser>();
            var manager = new UserManager<IdentityUser>(userStore);
            var userSearched = manager.FindByEmail(email);

            if (userSearched.Email == email)
            {
                match = true;
            }

            if (match)
            {
                Random random = new Random();
                char ch;
                string newPassword = "";
                for (int x = 0; x <= 8; x++)
                {
                    ch = Convert.ToChar(Convert.ToInt32(Math.Floor(28 * random.NextDouble() + 65)));
                    if (ch == Convert.ToChar("I") || ch == Convert.ToChar("E") || ch == Convert.ToChar("O")
                        || ch == Convert.ToChar("B") || ch == Convert.ToChar("G")) { ch = Convert.ToChar("C"); }

                    newPassword = newPassword + ch;
                }

                try
                {
                    IdentityResult passwordResult = manager.RemovePassword(userSearched.Id);
                    if (passwordResult.Succeeded)
                    {
                        userSearched.PasswordHash = manager.PasswordHasher.HashPassword(newPassword);
                        IdentityResult updateResult = manager.Update(userSearched);
                        if (updateResult.Succeeded)
                        {
                            string body = "Your password has been reset. Your new password is " + newPassword + ". " +
                                "Please login and change your password.";
                            Message emailMessage = new Message();
                            emailMessage.To = email;
                            emailMessage.Subject = "Ecotrend Sales Manager Password Reset";
                            emailMessage.Body = body;
                            mailHelper.EmailFromArvixe(emailMessage);
                            TempData["success"] = "A new password has been sent to " + email + ".";
                        }
                    }
                }
                catch
                {

                }
            }

            StringBuilder builder = new StringBuilder();
            //   builder.Append(RandomString(37, true));
            TempData["NewPasswordEmail"] = email;
            SalesAndSupplierRepo salesAndSupplierRepo = new SalesAndSupplierRepo();
            return RedirectToAction("ResetPassword", "Admin");
            //return RedirectToAction("NewPassword", "Admin");
        }

        public ActionResult NewPassword()
        {
            ViewBag.Email = TempData["NewPasswordEmail"];
            return View();
        }
        public ActionResult GetAllFlyers(int? page)
        {
            SupplierRepo supplierRepo = new SupplierRepo();

            List<SupplierEmailVM> list = supplierRepo.GetAll().ToList();
            const int PAGE_SIZE = 15;
            int pageNumber = (page ?? 1);
            return View(list.ToPagedList(pageNumber, PAGE_SIZE));
        }
        [HttpGet]
        public ActionResult FlyerDetails(int emailId)
        {
            EcotrendEntities context = new EcotrendEntities();
            var email = context.Emails.Find(emailId);
            SupplierEmailVM supplierEmailVM = new SupplierEmailVM();
            supplierEmailVM.emailID = email.emailID;
            supplierEmailVM.body = email.body;
            supplierEmailVM.subject = email.subject;
            return View(supplierEmailVM);
        }
        [HttpPost]
        public ActionResult FlyerDetails(int emailId, string subject, string body)
        {
            EcotrendEntities context = new EcotrendEntities();
            var email = context.Emails.Find(emailId);
            email.emailID = emailId;
            email.subject = subject;
            email.body = body;
            try
            {
                context.SaveChanges();
                ViewBag.Success = "Email successfully updated.";
                TempData["success"] = "Email successfully updated!";
            }
            catch
            {
                ViewBag.Error = "Unable to save changes.";
            }
            SupplierEmailVM supplierEmailVM = new SupplierEmailVM();
            supplierEmailVM.subject = subject;
            supplierEmailVM.body = body;
            supplierEmailVM.emailID = emailId;
            return View(supplierEmailVM);
        }
        [HttpGet]
        public ActionResult ChooseContacts(int emailId)
        {
            EcotrendEntities context = new EcotrendEntities();
            var email = context.Emails.Find(emailId);
            TempData["email"] = email;
            ViewBag.EmailId = emailId;
            SalesRepRepo salesRepRepo = new SalesRepRepo();
            return View(salesRepRepo.GetAll());
        }
        [HttpPost]
        public ActionResult ChooseContacts(List<string> emails)
        {
            EcotrendEntities context = new EcotrendEntities();
            Email email = TempData["email"] as Email;
            EmailRepo emailRepo = new EmailRepo();

            try
            {
                emailRepo.MassSend(emails, "some sender", email.subject, email.body);
                var emailContext = context.Emails.Find(email.emailID);
                emailContext.approved = true;
                emailContext.dateSent = DateTime.Now;
                context.SaveChanges();
                //ViewBag.Success = "Email approved and has been successfuly sent off.";
                TempData["success"] = "Email approved and has been successfuly sent off.";
            }
            catch
            {
                ViewBag.Error = "Uh oh.. Something went wrong. <a href='../Admin/GetAllFlyers' class='alert-link'>Please start again</a>.";
            }
            SalesRepRepo salesRepRepo = new SalesRepRepo();
            return View(salesRepRepo.GetAll());
        }

        [HttpGet]
        public ActionResult DeleteEmail(string supplierID, string emailID)
        {
            EmailRepo emailRepo = new EmailRepo();
            emailRepo.DeleteEmail(Convert.ToInt32(emailID));
            return RedirectToAction("SupplierDetails", "Admin", new { supplierID = supplierID });
        }

        [HttpGet]
        public ActionResult EditEmail(string emailID)
        {
            EmailRepo emailRepo = new EmailRepo();
            return View(emailRepo.GetEmail(Convert.ToInt32(emailID)));
        }
        [HttpPost]
        public ActionResult EditEmail(EmailVM email)
        {
            EmailRepo emailRepo = new EmailRepo();
            emailRepo.UpdateEmail(email);
            return RedirectToAction("SupplierDetails", "Admin", new { supplierID = email.SupplierID });
        }

        public ActionResult ViewEmail(string emailID)
        {
            EmailRepo emailRepo = new EmailRepo();
            return View(emailRepo.GetEmail(Convert.ToInt32(emailID)));
        }

        [HttpGet]
        public ActionResult CreateEmail(string supplierID)
        {
            EmailRepo emailRepo = new EmailRepo();
            ViewBag.SupplierID = supplierID;
            return View();
        }
        [HttpPost]
        public ActionResult CreateEmail(string supplierID, EmailVM email)
        {
            EmailRepo emailRepo = new EmailRepo();
            emailRepo.CreateEmail(email);
            return RedirectToAction("SupplierDetails", "Admin", new { supplierID = email.SupplierID });
        }
    }
}