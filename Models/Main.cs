using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace iAttendance.Models
{
    
        public class LoginView
        {
            [Required]
            public string Username { get; set; }

            [Required]
            public string Password { get; set; }
        }

        public class RegistrationModel
        {
            [Required]
            public string EmployeeNo { get; set; }
            [Required]
            public string EmployeeName { get; set; }
            public string StrImageURL { get; set; }
            public string Base64Image { get; set; }
        }

        public class ComparisonResult
        {
            public string EmployeeName { get; set; }
            public string status { get; set; } // This should be 'IN' or 'OUT'  
    }

        public class CompareFacesAPI
        {
            public string imgFileName { get; set; }
            public string ipAddress { get; set; } // Add the ipAddress property
            public string deviceId { get; set; }
    }

        public class FaceMatchResponse
        {
            public string name { get; set; }
            public int status { get; set; }
            public string employeeNo { get; set; }
            public bool success { get; set; }
            public double? distance { get; set; }
        }




        public class EmployeeModel
        {
            public int Emp_No { get; set; }
            public string Emp_Name { get; set; }
            public string Img { get; set; } // Assume IMG is stored as Base64 or a URL
        }

    public class AttendancetModel
    {
        public int EmpNo { get; set; }
        public string EmpName { get; set; }
        public int TotalIn { get; set; }
        public int TotalOut { get; set; }
    }
    public class PaginatedAttendanceReport
    {
        public IEnumerable<AttendancetModel> Reports { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }

}








