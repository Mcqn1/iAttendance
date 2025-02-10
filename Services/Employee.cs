using iAttendance.Models;
using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace iAttendance.Services
{
    public class EDbConnectionHelper
    {
        private readonly string connectionString;

        public EDbConnectionHelper()
        {
            connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
            Debug.WriteLine("Connection String: " + connectionString); 
        }

        public List<EmployeeModel> GetEmployeeById()
        {
            List<EmployeeModel> employeeList = new List<EmployeeModel>();

            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    Debug.WriteLine("Attempting to open MySQL connection..."); // Debugging output
                    connection.Open();
                    Debug.WriteLine("MySQL connection opened successfully."); // Debugging output

                    using (var command = new MySqlCommand("SELECT EMP_NO, EMP_NAME, IMG FROM register", connection))
                    {
                        //command.Parameters.AddWithValue("@EmpNo", empNo);
                        Debug.WriteLine("Executing query..."); // Debugging output

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                EmployeeModel employee = new EmployeeModel();
                                employee = new EmployeeModel
                                {
                                    Emp_No = reader.GetInt32("EMP_NO"),
                                    Emp_Name = reader.GetString("EMP_NAME"),
                                    Img = reader.IsDBNull(reader.GetOrdinal("IMG")) ? null : reader.GetString("IMG")
                                };

                                employeeList.Add(employee);
                                Debug.WriteLine("Employee found: " + employee.Emp_Name); // Debugging output
                            }
                            
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    Debug.WriteLine("MySQL Exception: " + ex.Message);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("General Exception: " + ex.Message);
                }
            }

            return employeeList;
        }
    }
}
