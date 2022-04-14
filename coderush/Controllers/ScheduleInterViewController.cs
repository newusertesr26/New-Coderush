using coderush.Controllers;
using coderush.Data;
using coderush.Models;
using coderush.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coderush.Controllers
{
    public class ScheduleInterViewController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ScheduleInterViewController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context,
            IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _roleManager = roleManager;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
            //_hostingEnvironment = hostingEnvironment;
        }
        public IActionResult ScheduleIndex()
        {

            var data = new List<CandidateMastersViewModel>();

            DateTime? nulldate = null;


            data = (from candidate in _context.CandidateMaster.OrderByDescending(x => x.Id)
                    where !candidate.IsDelete && candidate.Status == 1

            let commentdate = _context.Comments.OrderByDescending(x => x.Id).Where(w => w.CandidateId == candidate.Id).Select(s => s.NextFollowUpdate).FirstOrDefault()
                  
                    select new CandidateMastersViewModel
                    {
                        Id = candidate.Id,
                        Name = candidate.Name,
                        Email = candidate.Email,
                        Phone = candidate.Phone,
                        Technologies = candidate.Technologies,
                        technologies = _context.Datamaster.Where(x => x.Id == candidate.Technologies).Select(x => x.Text).FirstOrDefault(),
                        filename = candidate.FileUpload,
                        IsActive = candidate.IsActive,
                        InterviewDate = candidate.InterviewDate,
                        PlaceOfInterview = candidate.PlaceOfInterview,
                        InterviewDescription = candidate.InterviewDescription,
                        InterviewTime = candidate.InterviewTime,
                        IsReject = candidate.IsReject,
                        Status = candidate.Status,
                        dateforNext = (commentdate != null ? commentdate : nulldate),
                        CreatedBy = _userManager.Users.Where(x => x.Id == candidate.CreatedBy).Select(x => x.FirstName + " " + x.LastName).FirstOrDefault(),
                        CreatedDate = candidate.CreatedDate,
                    
                     
                    }).ToList();
            return View(data);
        }
    }
}
