using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace iAttendance.Services
{
    public class DbConnectionReg
    {
        private string connectionString;

        // Constructor to initialize connection string
        public DbConnectionReg()
        {
            connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
        }

        // Method to open a MySQL connection
        public MySqlConnection GetConnection()
        {
            MySqlConnection con = new MySqlConnection(connectionString);
            return con;
        }

        //Method to get non query such as Insert,Update etc...
        public void ExecuteNonQuery(string query, MySqlParameter[] parameters)
        {
            using (MySqlConnection con = GetConnection())
            {
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
        }

        // Method to get single value such password etc...
        public object ExecuteScalar(string query, MySqlParameter[] parameters)
        {
            object result = null;
            using (MySqlConnection con = GetConnection())
            {
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    try
                    {
                        con.Open();
                        result = cmd.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }

            return result;
        }
    }

}
