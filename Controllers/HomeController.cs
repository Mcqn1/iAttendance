using System.Collections.Generic;
using System.Web.Mvc;
using iAttendance.Models;
using iAttendance.Services;

namespace iAttendance.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        public ActionResult Main()
        {
            return View();
        }

        public ActionResult Registration()
        {
            return View();
        }

        public ActionResult Attendance()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Employee()
        {
            EDbConnectionHelper objServ = new EDbConnectionHelper();
            List<EmployeeModel> objModel = objServ.GetEmployeeById();

            return View("~/Views/Home/Employee.cshtml", objModel);
        }

        //public ActionResult InOut()
        //{
        //    InOut objServ = new InOut();
        //    List<AttendanceModel> objModel = objServ.GetAttendanceLog(page: 1); // Pass page number dynamically as needed
        //    return View("~/Views/Home/InOut.cshtml", objModel);
        //}
       

        public ActionResult WebPage()
        {
            return View();
        }
    }
}
