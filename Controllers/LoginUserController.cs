using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace TutorsWorldFrontend.Controllers
{
    public class LoginUserController : Controller
    {
        private readonly string _apiBaseAddress = ConfigurationManager.AppSettings["ApiBaseAddress"];
        
        // GET: LoginUser/LoginUserScreen
        public ActionResult LoginUserScreen()
        {
            // Optional: Secure the page at the server level
            if (Request.Cookies["UserProfile"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> GetProfileDataProxybyId(long id, string role)
        {
            try
            {
                // 1. Get data from Cookies on the Server side
                var cookie = Request.Cookies["UserProfile"];
                string token = cookie["Token"];
                if (cookie == null) return Json(new { success = false, message = "Not authorized" }, JsonRequestBehavior.AllowGet);

                string apiUrl = "";

                // 2. Determine which API to call based on cookie data
                if (role == "Tutor")
                    apiUrl = $"api/Users/GetTutorDetails/{id}";
                else if (role == "Student" || role == "Gardian")
                    apiUrl = $"api/Users/GetStudentDetails/{id}";
                
                if (string.IsNullOrEmpty(apiUrl)) return HttpNotFound();

                // 3. Call the Backend Web API
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_apiBaseAddress);
                    client.DefaultRequestHeaders.Authorization =
           new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        // Return the raw JSON from the API to your Frontend AJAX
                        return Content(content, "application/json");
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false, message = "Could not fetch profile" }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public async Task<ActionResult> GetConnectionsProxy()
        {
            try
            {
                var cookie = Request.Cookies["UserProfile"];
                if (cookie == null) return Json(new { success = false, message = "Session expired" }, JsonRequestBehavior.AllowGet);

                string token = cookie["Token"];
                string role = cookie["Role"];
                string userId = (role == "Tutor") ? cookie["TutorId"] : (role == "Student") ? cookie["StudentId"] : cookie["GardianId"];

                // USE NEW HTTPCLIENT INSTEAD OF FACTORY
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_apiBaseAddress);
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    var response = await client.GetAsync($"api/Users/GetConnections/{userId}/{role}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        return Content(content, "application/json");
                    }
                }
                return Json(new { success = false, message = "API call failed" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
