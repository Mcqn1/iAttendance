using iAttendance.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace iAttendance.Services
{
    public class InOut
    {
        private string connectionString;

        public InOut()
        {
            // Connection string from the configuration file
            connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
        }

       public List<AttendanceModel> GetAttendanceReport(DateTime? startDate, DateTime? endDate)
        {
            var attendanceReports = new List<AttendanceModel>();

            string query = @"
                SELECT 
                    EMP_NO, 
                    EMP_NAME, 
                    COUNT(CASE WHEN IN_OUT_STATUS = 1 THEN 1 END) AS TotalIn,
                    COUNT(CASE WHEN IN_OUT_STATUS = 0 THEN 1 END) AS TotalOut
                FROM attendances
                WHERE (@StartDate IS NULL OR IN_TIME >= @StartDate)
                  AND (@EndDate IS NULL OR IN_TIME <= @EndDate)
                GROUP BY EMP_NO, EMP_NAME
                ORDER BY TotalIn DESC, TotalOut DESC;";

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(query, connection))
                    {
                        // Add optional parameters for filtering by date range
                        command.Parameters.AddWithValue("@StartDate", startDate.HasValue ? (object)startDate.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@EndDate", endDate.HasValue ? (object)endDate.Value : DBNull.Value);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var report = new AttendanceModel
                                {
                                    EmpNo = reader.GetInt32(0),
                                    EmpName = reader.GetString(1),
                                    TotalIn = reader.GetInt32(2),
                                    TotalOut = reader.GetInt32(3)
                                };
                                attendanceReports.Add(report);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle errors (log or display them)
                Console.WriteLine("Error generating report: " + ex.Message);
            }

            return attendanceReports;
        }
    }
}
