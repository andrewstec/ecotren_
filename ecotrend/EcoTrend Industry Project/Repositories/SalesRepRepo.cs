using EcoTrend_Industry_Project.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcoTrend_Industry_Project.Repositories
{
    public class SalesRepRepo
    {
        public const string LASTNAME = "LastName";
        public const string DATE = "Date";
        public const string DATE_DESC = "Date_desc";

        EcotrendEntities context = new EcotrendEntities();
        public IEnumerable<SalesRepVM> GetAll()
        {
            IEnumerable<SalesRepVM> salesReps;

            salesReps = from s in context.SalesReps
                             select new SalesRepVM()
                             {
                                SalesRepID = s.salesRepID,
                                Name = s.name,
                                Branch = s.branch,
                                Email = s.email
                             };
            return salesReps;
        }

        IEnumerable<SalesRepVM> SortSalesReps(IEnumerable<SalesRepVM> salesReps, string sortOrder)
        {
            switch (sortOrder)
            {
                case LASTNAME:
                    salesReps = salesReps.OrderByDescending(s => s.SalesRepID);
                    break;
                case DATE:
                    salesReps = salesReps.OrderBy(s => s.Branch);
                    break;
                case DATE_DESC:
                    salesReps = salesReps.OrderByDescending(s => s.Name);
                    break;
                default:
                    salesReps = salesReps.OrderByDescending(s => s.SalesRepID);
                    break;
            }
            return salesReps;
        }

        IEnumerable<SalesRepsLockoutVM> SortSalesReps(IEnumerable<SalesRepsLockoutVM> salesReps, string sortOrder)
        {
            switch (sortOrder)
            {
                case LASTNAME:
                    salesReps = salesReps.OrderByDescending(s => s.Id);
                    break;
                case DATE:
                    salesReps = salesReps.OrderBy(s => s.Branch);
                    break;
                case DATE_DESC:
                    salesReps = salesReps.OrderByDescending(s => s.Name);
                    break;
                default:
                    salesReps = salesReps.OrderByDescending(s => s.Id);
                    break;
            }
            return salesReps;
        }

        public IEnumerable<SalesRepVM> GetSalesRepsSort(string searchString, string sortOrder)
        {

            IEnumerable<SalesRepVM> salesReps = from s in context.SalesReps
                        select new SalesRepVM()
                        {
                            SalesRepID = s.salesRepID,
                            Name = s.name,
                            Branch = s.branch,
                            Email = s.email
                        };

            salesReps = FilterSalesReps(salesReps, searchString);
            salesReps = SortSalesReps(salesReps, sortOrder);

            return salesReps;
        }

        IEnumerable<SalesRepVM> FilterSalesReps(IEnumerable<SalesRepVM> salesReps, string searchString)
        {

            //int? searchStringParsed;
            //try
            //{
            //    searchStringParsed = int.Parse(searchString);
            //    if (searchString != null)
            //        salesReps = salesReps.Where(s =>
            //                   s.SalesRepID == searchStringParsed);
            //}
            //catch
            //{
            //    // salesReps;
            //}

            // Filter results based on search.
            if (!String.IsNullOrEmpty(searchString))
                salesReps = salesReps.Where(
                            s => s.Branch.Contains(searchString.ToUpper())
                              || s.Name.ToUpper().Contains(searchString.ToUpper()) || s.Email.ToUpper().Contains(searchString.ToUpper()));


            return salesReps;
        }

        IEnumerable<SalesRepsLockoutVM> FilterSalesReps(IEnumerable<SalesRepsLockoutVM> salesReps, string searchString)
        {

            //int? searchStringParsed;
            //try
            //{
            //    searchStringParsed = int.Parse(searchString);
            //    if (searchString != null)
            //        salesReps = salesReps.Where(s =>
            //                   s.SalesRepID == searchStringParsed);
            //}
            //catch
            //{
            //    // salesReps;
            //}

            // Filter results based on search.
            if (!String.IsNullOrEmpty(searchString))
                salesReps = salesReps.Where(
                            s => s.Branch.Contains(searchString.ToUpper())
                              || s.Name.ToUpper().Contains(searchString.ToUpper()) || s.Email.ToUpper().Contains(searchString.ToUpper()));


            return salesReps;
        }

        public IEnumerable<SalesRep> GetAllSalesReps()
        {
            IEnumerable<SalesRep> salesReps;

            salesReps = from s in context.SalesReps
                        select s;
            return salesReps;
        }

        public SalesRepVM GetSalesRep(int salesRepID)
        {
            SalesRepVM salesReps;

            salesReps = (from s in context.SalesReps
                        where s.salesRepID == salesRepID
                        select new SalesRepVM()
                        {
                            SalesRepID = s.salesRepID,
                            Name = s.name,
                            Email = s.email,
                            Branch = s.branch
                        }).FirstOrDefault();
            return salesReps;
        }

        public IEnumerable<SalesRepsLockoutVM> GetAllSalesRepsLockoutStatus()
        {
            IEnumerable<SalesRepsLockoutVM> salesReps;


            salesReps = from s in context.SalesReps
                        select new SalesRepsLockoutVM()
                        {
                            Id = s.salesRepID,
                            Name = s.name,
                            Branch = s.branch,
                            Email = s.email,
                            Date = (from c in context.AspNetUsers
                                   where s.email == c.Email
                                   select c.LockoutEndDateUtc).FirstOrDefault()
                        };
            return salesReps;
        }
        public IEnumerable<SalesRepsLockoutVM> GetAllSalesRepsLockoutStatusSorted(string searchString, string sortOrder)
        {
            IEnumerable<SalesRepsLockoutVM> salesReps;


            salesReps = from s in context.SalesReps
                        select new SalesRepsLockoutVM()
                        {
                            Id = s.salesRepID,
                            Name = s.name,
                            Branch = s.branch,
                            Email = s.email,
                            Date = (from c in context.AspNetUsers
                                    where s.email == c.Email
                                    select c.LockoutEndDateUtc).FirstOrDefault()
                        };

            salesReps = FilterSalesReps(salesReps, searchString);
            salesReps = SortSalesReps(salesReps, sortOrder);

            return salesReps;
        }


        public IEnumerable<PasswordResetVM> GetAllSalesRepsLockoutDetails()
        {
            IEnumerable<PasswordResetVM> salesReps;


            salesReps = from s in context.SalesReps
                        select new PasswordResetVM()
                        {
                            Id = s.salesRepID,
                            Email = s.email,
                            AccessFailedCount = (from c in context.AspNetUsers
                                                 where s.email == c.Email
                                                 select c.AccessFailedCount).FirstOrDefault(),
                            UserRole = "Sales Representative",
                            LockoutStatus = (from c in context.AspNetUsers
                                    where s.email == c.Email
                                    select c.LockoutEndDateUtc).FirstOrDefault()
                        };
            return salesReps;
        }

        public void LockUser(string email) {
            AspNetUser aspNetUser = (from s in context.AspNetUsers
                       where s.Email == email
                       select s).FirstOrDefault();
            aspNetUser.LockoutEndDateUtc = DateTime.Today.AddYears(999);
            context.SaveChanges();
        }

        public void UnlockUser(string email)
        {
            AspNetUser aspNetUser = (from s in context.AspNetUsers
                                     where s.Email == email
                                     select s).FirstOrDefault();
            aspNetUser.LockoutEndDateUtc = null;
            context.SaveChanges();
        }

        public SalesRepDetailsVM GetSalesRepDetails(int salesRepID)
        {
            SalesRepDetailsVM salesReps;

            salesReps = (from s in context.SalesReps
                         where s.salesRepID == salesRepID
                         select new SalesRepDetailsVM()
                         {
                             SalesRepID = s.salesRepID,
                             Name = s.name,
                             Email = s.email,
                             Branch = s.branch,
                             RepsContacts = (from c in context.Contacts
                                             where salesRepID == c.salesRepID
                                             select new ContactVM()
                                             {
                                                 ContactID = c.contactID,
                                                 StoreName = c.Store.name,
                                                 FirstName = c.firstName,
                                                 LastName = c.lastName, 
                                                 Email = c.email,
                                                 Branch = c.branch,
                                                 Telephone = c.telephone
                                             })
                             
                         }).FirstOrDefault();
            return salesReps;
        }
        public void UpdateSalesRep(SalesRepVM salesRepVM)
        {
            SalesRep salesRep = (from s in context.SalesReps
                                 where s.salesRepID == salesRepVM.SalesRepID
                                 select s).FirstOrDefault();

            if (salesRepVM.Email != salesRep.email)
            {
                AspNetUser user = (from a in context.AspNetUsers
                                   where a.Email == salesRep.email
                                   select a).FirstOrDefault();
                user.Email = salesRepVM.Email;
                context.SaveChanges();
            }

            salesRep.branch = salesRepVM.Branch;
            salesRep.email = salesRepVM.Email;
            salesRep.name = salesRepVM.Name;

            context.SaveChanges();

        }
    }
}