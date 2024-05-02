using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Threading;

namespace log_to_db_ef;

internal class LogDbContext : DbContext
{
    public DbSet<LogEntry> LogEntries { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=127.0.0.1; Username=postgres; Password=postgres; Database=logscannerdotnet");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LogEntry>()
            .Property(e => e.timestamp)
            .HasColumnType("timestamp without time zone"); // This is to store the time as is, without a timezone.
    }

    public int DeleteLogEntries(string logFileName)
    {
        return Database.ExecuteSqlRaw($"DELETE FROM logef WHERE file_name = '{logFileName}'");
    }
}

[Table("logef")]
public record LogEntry(string file_name, DateTime timestamp, string thread_id, string log_level, string component, string message)
{
    [Key]
    public int id { get; set; }

    public static LogEntry Empty => new(
        string.Empty,
        DateTime.MinValue,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty);
};
