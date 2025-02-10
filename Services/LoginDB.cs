using MySql.Data.MySqlClient;
using System;
using System.Configuration;

namespace iAttendance.Services
{
    public class DbConnectionHelper
    {
        private string connectionString;

        public DbConnectionHelper()
        {
            connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
        }

      
        public MySqlConnection GetConnection()
        {
            MySqlConnection con = new MySqlConnection(connectionString);
            return con;
        }

        
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
