namespace TutorsWorldFrontend.Models
{
    public class LoginUser
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public int Type { get; set; }

        public string Role
        {
            get
            {
                if (Type == 0) return "Tutor";
                if (Type == 1) return "Student";
                if (Type == 2) return "Gardian";
                return "Unknown";
            }
        }

        public long? StudentId { get; set; }
        public long? TutorId { get; set; }
        public long? GardianId { get; set; }


    }

    public class HireRequest
    {
        public long StudentId { get; set; }
        public long TutorId { get; set; }
        public long? GuardianId { get; set; }
        public long? HiredByStudentId { get; set; }
    }
}