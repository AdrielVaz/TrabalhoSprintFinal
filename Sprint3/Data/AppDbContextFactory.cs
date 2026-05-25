using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Sprint3.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var contentRoot = Directory.GetCurrentDirectory();
            var projectRoot = Directory.Exists(Path.Combine(contentRoot, "Sprint3"))
                ? Path.Combine(contentRoot, "Sprint3")
                : contentRoot;

            var configuration = new ConfigurationBuilder()
                .SetBasePath(projectRoot)
                .AddJsonFile("appsettings.json", optional: true)
                .AddInMemoryCollection(ReadDotEnv(Path.Combine(projectRoot, ".env")))
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            return new AppDbContext(optionsBuilder.Options);
        }

        private static Dictionary<string, string?> ReadDotEnv(string path)
        {
            var values = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            if (!File.Exists(path))
            {
                return values;
            }

            foreach (var rawLine in File.ReadAllLines(path))
            {
                var line = rawLine.Trim();
                if (line.Length == 0 || line.StartsWith("#"))
                {
                    continue;
                }

                var separatorIndex = line.IndexOf('=');
                if (separatorIndex <= 0)
                {
                    continue;
                }

                var key = line[..separatorIndex].Trim().Replace("__", ":");
                var value = line[(separatorIndex + 1)..].Trim();
                values[key] = value;
            }

            return values;
        }
    }
}
