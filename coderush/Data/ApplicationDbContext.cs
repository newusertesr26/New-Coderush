using coderush.Models;
using CodesDotHRMS.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace coderush.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //custom entity, override identity user with new column
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        //custom entity, for simple todo app
        public DbSet<Todo> Todo { get; set; }
        public DbSet<DataMaster> Datamaster { get; set; }
        public DbSet<CandidateMaster> CandidateMaster { get; set; }
        public DbSet<ExpenseMaster> ExpenseMaster { get; set; }
        public DbSet<InvoiceMaster> InvoiceMaster { get; set; }
        public DbSet<ProjectMaster> ProjectMaster { get; set; }
        public DbSet<LeadMaster> LeadMaster { get; set; }
        public DbSet<LeaveCount> LeaveCount { get; set; }
        public DbSet<RoleDetails> RoleDetails { get; set; }
        public DbSet<LeaveHistory> LeaveHistory { get; set; }
        public DbSet<Comments> Comments { get; set; }
        public DbSet<Credit> Credit { get; set; }
        //public DbSet<Creadit> Creadit { get; set; }
        public DbSet<Contact> Contact { get; set; }
        public DbSet<Hire> Hire { get; set; }
        public DbSet<EmployeeHistory> EmployeeHistory { get; set; }
        public DbSet<HolidayList> HolidayList { get; set; }

    }
}
