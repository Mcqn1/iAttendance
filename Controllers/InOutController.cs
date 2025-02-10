//using iAttendance.Models;
//using iAttendance.Services;
//using System;
//using System.Collections.Generic;
//using System.Web.Mvc;

//namespace iAttendance.Controllers
//{
//    public class InOut : Controller
//    {
//        private const int PageSize = 10;

//        public ActionResult AttendanceReport(DateTime? startDate, DateTime? endDate, int page = 1)
//        {
//            InOut service = new InOut();
//            int pageSize = 10; // Number of records per page
//            int totalRecords;

//            List<AttendanceReportModel> reports = service.GetAttendanceReport(startDate, endDate, page, pageSize, out totalRecords);

//            var paginatedReport = new PaginatedAttendanceReport
//            {
//                Reports = reports,
//                TotalRecords = totalRecords,
//                PageSize = pageSize,
//                CurrentPage = page
//            };

//            return View(paginatedReport);
//        }

//    }
//}
