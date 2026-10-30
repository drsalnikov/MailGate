using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MailGate.Infrastructure.Persistence;

public sealed class MailGateDbContextFactory : IDesignTimeDbContextFactory<MailGateDbContext>
{
    public MailGateDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<MailGateDbContext>()
            .UseNpgsql(Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                ?? "Host=localhost;Database=mailgate;Username=mailgate;Password=mailgate")
            .Options;

        return new MailGateDbContext(options);
    }
}
