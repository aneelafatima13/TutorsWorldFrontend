using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TutorsWorldFrontend.Models;

public class StudentController : Controller
{
    private readonly string _apiBaseAddress = ConfigurationManager.AppSettings["ApiBaseAddress"];

    [HttpPost]
    public async Task<ActionResult> RegisterStudent()
    {
        try
        {
            int studentAge = 0;
            DateTime studentDob;

            int.TryParse(Request.Form["sAge"], out studentAge);
            DateTime.TryParse(Request.Form["sDOB"], out studentDob);

            var student = new Student
            {
                FullName = Request.Form["sFullName"],
                Username = Request.Form["sUsername"],
                Password = Request.Form["sPassword"],
                CNIC = Request.Form["sCNIC"],
                BFormNo = Request.Form["sBformNo"],
                StudentGender = Request.Form["sGender"],            
            StudentMaritalStatus = Request.Form["sMaritalStatus"],  
            Age = studentAge,
                DOB = studentDob,
                Religion = Request.Form["sReligion"],
                Nationality = Request.Form["sNationality"],
                City = Request.Form["sCity"],
                Province = Request.Form["sProvince"],
                Country = Request.Form["sCountry"],
                ContactEmail = Request.Form["sContactEmail"],
                ContactNo = Request.Form["sContactNo"],
                PAddress = Request.Form["sPAddress"],
                TAddress = Request.Form["sTAddress"],
                SubjectsToTeach = Request.Form["sTargitSubjects"]
            };
            byte[] imageBytes = null;

            if (Request.Files["sProfileImg"] != null)
            {
                using (var ms = new MemoryStream())
                {
                    Request.Files["sProfileImg"].InputStream.CopyTo(ms);
                    imageBytes = ms.ToArray();
                }
            }

            student.ProfileImgBytes = imageBytes; 

            Gardian gardian = null;

            // ✅ Guardian ONLY if student is UNDER 18
            if (studentAge < 18)
            {
                int parentAge = 0;
                DateTime parentDob;

                int.TryParse(Request.Form["pAge"], out parentAge);
                DateTime.TryParse(Request.Form["pDOB"], out parentDob);

                gardian = new Gardian
                {
                    FullName = Request.Form["pFullName"],
                    Username = Request.Form["pUsername"],
                    Password = Request.Form["pPassword"],
                    CNIC = Request.Form["pCNIC"],
                    ParentGender = Request.Form["pGender"],
                    ParentMaritalStatus = Request.Form["pMaritalStatus"],
                    Age = parentAge,
                    DOB = parentDob,
                    Religion = Request.Form["pReligion"],
                    Nationality = Request.Form["pNationality"],
                    City = Request.Form["pCity"],
                    Province = Request.Form["pProvince"],
                    Country = Request.Form["pCountry"],
                    ContactEmail = Request.Form["pContactEmail"],
                    ContactNo = Request.Form["pContactNo"],
                    PAddress = Request.Form["pPAddress"],
                    TAddress = Request.Form["pTAddress"]
                };
            }

            var payload = new StudentRegistrationVM
            {
                Student = student,
                Gardian = gardian
            };

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiBaseAddress);

                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var apiResponse = await client.PostAsync("api/Student/register", content);
                var responseData = await apiResponse.Content.ReadAsStringAsync();

                if (!apiResponse.IsSuccessStatusCode)
                    return Json(new { success = false, message = responseData });

                return Json(new { success = true });
            }
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

}
