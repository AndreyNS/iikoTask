using iikoTask.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace iikoTask.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public virtual DbSet<User> Users { get; set; }
    }
}
