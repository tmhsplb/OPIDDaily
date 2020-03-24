using System.Web.Mvc;

namespace OPIDDaily.Controllers
{
    [Authorize(Roles = "Interviewer")]
    public class InterviewerController : SharedController 
    {
        public ActionResult Home()
        {
           // ServiceTicketBackButtonHelper("Reset", null);
            return View();
        }
    }
}