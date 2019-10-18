using FWLog.Data.Logging;
using FWLog.Data.Models;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace FWLog.Data
{
    public class Entities : DbContext
    {
        public Entities()
            : base("name=Entities")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("DART");
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();           
        }

        public virtual DbSet<Application> Application { get; set; }
        public virtual DbSet<ApplicationLanguage> ApplicationLanguage { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<BOLogSystem> BOLogSystem { get; set; }
        public virtual DbSet<ApplicationSession> ApplicationSession { get; set; }
        public virtual DbSet<ApplicationLog> ApplicationLog { get; set; }
        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<UserCompany> UserCompany { get; set; }

        public IAuditLog AuditLog { get; private set; }

        public Entities(IAuditLog auditLog)
        {
            AuditLog = auditLog;
        }

        public override int SaveChanges()
        {
            int nonLogChanges;
            AuditLog.AddLogsToContextAndSaveChanges(this, out nonLogChanges);

            return nonLogChanges;
        }

        public int SaveChangesWithoutLog()
        {
            return base.SaveChanges();
        }

    }
}