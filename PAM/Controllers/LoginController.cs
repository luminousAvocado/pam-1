using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PAM.Models;

namespace PAM.Controllers
{
    public class LoginController : Controller
    {


        //Intial method with a GET request from http and returns the view of the LOGIN
        [HttpGet]
        public IActionResult Login()
        {
            var view = View();
            view.ViewData["Login"] = "~/Views/Shared/_LoginLayout.cshtml";
            return view;
        }

        /*
        [HttpPost]
        public IActionResult Login(AD_Info ad)
        {
            Employee employee = new Employee();
            ad.domain = "ad.calstatela.edu";
            try
            {
                DirectoryEntry dirEntry = new DirectoryEntry("LDAP://" + ad.domain, ad.userName, ad.password);

                DirectorySearcher dirSearch = new DirectorySearcher(dirEntry, "(objectClass=user)");

                dirSearch.Filter = String.Format("(&(SAMAccountName={0}))", ad.userName);
                dirSearch.PropertiesToLoad.Add("givenName");
                dirSearch.PropertiesToLoad.Add("mail");
                dirSearch.PropertiesToLoad.Add("cn");                           //'Full Name + EmpNo
                dirSearch.PropertiesToLoad.Add("SAMAccountName");               //'EMPNO
                dirSearch.PropertiesToLoad.Add("givenName");                    //'First Name
                dirSearch.PropertiesToLoad.Add("sn");                           //'Last Name
                dirSearch.PropertiesToLoad.Add("mail");                         //'Email
                dirSearch.PropertiesToLoad.Add("title");                        //'job title
                dirSearch.PropertiesToLoad.Add("department");                   //'Department
                dirSearch.PropertiesToLoad.Add("telephoneNumber");              //'Telephone
                dirSearch.PropertiesToLoad.Add("physicalDeliveryOfficeName");   //'Area Office
                dirSearch.PropertiesToLoad.Add("streetAddress");                //'Area Office address
                dirSearch.PropertiesToLoad.Add("l");                            //'city
                dirSearch.PropertiesToLoad.Add("st");                           //'state
                dirSearch.PropertiesToLoad.Add("postalCode");                   //'zipcode
                dirSearch.PropertiesToLoad.Add("Manager");                      //'manager

                SearchResult objResult = dirSearch.FindOne();

                if (objResult.GetDirectoryEntry().Properties["samaccountname"].Value != null)
                {
                    employee.Employee_Number = "Username : " + objResult.GetDirectoryEntry().Properties["samaccountname"].Value.ToString();
                }

                if (objResult.GetDirectoryEntry().Properties["givenName"].Value != null)
                {
                    employee.First_Name = objResult.GetDirectoryEntry().Properties["givenName"].Value.ToString();
                }
                if (objResult.GetDirectoryEntry().Properties["sn"].Value != null)
                {
                    employee.Last_Name = objResult.GetDirectoryEntry().Properties["sn"].Value.ToString();
                }

                if (objResult.GetDirectoryEntry().Properties["mail"].Value != null)
                {
                    employee.Email = objResult.GetDirectoryEntry().Properties["mail"].Value.ToString();
                }
                if (objResult.GetDirectoryEntry().Properties["title"].Value != null)
                {
                    employee.Payroll_Title = objResult.GetDirectoryEntry().Properties["title"].Value.ToString();
                }
                if (objResult.GetDirectoryEntry().Properties["department"].Value != null)
                {
                    employee.Department = objResult.GetDirectoryEntry().Properties["department"].Value.ToString();
                }
                if (objResult.GetDirectoryEntry().Properties["telephoneNumber"].Value != null)
                {
                    employee.Cell_Phone = objResult.GetDirectoryEntry().Properties["telephoneNumber"].Value.ToString();
                }
                if (objResult.GetDirectoryEntry().Properties["physicalDeliveryOfficeName"].Value != null)
                {
                    employee.Employee_Number = objResult.GetDirectoryEntry().Properties["physicalDeliveryOfficeName"].Value.ToString();
                }
                if (objResult.GetDirectoryEntry().Properties["streetAddress"].Value != null)
                {
                    employee.Work_address = objResult.GetDirectoryEntry().Properties["streetAddress"].Value.ToString();
                }
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                return Content(ex.ToString());
            }
            Debug.WriteLine(employee.Email + ", " + employee.Admin_Flag + ", " + employee.Last_Name);
            return RedirectToAction("Profile", "Home", employee);
        }
        */
    }
}
