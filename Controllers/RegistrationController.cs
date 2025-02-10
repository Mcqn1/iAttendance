using iAttendance.Models;
using iAttendance.Services;
using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace iAttendance.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly RegistrationService _registrationService;

        public RegistrationController()
        {
            _registrationService = new RegistrationService(); // Instantiate the correct service class
        }

        [HttpPost]
        public ActionResult RegisterFace(RegistrationModel model)
        {
            Debug.WriteLine("POST request received to Register action");

            // Log received data for debugging
            Debug.WriteLine("Employee No: " + model.EmployeeNo);
            Debug.WriteLine("Employee Name: " + model.EmployeeName);
            Debug.WriteLine("Image URL: " + model.StrImageURL);

            if (ModelState.IsValid)
            {
                Debug.WriteLine("Model is valid.");
                try
                {
                    string savedImagePath = null;
                    if (!string.IsNullOrEmpty(model.StrImageURL))
                    {
                        savedImagePath = SaveImage(model.StrImageURL, model.EmployeeNo); // Save image to a file
                    }

                    model.Base64Image = model.StrImageURL;

                    // Register employee and validate the face
                    string apiResponse = _registrationService.RegisterEmployee(model, savedImagePath);

                    SaveEmployeeToDatabase(model);

                    Debug.WriteLine("API Response: " + apiResponse);

                    if (apiResponse.Contains("Successfully registered"))
                    {
                        return Json(new { success = true, message = "Registration successful! Face detected." });
                    }
                    else
                    {
                        Debug.WriteLine("No face detected in the image.");
                        return Json(new { success = false, message = "Face not detected in the image." });
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error calling API: " + ex.Message);
                    return Json(new { success = false, message = "An error occurred while registering. Please try again." });
                }
            }
            else
            {
                Debug.WriteLine("Model is not valid.");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Debug.WriteLine("Validation Error: " + error.ErrorMessage);
                }
                return Json(new { success = false, message = "Registration failed. Please correct the errors and try again." });
            }
        }

        private string SaveImage(string base64Image, string employeeNo)
        {
            try
            {
                Debug.WriteLine("Base64 image string: " + base64Image.Substring(0, 100)); // Log only the first 100 characters

                var imageData = base64Image.Split(',')[1];
                byte[] imageBytes = Convert.FromBase64String(imageData);

                string strFileName = employeeNo + "_" + Guid.NewGuid().ToString() + ".png";
                string directoryPath = ConfigurationManager.AppSettings["ImageDirectory"];
                Debug.WriteLine("Image Directory Path from config: " + directoryPath);

                if (string.IsNullOrEmpty(directoryPath))
                {
                    Debug.WriteLine("ImageDirectory in config is null or empty.");
                    return null;
                }

                string imagePath = Path.Combine(directoryPath, strFileName);
                System.IO.File.WriteAllBytes(imagePath, imageBytes);
                Debug.WriteLine("Image saved successfully at: " + imagePath);

                return imagePath;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error saving image: " + ex.Message);
                return null;
            }
        }

        private void SaveEmployeeToDatabase(RegistrationModel model)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["RDBConnection"].ConnectionString;

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var command = new MySqlCommand("INSERT INTO register (EMP_NO, EMP_NAME, IMG) VALUES (@EmployeeNo, @EmployeeName, @Base64Image)", connection);
                command.Parameters.AddWithValue("@EmployeeNo", model.EmployeeNo);
                command.Parameters.AddWithValue("@EmployeeName", model.EmployeeName);
                command.Parameters.AddWithValue("@Base64Image", model.Base64Image);
                command.ExecuteNonQuery();
            }
        }
    }
}
