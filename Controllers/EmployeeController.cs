using System.Web.Mvc;
using iAttendance.Models;
using iAttendance.Services;

namespace iAttendance.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly EDbConnectionHelper dbHelper = new EDbConnectionHelper();

        // Action to display a single employee by employee number
        //public ActionResult EmployeeDetails(int empNo)
        //{
        //    // Fetch a single employee based on the provided empNo
        //    EmployeeModel employee = dbHelper.GetEmployeeById(empNo);

        //    if (employee == null)
        //    {
        //        ViewBag.ErrorMessage = "No employee found with the given employee number.";
        //    }

        //    return View(employee);
        //}
    }
}
