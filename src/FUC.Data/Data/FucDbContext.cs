using Microsoft.EntityFrameworkCore;

namespace FUC.Data.Data;

public class FucDbContext : DbContext
{
    public FucDbContext(DbContextOptions options) : base(options)
    {
    }
}
