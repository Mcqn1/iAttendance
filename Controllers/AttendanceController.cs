using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Web.Mvc;
using System.Configuration;
using iAttendance.Services;
using System.Net;

namespace iAttendance.Controllers
{
    public class AttendanceController : Controller
    {
        [HttpPost]
        public JsonResult CaptureImage(string imageData, string ipAddress, double latitude, double longitude, string systemId, string deviceId)
        {
            Debug.WriteLine($"ImageData Length: {imageData?.Length ?? 0}");
            Debug.WriteLine($"IP Address Received: {ipAddress}");
            Debug.WriteLine($"Geolocation: Latitude={latitude}, Longitude={longitude}");
            Debug.WriteLine($"System ID: {systemId}");
            Debug.WriteLine($"Device ID: {deviceId}"); // Log the device ID

            // Get the client machine name (host name) from the IP address
            string clientPCName = string.Empty;
            try
            {
                string[] computerName = Dns.GetHostEntry(Request.ServerVariables["REMOTE_ADDR"]).HostName.Split(new char[] { '.' });
                clientPCName = computerName[0].ToString();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error obtaining client PC name: {ex.Message}");
            }

            Debug.WriteLine($"Client Machine Name: {clientPCName}");

            try
            {
                string strCropFileLocation = ConfigurationManager.AppSettings["CropFileLocation"];
                string strFileName = Guid.NewGuid().ToString() + ".png";
                byte[] imageBytes;

                if (imageData.StartsWith("data:image/png;base64,"))
                {
                    imageData = imageData.Substring("data:image/png;base64,".Length);
                }

                imageBytes = Convert.FromBase64String(imageData);

                using (var ms = new MemoryStream(imageBytes))
                {
                    var image = Image.FromStream(ms);
                    string imagePath = Path.Combine(strCropFileLocation, strFileName);
                    Directory.CreateDirectory(strCropFileLocation);
                    image.Save(imagePath, ImageFormat.Png);

                    FaceComparisonService objFace = new FaceComparisonService();
                    var comparisonResults = objFace.CompareFaces(imagePath, ipAddress, deviceId);

                    if (comparisonResults != null && comparisonResults.Count > 0)
                    {
                        var firstMatch = comparisonResults[0];

                        return Json(new
                        {
                            success = true,
                            EmployeeName = firstMatch.name,
                            Distance = firstMatch.distance ?? 0.0,
                            EmpNo = firstMatch.employeeNo,
                            status = firstMatch.status == 0 ? "OUT" : "IN",
                            IPAddress = ipAddress,
                            Latitude = latitude,
                            Longitude = longitude,
                            SystemId = systemId, // Return the system ID
                            DeviceId = deviceId, // Return the device ID
                            MachineName = clientPCName // Include the machine name in the response
                        });
                    }
                    else
                    {
                        return Json(new { success = false, message = "No matching faces found." });
                    }
                }
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Try Again" });
            }
        }

    }
}
