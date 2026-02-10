using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TutorsWorldFrontend.Models;

namespace TutorsWorldFrontend.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _apiBaseAddress = ConfigurationManager.AppSettings["ApiBaseAddress"];

        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> RegisterTutor(TutorDTO model)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseAddress);
                using (var content = new MultipartFormDataContent())
                {
                    // --- 1. Basic Fields (ADD ALL OF THESE) ---
                    content.Add(new StringContent(model.FullName ?? ""), "FullName");
                    content.Add(new StringContent(model.CNIC ?? ""), "CNIC");
                    content.Add(new StringContent(model.Gender ?? ""), "Gender");
                    content.Add(new StringContent(model.Age.ToString()), "Age");
                    content.Add(new StringContent(model.DOB.ToString("yyyy-MM-dd")), "DOB");
                    content.Add(new StringContent(model.Religion ?? ""), "Religion");
                    content.Add(new StringContent(model.Nationality ?? ""), "Nationality");
                    content.Add(new StringContent(model.MaritalStatus ?? ""), "MaritalStatus");
                    content.Add(new StringContent(model.City ?? ""), "City");
                    content.Add(new StringContent(model.Province ?? ""), "Province");
                    content.Add(new StringContent(model.Country ?? ""), "Country");
                    content.Add(new StringContent(model.Subjects ?? ""), "Subjects");
                    content.Add(new StringContent(model.ContactEmail ?? ""), "ContactEmail");
                    content.Add(new StringContent(model.ContactNo ?? ""), "ContactNo");
                    content.Add(new StringContent(model.PAddress ?? ""), "PAddress");
                    content.Add(new StringContent(model.TAddress ?? ""), "TAddress");
                    content.Add(new StringContent(model.Username ?? ""), "Username");
                    content.Add(new StringContent(model.Password ?? ""), "Password");
                    content.Add(new StringContent(model.TeachingSource ?? ""), "TeachingSource");
                    content.Add(new StringContent(model.FeeType ?? ""), "FeeType");
                    content.Add(new StringContent(model.TotalExperienceYears.ToString()), "TotalExperienceYears");

                    // --- 2. Qualifications ---
                    if (model.Qualifications != null)
                    {
                        for (int i = 0; i < model.Qualifications.Count; i++)
                        {
                            var q = model.Qualifications[i];
                            content.Add(new StringContent(q.Institute ?? ""), $"Qualifications[{i}].Institute");
                            content.Add(new StringContent(q.Degree ?? ""), $"Qualifications[{i}].Degree");
                            content.Add(new StringContent(q.PassingYear.ToString()), $"Qualifications[{i}].PassingYear");
                            content.Add(new StringContent(q.Percentage ?? ""), $"Qualifications[{i}].Percentage");
                        }
                    }

                    // --- 3. Experiences ---
                    if (model.Experiences != null)
                    {
                        for (int i = 0; i < model.Experiences.Count; i++)
                        {
                            var e = model.Experiences[i];
                            content.Add(new StringContent(e.ExpInstitute ?? ""), $"Experiences[{i}].ExpInstitute");
                            content.Add(new StringContent(e.ExpStart.ToString("yyyy-MM-dd")), $"Experiences[{i}].ExpStart");
                            content.Add(new StringContent(e.ExpEnd.ToString("yyyy-MM-dd")), $"Experiences[{i}].ExpEnd");
                            content.Add(new StringContent(e.ExpDuration ?? ""), $"Experiences[{i}].ExpDuration");
                        }
                    }

                    // --- 4. Classes ---
                    if (model.Classes != null)
                    {
                        for (int i = 0; i < model.Classes.Count; i++)
                        {
                            content.Add(new StringContent(model.Classes[i]), $"Classes[{i}]");
                        }
                    }

                    // --- 5. Files ---
                    if (model.ProfileImg != null)
                    {
                        var fileContent = new StreamContent(model.ProfileImg.InputStream);
                        content.Add(fileContent, "ProfileImg", model.ProfileImg.FileName);
                    }
                    if (model.ResumePdf != null)
                    {
                        var fileContent = new StreamContent(model.ResumePdf.InputStream);
                        content.Add(fileContent, "ResumePdf", model.ResumePdf.FileName);
                    }

                    var response = await client.PostAsync("api/Tutor/SaveTutor", content);
                    var result = await response.Content.ReadAsStringAsync();

                    return Json(new { success = response.IsSuccessStatusCode, message = result });
                }
            }
        }

        public async Task<ActionResult> GetTutors(int pageNumber = 1, int rowsPerPage = 9, string searchTerm = "", string gender = "", string maritalStatus = "", string teachingSources = "", string feeStructures = "", string classes = "")
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseAddress);

                var filterOptions = new
                {
                    PageNumber = pageNumber,
                    RowsPerPage = rowsPerPage,
                    SearchTerm = searchTerm,
                    Gender = gender,
                    MaritalStatus = maritalStatus,
                    TeachingSources = teachingSources,
                    FeeStructures = feeStructures,
                    Classes = classes
                };

                // Serialize filterOptions to JSON and send as StringContent
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(filterOptions);
                var contentPost = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/Tutor/GetTutors", contentPost);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Content(content, "application/json");
                }

                return Json(new { success = false, message = "Error fetching data from API" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult TutorDetails(long Id)
        {
            ViewBag.TutorID = Id;
            return View();
        }

        public async Task<ActionResult> GetTutorDetailsByIdProxy(long id)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_apiBaseAddress);
                    var response = await client.GetAsync($"api/Tutor/GetTutorDetails/{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        return Content(content, "application/json");
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public async Task<JsonResult> SubmitLogin(string Username, string Password)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseAddress);
                var loginData = new { Username = Username, Password = Password };

                // Manually serialize the object to JSON string
                var json = JsonConvert.SerializeObject(loginData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Use PostAsync instead
                var response = await client.PostAsync("api/Users/login", content);

                var resultText = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<dynamic>(resultText);

                if (response.IsSuccessStatusCode && result != null && (bool)result.success)
                {
                    var user = result.userdata;

                    HttpCookie userCookie = new HttpCookie("UserProfile");
                    // Use (string) cast for dynamic values to ensure they aren't null
                    userCookie["UserId"] = (string)user.id;
                    userCookie["FullName"] = (string)user.fullName;
                    userCookie["UserName"] = (string)user.userName;
                    userCookie["Role"] = (string)user.role;
                    userCookie["TutorId"] = (string)user.tutorId ?? "";
                    userCookie["StudentId"] = (string)user.studentId ?? "";
                    userCookie["GardianId"] = (string)user.gardianId ?? "";
                    userCookie["Token"] = (string)result.token;

                    userCookie.Expires = DateTime.Now.AddDays(2);
                    Response.Cookies.Add(userCookie);

                    return Json(new { success = true, redirectUrl = Url.Action("LoginUserScreen", "LoginUser") });
                }

                return Json(new { success = false, message = result?.message ?? "Login failed" });
            }
        }
        public ActionResult Logout()
        {
            if (Request.Cookies["UserProfile"] != null)
            {
                HttpCookie userCookie = new HttpCookie("UserProfile");
                userCookie.Expires = DateTime.Now.AddDays(-1); // Expire the cookie immediately
                Response.Cookies.Add(userCookie);
            }
            return RedirectToAction("Index", "Home");
        }
    

    [HttpPost]
        public async Task<JsonResult> HireTutorProxy(long tutorId)
        {
            try
            {
                // 1. Get Cookie
                var userCookie = Request.Cookies["UserProfile"];
                string token = userCookie["Token"]; // Get the token from cookie
                if (userCookie == null)
                {
                    return Json(new { success = false, redirectUrl = Url.Action("Login", "Home"), message = "Please login first." });
                }

                string role = userCookie["Role"]; // Check if you stored "0" or "Tutor"

                // 2. Validate Role
                if (role == "0" || role == "Tutor")
                {
                    return Json(new { success = false, redirectUrl = Url.Action("Register", "Home"), message = "Tutors cannot hire other tutors. Please register as a Student/Guardian." });
                }

                // 3. Prepare IDs based on Role
                long studentId = 0;
                long? guardianId = null;
                long? hiredByStudentId = null;

                if (role == "1" || role == "Student")
                {
                    studentId = Convert.ToInt64(userCookie["StudentId"]);
                    hiredByStudentId = studentId;
                }
                else if (role == "2" || role == "Gardian")
                {
                    // For Guardian, we assume they are hiring for a specific student 
                    // (You might need a StudentId from a dropdown, but for now we'll take the linked StudentId if available)
                    studentId = Convert.ToInt64(userCookie["StudentId"]);
                    guardianId = Convert.ToInt64(userCookie["GardianId"]);
                }

                // 4. Call Backend API
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_apiBaseAddress);
                    client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var hireData = new
                    {
                        StudentId = studentId,
                        TutorId = tutorId,
                        GuardianId = guardianId,
                        HiredByStudentId = hiredByStudentId
                    };

                    var json = JsonConvert.SerializeObject(hireData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync("api/Users/HireTutor", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return Json(new { success = true, message = "Tutor hired successfully!" });
                    }

                    var error = await response.Content.ReadAsStringAsync();
                    return Json(new { success = false, message = error });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }
    }
}
