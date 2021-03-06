﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using RadaCode.Web.Application.MVC;
using misechko.com.Areas.Admin.Models;
using misechko.com.Content;
using misechko.com.Models;
using misechko.com.core;
using misechko.com.data.EF;

namespace misechko.com.Controllers
{
    public class IndustriesController : RadaCodeBaseController
    {
        private readonly MPDataContext _context;
        private readonly IMPSettings _settings;

        public IndustriesController(MPDataContext context, IMPSettings settings)
        {
            _context = context;
            _settings = settings;
        }

        //
        // GET: /Industries/

        public ActionResult Index(string industry)
        {
            var model = new IndustryViewModel
                            {
                                AllIndustries =
                                    _context.Industries.Where(
                                        i => i.Culture == _curCult || String.IsNullOrEmpty(i.Culture)).
                                    ToList().Select(pr => new IndustryMenuViewModel()
                                                              {
                                                                  DisplayText = pr.Headline,
                                                                  Slug = pr.LinkPath,
                                                                  Index = pr.ListWeight
                                                              }).ToList()
                            };

            model.AllIndustries.Sort((a, b) => a.Index.CompareTo(b.Index));

            var key = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName + "/Industries";

            if (!string.IsNullOrEmpty(industry))
            {
                key += "/" + industry;
            }

            key += "#main-content";

            if (_settings.ShouldGoToFirstMenuItem && model.AllIndustries.Count > 0 && string.IsNullOrEmpty(industry))
            {
                return Redirect(model.AllIndustries.First().Slug);
            }

            model.CurrentIndustryName = industry;

            var industryItem = _context.Industries.SingleOrDefault(pr => pr.LinkPath == "/Industries/" + industry && pr.Culture == _curCult);
            if (industryItem != null && (industryItem.Projects.Count > 0 || industryItem.Publications.Count > 0))
            {
                model.HasSupportMaterials = true;

                model.Projects = new List<ProjectModel>();
                model.Publications = new List<PublicationModel>();

                model.Projects = industryItem.Projects.Select(prj => new ProjectModel
                {
                    Id = prj.Id.ToString(),
                    Index = prj.ListWeight,
                    Headline = prj.Headline,
                    LinkPath = "/Read" + prj.LinkPath,
                    PublishDate =
                        prj.PublishDate.ToString("yyyy-MM-dd"),
                    RelatesToPaths = prj.RelatesToPaths

                }).ToList();

                model.Publications = industryItem.Publications.Select(pub => new PublicationModel
                {
                    Id = pub.Id.ToString(),
                    Index = pub.ListWeight,
                    Headline = pub.Headline,
                    LinkPath = "/Read" + pub.LinkPath,
                    PublishDate =
                        pub.PublishDate.ToString("yyyy-MM-dd"),
                    RelatesToPaths = pub.RelatesToPaths

                }).ToList();

                model.Projects.Sort((a, b) => b.PublishDate.CompareTo(a.PublishDate));
                model.Projects = model.Projects.Take(3).ToList();
                model.Publications.Sort((a, b) => b.PublishDate.CompareTo(a.PublishDate));
                model.Publications = model.Publications.Take(3).ToList();

            }

            var firstOrDefault = _context.ContentElements.FirstOrDefault(c => c.ContentKey == key);
            if (firstOrDefault != null)
            {
                model.IndustryMarkup = firstOrDefault.ContentMarkup;
            }

            return View(model);
        }

    }
}
