using System.Web.Mvc;
using iAttendance.Models; 
using iAttendance.Services;
using MySql.Data.MySqlClient;

namespace iAttendance.Controllers
{
    public class LoginController : Controller
    {
        private readonly DbConnectionHelper _dbConnectionHelper;

        // Constructor to initialize DbConnectionHelper
        public LoginController()
        {
            _dbConnectionHelper = new DbConnectionHelper();
        }

        // GET: Login page
        [HttpGet]
        public ActionResult Index()
        {
            return View("~/Views/Home/Login.cshtml");
        }

        // POST: Handle Login


        public ActionResult Login(LoginView model)
        {
            // Check if the model state is valid
            if (ModelState.IsValid)
            {
                // SQL query to get the stored password from the table
                string query = "SELECT PASSWORD FROM log_in WHERE USER_NAME = @Username";

                // Parameters for query
                MySqlParameter[] parameters = new MySqlParameter[]
                {
                    new MySqlParameter("@Username", model.Username)
                };

                // Fetch the stored password from the DB
                var storedPassword = _dbConnectionHelper.ExecuteScalar(query, parameters)?.ToString();

                // Validation of password
                if (!string.IsNullOrEmpty(storedPassword) && model.Password == storedPassword)
                {
                    // Password match - Redirect to Main page
                    return RedirectToAction("Main", "Home");
                }
                else
                {
                    // Error Message: Invalid username or password
                    ModelState.AddModelError("", "Invalid username or password. Please try again.");
                }
            }


            return View("~/Views/Home/Login.cshtml", model);
        }
    }
}
