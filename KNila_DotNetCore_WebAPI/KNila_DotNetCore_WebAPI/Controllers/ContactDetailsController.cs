using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace KNila_DotNetCore_WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ContactDetailsController : ControllerBase
    {
        private SqlConnection con;
        private readonly IConfiguration _configuration;
        
        public ContactDetailsController (IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private void connection()
        {
            string constr = _configuration.GetConnectionString("getconn");
            con = new SqlConnection(constr);
        }

        /// <summary>
        /// Get Contact Details
        /// </summary>
        /// <returns>Contact Details List</returns>
        [HttpGet]
        public IEnumerable<ContactModel> GetContactDetails()
        {
            List<ContactModel> contact = new List<ContactModel>();
            connection();
            con.Open();
            SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM ContactDetails", con);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            DataTable dt = ds.Tables[0];
            sda.Dispose();

            foreach(DataRow dr in dt.Rows)
            {
                ContactModel cm = new ContactModel
                {
                    FirstName = dr["FirstName"].ToString(),
                    LastName = dr["LastName"].ToString(),
                    Email = dr["Email"].ToString(),
                    PhoneNumber = dr["PhoneNumber"].ToString(),
                    Address = dr["Address"].ToString(),
                    City = dr["City"].ToString(),
                    State = dr["State"].ToString(),
                    Country = dr["Country"].ToString(),
                    PostalCode = dr["PosalCode"].ToString()
                };
                contact.Add(cm);
            }
            con.Close();
            return contact;
        }

        /// <summary>
        /// Save Edi Details
        /// </summary>
        /// <param name="editContactDetails"></param>
        /// <returns>Success/Failure</returns>
        [HttpPost]
        public string SaveEditDetails (ContactModel editContactDetails)
        {
            connection();
            SqlCommand com = new SqlCommand("UpdateContactDetails", con);
            com.CommandType = System.Data.CommandType.StoredProcedure;
            com.Parameters.AddWithValue("@FirstName", editContactDetails.FirstName ?? "");
            com.Parameters.AddWithValue("@LastName", editContactDetails.LastName ?? "");
            com.Parameters.AddWithValue("@Email", editContactDetails.Email ?? "");
            com.Parameters.AddWithValue("@PhoneNumber", editContactDetails.PhoneNumber ?? "");
            com.Parameters.AddWithValue("@Address", editContactDetails.Address ?? "");
            com.Parameters.AddWithValue("@City", editContactDetails.City ?? "");
            com.Parameters.AddWithValue("@State", editContactDetails.State ?? "");
            com.Parameters.AddWithValue("@Country", editContactDetails.Country ?? "");
            com.Parameters.AddWithValue("@PostalCode", editContactDetails.PostalCode ?? "");
            con.Open();
            int i = com.ExecuteNonQuery();
            con.Close();
            if (i >= 1)
            {
                return "Success";
            }
            else
            {
                return "Failure";
            }
        }
    }
}