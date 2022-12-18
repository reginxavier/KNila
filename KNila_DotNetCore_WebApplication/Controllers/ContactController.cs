using System;
using System.Configuration;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using KNila_DotNetCore_WebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace KNila_DotNetCore_WebApplication.Controllers
{
    public class ContactController : Controller
    {
        private SqlConnection con;
        private readonly IConfiguration _configuration;

        public ContactController (IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// DB Connection
        /// </summary>
        private void connection()
        {
            string constr = _configuration.GetConnectionString("getconn");
            con = new SqlConnection(constr);
        }

        string BaseUrl = "http://localhost:54677/";
        static List<ContactModel> contact = new List<ContactModel>();

        /// <summary>
        /// Get the Contact Details
        /// </summary>
        /// <returns>Index Page</returns>
        [HttpGet]
        public async Task<ActionResult> GetContactDetails()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage result = await client.GetAsync("api/contactdetails/getcontactdetails");
                if(result.IsSuccessStatusCode)
                {
                    var response = result.Content.ReadAsStringAsync().Result;
                    contact = JsonConvert.DeserializeObject<List<ContactModel>>(response);
                }
                return View(contact);
            }
        }

        /// <summary>
        /// Add Contact Details
        /// </summary>
        /// <returns>Add Page</returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Error Page
        /// </summary>
        /// <returns>Error Page</returns>
        public ActionResult Error()
        {
            return View();
        }

        /// <summary>
        /// Add Contact Details
        /// </summary>
        /// <param name="contactDetails"></param>
        /// <returns>Add Contact Page</returns>
        public ActionResult CreateContact(ContactModel contactDetails)
        {
            if (AddContact(contactDetails))
            {
                contact.Add(contactDetails);
                return RedirectToAction("GetContactDetails");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Add Contact Details
        /// </summary>
        /// <param name="contactDetails"></param>
        /// <returns>Redirection to Home Page</returns>
        public bool AddContact(ContactModel contactDetails)
        {
            connection();
            SqlCommand com = new SqlCommand("InsertContactDetails", con);
            com.CommandType = System.Data.CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@FirstName", contactDetails.FirstName ?? "");
            com.Parameters.AddWithValue("@LastName", contactDetails.LastName ?? "");
            com.Parameters.AddWithValue("@Email", contactDetails.Email ?? "");
            com.Parameters.AddWithValue("@PhoneNumber", contactDetails.PhoneNumber ?? "");
            com.Parameters.AddWithValue("@Address", contactDetails.Address ?? "");
            com.Parameters.AddWithValue("@City", contactDetails.City ?? "");
            com.Parameters.AddWithValue("@State", contactDetails.State ?? "");
            com.Parameters.AddWithValue("@Country", contactDetails.Country ?? "");
            com.Parameters.AddWithValue("@PostalCode", contactDetails.PostalCode ?? "");
            con.Open();
            int i = com.ExecuteNonQuery();
            con.Close();
            if (i >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Edit the Contact
        /// </summary>
        /// <param name="firstName"></param>
        /// <returns>Edit Screen</returns>
        [HttpGet]
        public ActionResult Edit (string firstName)
        {
            ContactModel contactDetails = contact.Find(x => x.FirstName == firstName);
            return View(contactDetails);
        }

        /// <summary>
        /// Save Edit Details
        /// </summary>
        /// <param name="editContactDetails"></param>
        /// <returns>Returns Home Page</returns>
        [HttpPost]
        public ActionResult SaveEditDetails (ContactModel editContactDetails)
        {
            HttpResponseMessage result;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Clear();
                var content = JsonConvert.SerializeObject(editContactDetails);
                var buffer = System.Text.Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                result = client.PostAsync("api/contactdetails/saveeditdetails",byteContent).Result;
            }
            if (result.IsSuccessStatusCode)
            {
                return RedirectToAction("GetContactDetails");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
    }
}