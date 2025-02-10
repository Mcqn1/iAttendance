using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using iAttendance.Models;
using iAttendance.Services;
using MySql.Data.MySqlClient;
    
namespace iAttendance.Services
{
    public class RegistrationService
    {

        public class FaceRequest
        {
            public string filename { get; set; }
            public string employeeNo { get; set; }
            public string empName { get; set; }
        }

        // Method to register an employee
        public string RegisterEmployee(RegistrationModel model, string imagePath)
        {
            string webResponse = "";

            try
            {
                string fileName = Path.GetFileName(imagePath); // Extract the file name from the image path
                string strPythonAPIUrl = ConfigurationManager.AppSettings["APIServerName"];
                string apiUrl = strPythonAPIUrl + "RegisterFace"; // Flask API endpoint

                Uri uri = new Uri(apiUrl);
                WebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
                httpWebRequest.Method = "POST"; // Set request method to POST
                httpWebRequest.ContentType = "application/json; charset=utf-8"; // Set content type for JSON

                // Prepare the request object with employee data
                FaceRequest objprop = new FaceRequest
                {
                    filename = fileName,
                    employeeNo = model.EmployeeNo,
                    empName = model.EmployeeName
                };

                // Serialize the request object to JSON
                string inputJson = (new JavaScriptSerializer()).Serialize(objprop);
                byte[] bytes = Encoding.UTF8.GetBytes(inputJson); // Convert JSON string to byte array

                // Write the JSON to the request stream
                using (Stream stream = httpWebRequest.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length); // Write the data to the request stream
                }

                // Get the response from the API
                using (var response = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        webResponse = reader.ReadToEnd(); // Read the response content
                    }
                }
            }
            catch (WebException webEx)
            {

                using (var response = (HttpWebResponse)webEx.Response)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        string errorResponse = reader.ReadToEnd(); // Capture error response
                        Debug.WriteLine("WebException Response: " + errorResponse);
                    }
                }
                Debug.WriteLine("WebException in RegistrationService: " + webEx.Message); // Log web exception message
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in RegistrationService: " + ex.Message); // Log any general exceptions
                throw; // Re-throw the exception if needed
            }

            return webResponse;
        }
    }
}
