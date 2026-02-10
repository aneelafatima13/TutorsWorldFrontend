using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TutorsWorldFrontend.Models
{
    // The main container for the incoming request
    public class TutorDTO
    {
        // General Info
        public string FullName { get; set; }
        public string CNIC { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public DateTime DOB { get; set; }
        public string Religion { get; set; }
        public string Nationality { get; set; }
        public string MaritalStatus { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public string Subjects { get; set; }
        public string ContactEmail { get; set; }
        public string ContactNo { get; set; }
        public string PAddress { get; set; }
        public string TAddress { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string TeachingSource { get; set; }
        public string FeeType { get; set; }
        public int TotalExperienceYears { get; set; }

        // Files (For Controller Binding)
        public IFormFile IProfileImg { get; set; }
        public IFormFile IResumePdf { get; set; }
              public HttpPostedFileBase ProfileImg { get; set; }
            public HttpPostedFileBase ResumePdf { get; set; }

            // Use these for binding
            public List<string> Classes { get; set; } = new List<string>();
            public List<QualificationDetail> Qualifications { get; set; } = new List<QualificationDetail>();
            public List<ExperienceDetail> Experiences { get; set; } = new List<ExperienceDetail>();
        

        // Flat lists for UI-to-UI transfer (Match your HTML names)
        public List<string> Institute { get; set; }
        public List<string> Degree { get; set; }
        public List<int> PassingYear { get; set; }
        public List<string> Percentage { get; set; }
        public List<string> ExpInstitute { get; set; }
        public List<DateTime> ExpStart { get; set; }
        public List<DateTime> ExpEnd { get; set; }
        public List<string> ExpDuration { get; set; }
    }
    public class QualificationDetail
    {
        public string Institute { get; set; }
        public string Degree { get; set; }
        public int PassingYear { get; set; }
        public string Percentage { get; set; }
    }

    public class ExperienceDetail
    {
        public string ExpInstitute { get; set; }
        public DateTime ExpStart { get; set; }
        public DateTime ExpEnd { get; set; }
        public string ExpDuration { get; set; }
    }

    public class TutorClasses
    {
        public string ClassName { get; set; }
        public object TutorID { get; internal set; }
    }

    public class PaginatedTutorResponse
    {
        public List<TutorProfile> Tutors { get; set; } = new List<TutorProfile>();
        public int TotalCount { get; set; }
    }

    public class TutorProfile : TutorDTO
    {
        // These lists will hold the data from the other result sets
        public List<TutorClasses> Classes { get; set; } = new List<TutorClasses>();
        public List<QualificationDetail> Qualifications { get; set; } = new List<QualificationDetail>();
        public List<ExperienceDetail> Experiences { get; set; } = new List<ExperienceDetail>();
        public long TutorID { get; set; }
    }
}