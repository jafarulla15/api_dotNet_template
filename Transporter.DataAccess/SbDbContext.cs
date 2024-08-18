using Microsoft.EntityFrameworkCore;
using Transporter.Common.Models;
using Transporter.Common.VM;

namespace Transporter.DataAccess
{

#pragma warning disable CS8618

    //public class ScalarResult
    //{
    //    public int Result { get; set; }
    //}
    public class SbDbContext : DbContext
    {
        public SbDbContext()
        {

        }
        public SbDbContext(DbContextOptions<SbDbContext> options) : base(options)
        {

        }

      //  public DbSet<ScalarResult> SomeScalarQueryResults { get; set; }

        //*** Square Village
        public virtual DbSet<VMSystemUser> VMSystemUser { get; set; }
        public virtual DbSet<RolePermissionMapping> RolePermissionMapping { get; set; }
        public virtual DbSet<Page> Page { get; set; }
        public virtual DbSet<Roles> Role { get; set; }
        public virtual DbSet<SystemUser> SystemUser { get; set; }
        public virtual DbSet<AccessToken> AccessTokens { get; set; }
        public virtual DbSet<Actions> Actions { get; set; }
        public virtual DbSet<RoleActionMapping> RoleActionMapping { get; set; }
        public virtual DbSet<UserSession> UserSession { get; set; }
        public virtual DbSet<VMChangeUserRole> VMChangeUserRole { get; set; }
        public virtual DbSet<VMUserAndDepartmentMapping> VMUserAndDepartmentMapping { get; set; }

        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<CompanyEmployeeMapping> CompanyEmployeeMapping { get; set; }

        public virtual DbSet<EmployeeInformation> EmployeeInformation { get; set; }
        public virtual DbSet<AdminPaymentInformation> AdminPaymentInformation { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
            
            modelBuilder.Entity<RolePermissionMapping>(entity =>
            {
                entity.HasKey(x => x.RolePermissionMappingID);
                entity.ToTable("RolePermissionMapping");
            });
            modelBuilder.Entity<Page>(entity =>
            {
                entity.HasKey(x => x.PageID);
                entity.ToTable("Page");
            });
            modelBuilder.Entity<Roles>(entity =>
            {
                entity.HasKey(x => x.RoleID);
                entity.ToTable("Role");
            });
            modelBuilder.Entity<SystemUser>(entity =>
            {
                entity.HasKey(x => x.SystemUserID);
                entity.ToTable("SystemUser");
            });

            //*** Old
            modelBuilder.Entity<AccessToken>(entity =>
            {
                entity.HasKey(x => x.AccessTokenID);
                entity.ToTable("AccessTokens");
            });
            modelBuilder.Entity<Actions>(entity =>
            {
                entity.HasKey(x => x.ActionID);
                entity.ToTable("Actions");
            });

            modelBuilder.Entity<Page>(entity =>
            {
                entity.HasKey(x => x.PageID);
                entity.ToTable("Page");
            });
            modelBuilder.Entity<RoleActionMapping>(entity =>
            {
                entity.HasKey(x => x.RoleActionMappingID);
                entity.ToTable("RoleActionMapping");
            });
            modelBuilder.Entity<RolePermissionMapping>(entity =>
            {
                entity.HasKey(x => x.RolePermissionMappingID);
                entity.ToTable("RolePermissionMapping");
            });
            modelBuilder.Entity<VMSystemUser>(entity =>
            {
                entity.HasKey(x => x.SystemUserID);
                entity.ToTable("VMSystemUser");
            });
            modelBuilder.Entity<UserSession>(entity =>
            {
                entity.HasKey(x => x.UserSessionID);
                entity.ToTable("UserSession");
            });
            modelBuilder.Entity<VMChangeUserRole>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("ChangeRole");
            });
            modelBuilder.Entity<VMUserAndDepartmentMapping>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("GetSystemUsersByDepartmentID");
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(x => x.CompanyId);
                entity.ToTable("Company");
            });
            modelBuilder.Entity<CompanyEmployeeMapping>(entity =>
            {
                entity.HasKey(x => x.CompanyEmployeeMappingID);
                entity.ToTable("CompanyEmployeeMapping");
            });
        }
    }

#pragma warning restore CS8618
}