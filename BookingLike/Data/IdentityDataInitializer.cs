using Microsoft.AspNetCore.Identity;

namespace BookingLike.Data
{
    public class IdentityDataInitializer
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Moderator", "User" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        public static async Task SeedUsersAsync(UserManager<IdentityUser> userManager)
        {
            // MODERATOR
            var moderator = await userManager.FindByEmailAsync("moderator@booking.pl");
            if (moderator == null)
            {
                var newModerator = new IdentityUser
                {
                    UserName = "moderator@booking.pl",
                    Email = "moderator@booking.pl",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(newModerator, "Mod123!");
                await userManager.AddToRoleAsync(newModerator, "Moderator");
            }

            // UŻYTKOWNIK
            var user = await userManager.FindByEmailAsync("user@booking.pl");
            if (user == null)
            {
                var newUser = new IdentityUser
                {
                    UserName = "user@booking.pl",
                    Email = "user@booking.pl",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(newUser, "User123!");
                await userManager.AddToRoleAsync(newUser, "User");
            }
        }
    }
}
