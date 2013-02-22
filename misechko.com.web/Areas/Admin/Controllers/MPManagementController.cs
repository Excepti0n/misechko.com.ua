using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Compilation;
using System.Web.Mvc;
using System.Web.Security;
using MP.web.Application.Membership;
using Newtonsoft.Json;
using RadaCode.Web.Application.MVC;
using UnidecodeSharpFork;
using misechko.com.Application.Membership;
using misechko.com.Areas.Admin.Models;
using misechko.com.data.EF;
using misechko.com.data.Entities;

namespace misechko.com.Areas.Admin.Controllers
{
    [UrlAuthorize(Roles = "Administrator", AuthUrl = "~/Admin/Authorization/Authenticate")]
    public class MPManagementController : Controller
    {
        private readonly MPDataContext _context;
        private readonly MPSiteUserMembershipProvider _membershipProvider;
        private readonly MPUserRoleProvider _roleProvider;

        public MPManagementController(MPDataContext context, MPSiteUserMembershipProvider membershipProvider, MPUserRoleProvider roleProvider)
        {
            _context = context;
            _membershipProvider = membershipProvider;
            _roleProvider = roleProvider;
        }

        public ActionResult Index()
        {
            return View();
        }

        #region Users controller

        public ActionResult GetUsersControl()
        {
            var roleNamesArray = _roleProvider.GetAllRoles();

            var roleModels = roleNamesArray.Select(roleName => new RoleModel { RoleName = roleName, RoleUsersCount = _roleProvider.GetUsersInRole(roleName).Count(), AdminFeaturesAvailable = _roleProvider.DoesRoleHaveAnAdminFeatures(roleName) }).ToList();

            MembershipUserCollection users;

            var model = new RolesAndUsersModel
                            {
                                RoleModels = roleModels
                            };

            if (roleNamesArray.Length > 0)
            {
                users = _membershipProvider.GetAllUsersInRole(roleNamesArray[0]);

                foreach (MPMembershipUser user in users)
                {
                    user.Roles = _roleProvider.GetRolesForUser(user.UserName).ToList();
                }

                model.UsersInFirstRole = users.Cast<MPMembershipUser>().ToList();
            }

            return PartialView("_Users", model);
        }

        #region Roles

        [HttpPost]
        public ActionResult AddUserToRoles(string userName, string newRoles)
        {
            try
            {
                if (newRoles == null) return Json("SPCD: NORLPROVIDED");

                _roleProvider.ClearUserRoles(userName);

                var rolesList = JsonConvert.DeserializeObject<List<string>>(newRoles);
                foreach (var roleName in rolesList)
                {
                    _roleProvider.AddUserToRole(userName, roleName);
                }
            }
            catch (Exception ex)
            {
                return Json("SPCD: ERR - " + ex.Message);
            }

            return Json("SPCD: OK");
        }

        [HttpPost]
        public ActionResult AddNewRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName)) return Json("SPCD: NORLPROVIDED");

            try
            {
                _roleProvider.CreateRole(roleName);
            }
            catch (Exception ex)
            {
                return Json("SPCD: ERR - " + ex.Message);
            }

            return Json("SPCD: RLADDED");
        }

        [HttpPost]
        public ActionResult RemoveRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName)) return Json("SPCD: NORLPROVIDED");

            try
            {
                _roleProvider.DeleteRole(roleName, true);
            }
            catch (Exception ex)
            {
                return Json("SPCD: ERR - " + ex.Message);
            }

            return Json("SPCD: RLREMOVED");
        }

        [HttpGet]
        public JsonResult GetUsersInRole(string roleName)
        {
            var users = _membershipProvider.GetAllUsersInRole(roleName);

            foreach (MPMembershipUser user in users)
            {
                user.Roles = _roleProvider.GetRolesForUser(user.UserName).ToList();
            }

            return Json(new { 
                    status = "SPCD: OK", 
                    users = users.Cast<MPMembershipUser>().ToList() },
                JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region Users

        [HttpPost]
        public ActionResult UpdateDisplayName(string userName, string newDisplayName)
        {
            try
            {
                _membershipProvider.UpdateUserDisplayName(userName, newDisplayName);
            }
            catch(Exception ex)
            {
                return Json("SPCD: ERR - " + ex.Message);
            }

            return Json("SPCD: USRNMUPDATED");
        }

        [HttpPost]
        public ActionResult UpdateUserPassword(string userName, string newPass)
        {
            if (_membershipProvider.ChangePassword(userName, newPass)) return Json("SPCD: OK");
            else return Json("SPCD: FAIL");
        }

        [HttpPost]
        public ActionResult DeleteUser(string userName)
        {
            if (_membershipProvider.DeleteUser(userName, true)) return Json("SPCD: OK");
            else return Json("SPCD: FAIL");
        }

        [HttpPost]
        public ActionResult AddNewUser(string userName, string pass, string displayName, string email, string roles)
        {
            if (String.IsNullOrEmpty(roles)) return Json(new { status = "SPCD: ERR-NO-ROLES-PROVIDED" });

            MembershipCreateStatus status;
            var user = _membershipProvider.CreateUser(userName, pass, email, null, null, true, null, displayName, out status);
            if (status != MembershipCreateStatus.Success) return Json(new { status = "SPCD: ERR - " + status.ToString() });

            if (roles != null)
            {
                try
                {
                    var rolesList = JsonConvert.DeserializeObject<List<string>>(roles);
                    foreach (var roleName in rolesList)
                    {
                        _roleProvider.AddUserToRole(user.UserName, roleName);
                    }

                    user.Roles = _roleProvider.GetRolesForUser(user.UserName).ToList();
                }
                catch (Exception ex)
                {
                    return Json(new { status = "SPCD: ERR - " + ex.Message });
                }
            }

            return Json(new { status = "SPCD: OK", user });
        }

        #endregion

        #endregion

        #region Publications Controller

        public ActionResult GetPublicationsControl()
        {
            var pubModels = new List<PublicationModel>();

            foreach (var publication in _context.Publications.ToList())
            {
                pubModels.Add(new PublicationModel
                                  {
                                      Headline = publication.Headline,
                                      LinkPath = publication.LinkPath,
                                      PublishDate = publication.PublishDate.ToString("yyyy-MM-dd"),
                                      Id = publication.Id.ToString()
                                  });
            }

            var model = new PublicationsModel
            {
                Publications = pubModels
            };
            
            return PartialView("_Publications", model);
        }

        [HttpPost]
        public ActionResult DeletePublication(string id)
        {
            var gUq = Guid.Parse(id);

            var publicationToDelete = _context.Publications.FirstOrDefault(pb => pb.Id == gUq);

            if (publicationToDelete == null)
                return Json("SPCD: FAIL");
            
            _context.Publications.Remove(publicationToDelete);
            _context.SaveChanges();
            return Json("SPCD: OK");
        }

        [HttpPost]
        public ActionResult UpdatePublication(string id, string publicationName, string dateCreated)
        {
            var gUq = Guid.Parse(id);

            var publicationToUpdate = _context.Publications.FirstOrDefault(pb => pb.Id == gUq);

            if (publicationToUpdate == null)
                return Json("SPCD: FAIL");

            publicationToUpdate.Headline = publicationName;
            publicationToUpdate.LinkPath = MakeUrl(publicationName);
            publicationToUpdate.PublishDate = DateTime.ParseExact(dateCreated, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return Json(new { status = "SPCD: ERR - " + ex.Message });
            }

            return Json(new { status = "SPCD: OK", pub = publicationToUpdate });
            
        }

        [HttpPost]
        public ActionResult AddNewPublication(string publicationName)
        {
            var newPublication = new Publication
                                     {
                                         Headline = publicationName,
                                         PublishDate = DateTime.Now,
                                         LinkPath = "/Publication/" + MakeUrl(publicationName)
                                     };
            try
            {
                _context.Publications.Add(newPublication);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return Json(new { status = "SPCD: ERR - " + ex.Message });
            }

            return Json(new { status = "SPCD: PBADDED", pub = newPublication });
        }

        #endregion

        #region News Controller

        public ActionResult GetNewsControl()
        {
            var newsItemsModels = new List<NewModel>();

            foreach (var newItem in _context.News.ToList())
            {
                newsItemsModels.Add(new NewModel
                {
                    Headline = newItem.Headline,
                    LinkPath = newItem.LinkPath,
                    PublishDate = newItem.PublishDate.ToString("yyyy-MM-dd"),
                    Id = newItem.Id.ToString()
                });
            }

            var model = new NewsModel
            {
                News = newsItemsModels
            };

            return PartialView("_News", model);
        }

        [HttpPost]
        public ActionResult DeleteNewsItem(string id)
        {
            var gUq = Guid.Parse(id);

            var newsItemToDelete = _context.News.FirstOrDefault(pb => pb.Id == gUq);

            if (newsItemToDelete == null)
                return Json("SPCD: FAIL");

            _context.News.Remove(newsItemToDelete);
            _context.SaveChanges();
            return Json("SPCD: OK");
        }

        [HttpPost]
        public ActionResult UpdateNewsItem(string id, string newsItemName, string dateCreated)
        {
            var gUq = Guid.Parse(id);

            var newsItemToUpdate = _context.News.FirstOrDefault(pb => pb.Id == gUq);

            if (newsItemToUpdate == null)
                return Json("SPCD: FAIL");

            newsItemToUpdate.Headline = newsItemName;
            newsItemToUpdate.LinkPath = MakeUrl(newsItemName); 
            newsItemToUpdate.PublishDate = DateTime.ParseExact(dateCreated, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return Json(new { status = "SPCD: ERR - " + ex.Message });
            }

            return Json(new { status = "SPCD: OK", newsItem = newsItemToUpdate });

        }

        [HttpPost]
        public ActionResult AddNewNewsItem(string newsItemName)
        {
            var newNewsItem = new New
            {
                Headline = newsItemName,
                PublishDate = DateTime.Now,
                LinkPath = "/News/" + MakeUrl(newsItemName) 
            };
            try
            {
                _context.News.Add(newNewsItem);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return Json(new { status = "SPCD: ERR - " + ex.Message });
            }

            return Json(new { status = "SPCD: NIADDED", newsItem = newNewsItem });
        }

        #endregion

        #region LawNews Controller

        public ActionResult GetLawNewsControl()
        {
            var lawNewsItemsModels = new List<LawNewModel>();

            foreach (var lawNewItem in _context.LawNews.ToList())
            {
                lawNewsItemsModels.Add(new LawNewModel
                {
                    Headline = lawNewItem.Headline,
                    LinkPath = lawNewItem.LinkPath,
                    PublishDate = lawNewItem.PublishDate.ToString("yyyy-MM-dd"),
                    Id = lawNewItem.Id.ToString()
                });
            }

            var model = new LawNewsModel
            {
                LawNews = lawNewsItemsModels
            };

            return PartialView("_LawNews", model);
        }

        [HttpPost]
        public ActionResult DeleteLawNewsItem(string id)
        {
            var gUq = Guid.Parse(id);

            var lawNewsItemToDelete = _context.LawNews.FirstOrDefault(pb => pb.Id == gUq);

            if (lawNewsItemToDelete == null)
                return Json("SPCD: FAIL");

            _context.LawNews.Remove(lawNewsItemToDelete);
            _context.SaveChanges();
            return Json("SPCD: OK");
        }

        [HttpPost]
        public ActionResult UpdateLawNewsItem(string id, string lawNewsItemName, string dateCreated)
        {
            var gUq = Guid.Parse(id);

            var lawNewsItemToUpdate = _context.LawNews.FirstOrDefault(pb => pb.Id == gUq);

            if (lawNewsItemToUpdate == null)
                return Json("SPCD: FAIL");

            lawNewsItemToUpdate.Headline = lawNewsItemName;
            lawNewsItemToUpdate.LinkPath = MakeUrl(lawNewsItemName); 
            lawNewsItemToUpdate.PublishDate = DateTime.ParseExact(dateCreated, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return Json(new { status = "SPCD: ERR - " + ex.Message });
            }

            return Json(new { status = "SPCD: OK", lawNewsItem = lawNewsItemToUpdate });

        }

        [HttpPost]
        public ActionResult AddNewLawNewsItem(string lawNewsItemName)
        {
            var newLawNewsItem = new LawNew
            {
                Headline = lawNewsItemName,
                PublishDate = DateTime.Now,
                LinkPath = "/LawNews/" + MakeUrl(lawNewsItemName)
            };
            try
            {
                _context.LawNews.Add(newLawNewsItem);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return Json(new { status = "SPCD: ERR - " + ex.Message });
            }

            return Json(new { status = "SPCD: LNIADDED", newsItem = newLawNewsItem });
        }

        #endregion

        #region Awards Controller

        public ActionResult GetAwardsControl()
        {
            var awardsModels = new List<AwardModel>();

            foreach (var awardItem in _context.Awards.ToList())
            {
                awardsModels.Add(new AwardModel
                {
                    Headline = awardItem.Headline,
                    LinkPath = awardItem.LinkPath,
                    PublishDate = awardItem.PublishDate.ToString("yyyy-MM-dd"),
                    Id = awardItem.Id.ToString()
                });
            }

            var model = new AwardsModel
            {
                Awards = awardsModels
            };

            return PartialView("_Awards", model);
        }

        [HttpPost]
        public ActionResult DeleteAward(string id)
        {
            var gUq = Guid.Parse(id);

            var ItemToDelete = _context.Awards.FirstOrDefault(pb => pb.Id == gUq);

            if (ItemToDelete == null)
                return Json("SPCD: FAIL");

            _context.Awards.Remove(ItemToDelete);
            _context.SaveChanges();
            return Json("SPCD: OK");
        }

        [HttpPost]
        public ActionResult UpdateAward(string id, string awardName, string dateCreated)
        {
            var gUq = Guid.Parse(id);

            var awardToUpdate = _context.Awards.FirstOrDefault(pb => pb.Id == gUq);

            if (awardToUpdate == null)
                return Json("SPCD: FAIL");

            awardToUpdate.Headline = awardName;
            awardToUpdate.LinkPath = MakeUrl(awardName);
            awardToUpdate.PublishDate = DateTime.ParseExact(dateCreated, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return Json(new { status = "SPCD: ERR - " + ex.Message });
            }

            return Json(new { status = "SPCD: OK", award = awardToUpdate });

        }

        [HttpPost]
        public ActionResult AddNewAward(string awardName)
        {
            var newAward = new Award
            {
                Headline = awardName,
                PublishDate = DateTime.Now,
                LinkPath = "/Awards/" + MakeUrl(awardName)
            };
            try
            {
                _context.Awards.Add(newAward);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return Json(new { status = "SPCD: ERR - " + ex.Message });
            }

            return Json(new { status = "SPCD: AWADDED", award = newAward });
        }

        #endregion

        #region Brochures Controller

        public ActionResult GetBrochuresControl()
        {
            var brochuresModels = new List<BrochureModel>();

            foreach (var brochureItem in _context.Brochures.ToList())
            {
                brochuresModels.Add(new BrochureModel
                {
                    Headline = brochureItem.Headline,
                    LinkPath = brochureItem.LinkPath,
                    PublishDate = brochureItem.PublishDate.ToString("yyyy-MM-dd"),
                    Id = brochureItem.Id.ToString()
                });
            }

            var model = new BrochuresModel
            {
                Brochures = brochuresModels
            };

            return PartialView("_Brochures", model);
        }

        [HttpPost]
        public ActionResult DeleteBrochure(string id)
        {
            var gUq = Guid.Parse(id);

            var ItemToDelete = _context.Brochures.FirstOrDefault(pb => pb.Id == gUq);

            if (ItemToDelete == null)
                return Json("SPCD: FAIL");

            _context.Brochures.Remove(ItemToDelete);
            _context.SaveChanges();
            return Json("SPCD: OK");
        }

        [HttpPost]
        public ActionResult UpdateBrochure(string id, string brochureName, string dateCreated)
        {
            var gUq = Guid.Parse(id);

            var brochureToUpdate = _context.Brochures.FirstOrDefault(pb => pb.Id == gUq);

            if (brochureToUpdate == null)
                return Json("SPCD: FAIL");

            brochureToUpdate.Headline = brochureName;
            brochureToUpdate.LinkPath = MakeUrl(brochureName);
            brochureToUpdate.PublishDate = DateTime.ParseExact(dateCreated, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return Json(new { status = "SPCD: ERR - " + ex.Message });
            }

            return Json(new { status = "SPCD: OK", brochure = brochureToUpdate });

        }

        [HttpPost]
        public ActionResult AddNewBrochure(string brochureName)
        {
            var newBrochure = new Brochure
            {
                Headline = brochureName,
                PublishDate = DateTime.Now,
                LinkPath = "/Brochures/" + MakeUrl(brochureName)
            };
            try
            {
                _context.Brochures.Add(newBrochure);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return Json(new { status = "SPCD: ERR - " + ex.Message });
            }

            return Json(new { status = "SPCD: BRADDED", brochure = newBrochure });
        }

        #endregion

        #region Projects Controller

        public ActionResult GetProjectsControl()
        {
            var projectsModels = new List<ProjectModel>();

            foreach (var projectItem in _context.Projects.ToList())
            {
                projectsModels.Add(new ProjectModel
                {
                    Headline = projectItem.Headline,
                    LinkPath = projectItem.LinkPath,
                    PublishDate = projectItem.PublishDate.ToString("yyyy-MM-dd"),
                    Id = projectItem.Id.ToString()
                });
            }

            var model = new ProjectsModel
            {
                Projects = projectsModels
            };

            return PartialView("_Projects", model);
        }

        [HttpPost]
        public ActionResult DeleteProject(string id)
        {
            var gUq = Guid.Parse(id);

            var ItemToDelete = _context.Projects.FirstOrDefault(pb => pb.Id == gUq);

            if (ItemToDelete == null)
                return Json("SPCD: FAIL");

            _context.Projects.Remove(ItemToDelete);
            _context.SaveChanges();
            return Json("SPCD: OK");
        }

        [HttpPost]
        public ActionResult UpdateProject(string id, string projectName, string dateCreated)
        {
            var gUq = Guid.Parse(id);

            var projectToUpdate = _context.Projects.FirstOrDefault(pb => pb.Id == gUq);

            if (projectToUpdate == null)
                return Json("SPCD: FAIL");

            projectToUpdate.Headline = projectName;
            projectToUpdate.LinkPath = MakeUrl(projectName);
            projectToUpdate.PublishDate = DateTime.ParseExact(dateCreated, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return Json(new { status = "SPCD: ERR - " + ex.Message });
            }

            return Json(new { status = "SPCD: OK", project = projectToUpdate });

        }

        [HttpPost]
        public ActionResult AddNewProject(string projectName)
        {
            var newProject = new Project
            {
                Headline = projectName,
                PublishDate = DateTime.Now,
                LinkPath = "/Projects/" + MakeUrl(projectName)
            };
            try
            {
                _context.Projects.Add(newProject);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return Json(new { status = "SPCD: ERR - " + ex.Message });
            }

            return Json(new { status = "SPCD: PJADDED", project = newProject });
        }

        #endregion

        #region Practicies Controller

        public ActionResult GetPracticeItemsControl()
        {
            var practiceModels = new List<PracticeModel>();

            foreach (var practice in _context.Practicies.ToList())
            {
                var practiceModel = new PracticeModel
                {
                    Headline = practice.Headline,
                    LinkPath = practice.LinkPath,
                    PublishDate = practice.PublishDate.ToString("yyyy-MM-dd"),
                    Id = practice.Id.ToString()
                };

                var projectsForPractice = new List<ProjectModel>();
                var publicationsForPractice = new List<PublicationModel>();

                foreach (var publication in practice.Publications.ToList())
                {
                    publicationsForPractice.Add(new PublicationModel
                                                    {
                                                        Headline = practice.Headline,
                                                        LinkPath = practice.LinkPath,
                                                        PublishDate = practice.PublishDate.ToString("yyyy-MM-dd"),
                                                        Id = practice.Id.ToString()
                                                    });
                }

                foreach (var project in practice.Projects.ToList())
                {
                    projectsForPractice.Add(new ProjectModel
                                                {
                                                    Headline = practice.Headline,
                                                    LinkPath = practice.LinkPath,
                                                    PublishDate = practice.PublishDate.ToString("yyyy-MM-dd"),
                                                    Id = practice.Id.ToString()
                                                });
                }

                practiceModel.Projects = projectsForPractice;
                practiceModel.Publications = publicationsForPractice;
            }

            var model = new PracticiesModel
            {
                Practicies = practiceModels
            };

            return PartialView("_Practicies", model);
        }

        [HttpPost]
        public ActionResult DeletePractice(string id)
        {
            var gUq = Guid.Parse(id);

            var ItemToDelete = _context.Practicies.FirstOrDefault(pb => pb.Id == gUq);

            if (ItemToDelete == null)
                return Json("SPCD: FAIL");

            _context.Practicies.Remove(ItemToDelete);
            _context.SaveChanges();
            return Json("SPCD: OK");
        }

        [HttpPost]
        public ActionResult UpdatePractice(string id, string practiceName, string projectsInPractice, string publicationsInPractice)
        {
            var gUq = Guid.Parse(id);

            var practiceToUpdate = _context.Practicies.FirstOrDefault(pb => pb.Id == gUq);

            if (practiceToUpdate == null)
                return Json("SPCD: FAIL");



            practiceToUpdate.Headline = practiceName;
            practiceToUpdate.LinkPath = MakeUrl(practiceName);
            practiceToUpdate.PublishDate = DateTime.Now;

            var projectsList = JsonConvert.DeserializeObject<List<string>>(projectsInPractice);
            foreach (var projectId in projectsList)
            {
                var projectIdAsGuid = Guid.Parse(projectId);

                if (!practiceToUpdate.Projects.Any(pr => pr.Id != projectIdAsGuid))
                {
                    practiceToUpdate.Projects.Add(_context.Projects.SingleOrDefault(pr => pr.Id == projectIdAsGuid));
                }
            }

            var publicationsList = JsonConvert.DeserializeObject<List<string>>(publicationsInPractice);
            foreach (var publicationId in publicationsList)
            {
                var publicationIdAsGuid = Guid.Parse(publicationId);

                if (!practiceToUpdate.Publications.Any(pub => pub.Id != publicationIdAsGuid))
                {
                    practiceToUpdate.Publications.Add(_context.Publications.SingleOrDefault(pub => pub.Id == publicationIdAsGuid));
                }
            }

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return Json(new { status = "SPCD: ERR - " + ex.Message });
            }

            return Json(new { status = "SPCD: OK", practice = practiceToUpdate });

        }

        [HttpPost]
        public ActionResult AddNewPractice(string practiceName)
        {
           
            var newPractice = new Practice
            {
                Headline = practiceName,
                PublishDate = DateTime.Now,
                LinkPath = "/Practicies/" + MakeUrl(practiceName)
            };
            try
            {
                _context.Practicies.Add(newPractice);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return Json(new { status = "SPCD: ERR - " + ex.Message });
            }

            return Json(new { status = "SPCD: PRADDED", practice = newPractice });
        }

        #endregion

        #region Industries Controller

        public ActionResult GetIndustryItemsControl()
        {
            var industryModels = new List<IndustryModel>();

            foreach (var industry in _context.Industries.ToList())
            {
                var industryModel = new IndustryModel
                {
                    Headline = industry.Headline,
                    LinkPath = industry.LinkPath,
                    PublishDate = industry.PublishDate.ToString("yyyy-MM-dd"),
                    Id = industry.Id.ToString()
                };

                var projectsForIndustry = new List<ProjectModel>();
                var publicationsForIndustry = new List<PublicationModel>();

                foreach (var publication in industry.Publications.ToList())
                {
                    publicationsForIndustry.Add(new PublicationModel
                    {
                        Headline = industry.Headline,
                        LinkPath = industry.LinkPath,
                        PublishDate = industry.PublishDate.ToString("yyyy-MM-dd"),
                        Id = industry.Id.ToString()
                    });
                }

                foreach (var project in industry.Projects.ToList())
                {
                    projectsForIndustry.Add(new ProjectModel
                    {
                        Headline = industry.Headline,
                        LinkPath = industry.LinkPath,
                        PublishDate = industry.PublishDate.ToString("yyyy-MM-dd"),
                        Id = industry.Id.ToString()
                    });
                }

                industryModel.Projects = projectsForIndustry;
                industryModel.Publications = publicationsForIndustry;
            }

            var model = new IndustriesModel
            {
                Industries = industryModels
            };

            return PartialView("_Industries", model);
        }

        [HttpPost]
        public ActionResult DeleteIndustry(string id)
        {
            var gUq = Guid.Parse(id);

            var ItemToDelete = _context.Industries.FirstOrDefault(pb => pb.Id == gUq);

            if (ItemToDelete == null)
                return Json("SPCD: FAIL");

            _context.Industries.Remove(ItemToDelete);
            _context.SaveChanges();
            return Json("SPCD: OK");
        }

        [HttpPost]
        public ActionResult UpdateProject(string id, string industryName, string projectsInIndustry, string publicationsInIndustry)
        {
            var gUq = Guid.Parse(id);

            var industryToUpdate = _context.Industries.FirstOrDefault(pb => pb.Id == gUq);

            if (industryToUpdate == null)
                return Json("SPCD: FAIL");

            industryToUpdate.Headline = industryName;
            industryToUpdate.LinkPath = MakeUrl(industryName);
            industryToUpdate.PublishDate = DateTime.Now;

            var projectsList = JsonConvert.DeserializeObject<List<string>>(projectsInIndustry);
            foreach (var projectId in projectsList)
            {
                var projectIdAsGuid = Guid.Parse(projectId);

                if (!industryToUpdate.Projects.Any(pr => pr.Id != projectIdAsGuid))
                {
                    industryToUpdate.Projects.Add(_context.Projects.SingleOrDefault(pr => pr.Id == projectIdAsGuid));
                }
            }

            var publicationsList = JsonConvert.DeserializeObject<List<string>>(publicationsInIndustry);
            foreach (var publicationId in publicationsList)
            {
                var publicationIdAsGuid = Guid.Parse(publicationId);

                if (!industryToUpdate.Publications.Any(pub => pub.Id != publicationIdAsGuid))
                {
                    industryToUpdate.Publications.Add(_context.Publications.SingleOrDefault(pub => pub.Id == publicationIdAsGuid));
                }
            }

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return Json(new { status = "SPCD: ERR - " + ex.Message });
            }

            return Json(new { status = "SPCD: OK", industry = industryToUpdate });

        }

        [HttpPost]
        public ActionResult AddNewIndustry(string industryName)
        {
            var newIndustry = new Industry
            {
                Headline = industryName,
                PublishDate = DateTime.Now,
                LinkPath = "/Industries/" + MakeUrl(industryName)
            };
            try
            {
                _context.Industries.Add(newIndustry);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return Json(new { status = "SPCD: ERR - " + ex.Message });
            }

            return Json(new { status = "SPCD: PRADDED", industry = newIndustry });
        }

        #endregion

        #region Private Subroutines

        private string MakeUrl(String incoming)
        {
            if (incoming.Length > 40) incoming = incoming.Substring(0, 40);
            incoming = incoming.Unidecode();
            incoming = incoming.Replace(" ", "_");
            var alphaNum = Regex.Replace(incoming, @"[^A-Za-z0-9_]+", "");
            return alphaNum;

            char[] charsToTrim = { ' ', '\t', '/' };
            var transliteratedName = incoming.Unidecode();
            transliteratedName = transliteratedName.Trim(charsToTrim);
            if (transliteratedName.Length > 40 ) transliteratedName = transliteratedName.Substring(0, 40);
            return HttpUtility.UrlEncode(transliteratedName);
        }

        private Dictionary<string, List<string>> GetAllControllersAndActions()
        {
            var controllersAndActions = new Dictionary<string, List<string>>();

            foreach (var controller in GetControllers())
            {
                var newDictionaryEntry = new KeyValuePair<string, List<string>>(controller.Name, new List<string>()); 

                var controllerDescriptor = new ReflectedControllerDescriptor(controller);

                ActionDescriptor[] actions = controllerDescriptor.GetCanonicalActions();
                foreach (var action in actions)
                {
                    var paramSignatureString = GetParamSignatureString(action);
                    newDictionaryEntry.Value.Add(action.ActionName + paramSignatureString);
                    //controllersAndActions.Add(action.ControllerDescriptor.ControllerName + " -> " + action.ActionName + paramSignatureString);
                    
                }

                controllersAndActions.Add(newDictionaryEntry.Key, newDictionaryEntry.Value);
            }

            return controllersAndActions;
        }

        private string GetParamSignatureString(ActionDescriptor action)
        {
            var res = "(";

            ReflectedActionDescriptor aD = action as ReflectedActionDescriptor;

            foreach (var parameterDescriptor in aD.GetParameters())
            {
                res += parameterDescriptor.ParameterType.Name + " " + parameterDescriptor.ParameterName + ", ";
            }

            if(res == "(")
            {
                res += ")";
                return res;
            }

            if(res.Substring(res.Length - 2) == ", ")
            {
                res = res.Substring(0, res.Length - 2);
                res += ")";
            }

            return res;
        }

        private IEnumerable<Type> GetControllers()
        {
            IEnumerable<Type> typesSoFar = Type.EmptyTypes;
            var assemblies = BuildManager.GetReferencedAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                Type[] typesInAsm;
                try
                {
                    typesInAsm = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    typesInAsm = ex.Types;
                }
                typesSoFar = typesSoFar.Concat(typesInAsm);
            }
            return typesSoFar.Where(type =>
                                    type != null &&
                                    type.IsPublic &&
                                    type.IsClass &&
                                    !type.IsAbstract &&
                                    typeof (RadaCodeBaseController).IsAssignableFrom(type)
                //typeof(IController).IsAssignableFrom(type)
                );
        }

        private string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

       

        #endregion
    }
}
