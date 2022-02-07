using Microsoft.EntityFrameworkCore;
using Models;
using SDTS.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDTS.DataAccess
{
    public class SDTSContext : DbContext
    {
        public SDTSContext(DbContextOptions<SDTSContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<ConnectedUser> ConnectedUsers { get; set; }
        public DbSet<GuardianAndWard> GuardiansAndWards { get; set; }
        public DbSet<Invitation> Invitations { get; set; }
        public DbSet<SecureArea> SecureAreas { get; set; }
        public DbSet<EmergencyHelper> EmergencyHelpers { get; set; }
        //public DbSet<RescureGroup> RescureGroups { get; set; }
        //public DbSet<IsPublished> IsPublisheds { get; set; }
        public DbSet<SensorsData> UserData { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<UserModel>().ToTable("UserModel");
            //base.OnModelCreating(modelBuilder);

            modelBuilder.Ignore<List<string>>();

            modelBuilder.Ignore<List<int>>();
        }



    }
}
