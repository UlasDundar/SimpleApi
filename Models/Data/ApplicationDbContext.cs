using Microsoft.EntityFrameworkCore;
using SimpleApi.Models;

namespace SimpleApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<SmsEntity> SmsMessages => Set<SmsEntity>();
    public DbSet<User> Users => Set<User>();
}