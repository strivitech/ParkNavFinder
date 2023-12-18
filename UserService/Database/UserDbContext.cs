using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Database;

internal class UserDbContext(DbContextOptions<UserDbContext> options) : IdentityDbContext<User>(options);