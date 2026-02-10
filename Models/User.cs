using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace TutorsWorldFrontend.Models
{
 
    public abstract class User
    {
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string CNIC { get; set; }
        public string ContactNo { get; set; }
        public string City { get; set; }

        public string Province { get; set; }
        public string Country { get; set; }
        public string PAddress { get; set; }
        public string TAddress { get; set; }
        
        public int Age { get; set; }
        public DateTime DOB { get; set; }
        public string ContactEmail { get; set; }
        public string Religion { get; set; }
        public string Nationality { get; set; }
        
    }

    public class Student : User
    {
        public string StudentGender { get; set; }
        public string StudentMaritalStatus { get; set; }
        public string BFormNo { get; set; }
        public string RollNo { get; set; }
        public string SubjectsToTeach { get; set; }
        public string GardianId { get; set; }

        // Encapsulation: Logic for image handling
        public HttpPostedFileBase ProfileImg { get; set; }
        public byte[] ProfileImgBytes { get; set; }

        public string ProfileImgPath { get; set; }

        
    }

    public class Gardian : User
    {
        public string ParentGender { get; set; }
        public string ParentMaritalStatus { get; set; }
        public string StudentId { get; set; }

    }

    public class StudentRegistrationVM
    {
        // Student
        public Student Student { get; set; }

        // Guardian (nullable if age < 18)
        public Gardian Gardian { get; set; }
    }

}
