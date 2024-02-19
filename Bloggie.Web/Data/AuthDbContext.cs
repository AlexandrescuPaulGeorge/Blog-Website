using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Data
{
    public class AuthDbContext : IdentityDbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var adminRoleId = "74053433-2fa0-4ab6-b2db-e6d49786ef5f";
            var superAdminRoleId = "c8e839e0-547b-4205-ac88-0e0be9e4bf3f";
            var userRoleId = "84a47d20-aa75-4763-b839-691f0ff5ee4f";

            //Seed Roles(user,admin,superadmin)
            var roles = new List<IdentityRole> {
                new IdentityRole
                {
                    Name = "Admin",
                    NormalizedName = "Admin",
                    Id = adminRoleId,
                    ConcurrencyStamp = adminRoleId
                },
                new IdentityRole
                {
                    Name="SuperAdmin",
                    NormalizedName = "SuperAdmin",
                    Id = superAdminRoleId,
                    ConcurrencyStamp = superAdminRoleId
                },
                new IdentityRole
                {
                    Name="User",
                    NormalizedName="User",
                    Id= userRoleId,
                    ConcurrencyStamp = userRoleId
                }
            };
            

            builder.Entity<IdentityRole>().HasData(roles);

            var superAdminId = "9d337ca7-db60-4c11-bf93-dc479619729d";
            //Seed SuperAdminUser
            var superAdminUser = new IdentityUser
            {
                UserName = "superadmin@bloggie.com",
                Email = "superadmin@bloggie.com",
                NormalizedEmail = "superadmin@bloggie.com".ToUpper(),
                NormalizedUserName = "superadmin@bloggie.com".ToUpper(),
                Id = superAdminId
            };

            superAdminUser.PasswordHash = new PasswordHasher<IdentityUser>()
                .HashPassword(superAdminUser,"Superadmin@123");
            builder.Entity<IdentityUser>().HasData(superAdminUser);

            //Add All roles to SUperAdminUser

            var superAdminRoles = new List<IdentityUserRole<string>>
            {
                new IdentityUserRole<string>
                {
                    RoleId = adminRoleId,
                    UserId = superAdminId
                },
                new IdentityUserRole<string>
                {
                    RoleId = superAdminRoleId,
                    UserId = superAdminId
                },
                new IdentityUserRole<string>
                {
                    RoleId = userRoleId,
                    UserId = superAdminId
                }
            };

            builder.Entity<IdentityUserRole<string>>().HasData(superAdminRoles);

        }
    }
}
