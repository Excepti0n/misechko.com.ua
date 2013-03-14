using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using RadaCode.Web.Application.MVC;
using misechko.com.Models;
using misechko.com.data.EF;

namespace misechko.com.Controllers
{
    public class TeamController : RadaCodeBaseController
    {
        private readonly MPDataContext _context;

        public TeamController(MPDataContext context)
        {
            _context = context;
        }

        //
        // GET: /Team/

        public ActionResult Index(string employee)
        {
            var key = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName + "/Team";

            if (!string.IsNullOrEmpty(employee))
            {
                key += "/" + employee;
            }

            key += "#main-content";

            var model = new TeamViewModel
            {
                CurrentEmployeeName = employee,
                AllEmployees = _context.TeamMembers.Where(
                                        i => i.Culture == _curCult || String.IsNullOrEmpty(i.Culture)).
                                    ToList().Select(pr => new EmployeeMenuViewModel()
                                    {
                                        DisplayText = pr.Headline,
                                        Slug = pr.LinkPath,
                                        Index = pr.ListWeight
                                    }).ToList()
            };

            model.AllEmployees.Sort((a, b) => a.Index.CompareTo(b.Index));


            var firstOrDefault = _context.ContentElements.FirstOrDefault(c => c.ContentKey == key);
            if (firstOrDefault != null)
            {
                model.MainMarkup = firstOrDefault.ContentMarkup;
            }
            return View(model);
        }

    }
}
