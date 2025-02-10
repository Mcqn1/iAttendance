using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using iAttendance.Models;

namespace iAttendance.Services
{
    public class FaceComparisonService
    {
        public List<FaceMatchResponse> CompareFaces(string strFileName, string ipAddress, string deviceId)
        {
            string webResponse = "";

            try
            {
                string strPythonAPIUrl = ConfigurationManager.AppSettings["APIServerName"];
                string uriWebAPI = strPythonAPIUrl + "FaceMatch";

                Uri uri = new Uri(uriWebAPI);
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/json; charset=utf-8";

                // Create an object for CompareFacesAPI with imgFileName, ipAddress, and deviceId
                CompareFacesAPI objprop = new CompareFacesAPI
                {
                    imgFileName = strFileName,
                    ipAddress = ipAddress,   // Pass the IP address to the API
                    deviceId = deviceId      // Pass the device ID to the API
                };

                // Serialize the object to JSON
                string inputJson = (new JavaScriptSerializer()).Serialize(objprop);
                byte[] bytes = Encoding.UTF8.GetBytes(inputJson);

                // Write the bytes to the request stream
                using (Stream stream = httpWebRequest.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }

                // Getting the response
                using (var response = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        webResponse = streamReader.ReadToEnd();
                        Debug.WriteLine("API Response: " + webResponse);
                    }
                }

                // Deserialize the response into a list of FaceMatchResponse objects
                var result = (new JavaScriptSerializer()).Deserialize<List<FaceMatchResponse>>(webResponse);

                // Return the list of matches if not empty
                if (result != null && result.Count > 0)
                {
                    return result;
                }
                else
                {
                    // Return an empty list if no matches are found
                    return new List<FaceMatchResponse>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in FaceComparisonService: " + ex.Message);
                throw;
            }
        }
    }

}





