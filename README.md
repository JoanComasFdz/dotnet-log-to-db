# Log to DB
This is a simple .NET Core console application that reads a log file and writes the log entries to a PostgreSQL database.

It has been implemented with a focus on performance. Some of the key features are:
- The connection to the database is opened once and closed once.
- The log entries are written to the database in batches.
- The log lines are minimally parsed.
- `ref struct` is used to avoid heap allocations.
- `ReadOnlySpan<T>` is used to avoid copying data.

The code tries to follow a more functional approach.

## Motivation
Log files are still a tool that is used to troubleshoot issues. It is the daily life of plenty of developers to look at log files to understand what is happening in the system. Those files can be up to multiple gigabytes in size and contain millions of log entries.

Reading them manually is impossible.
Reading them line by line is slow and can be inconsistent: lines may be interleaved due to multiple threads writing to the log file.
Correlation between log entries is difficult.

This application is a simple example of how to read a log file and write the log entries to a database.

Once in the DB, it is possible to query the log entries, filter them, and correlate them using a query language, wihch is more natural and easy to understand than custom algorithms looking for stuff up and down.

## Usage

Start a PostgreSQL database in a Docker container:

```bash
docker run --name logscanner-postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_USER=postgres -e POSTGRES_DB=logs -d -p 5432:5432 postgres
```

Run the application:

```bash
cd log-to-db
dotnet run "C:\path\to\log-folder"
```

## Useful PostgreSQL commands

List databases:

```bash
docker exec logscanner-postgres psql -h 127.0.0.1 -U postgres -c "\l"

docker exec logscanner-postgres psql -h 127.0.0.1 -U postgres -c "SELECT datname FROM pg_database WHERE datistemplate = false;"
```

List tables in database:

```bash	
docker exec logscanner-postgres psql -h 127.0.0.1 -U postgres -d logscannerdotnet -c "SELECT schemaname, tablename FROM pg_tables WHERE schemaname NOT IN ('pg_catalog', 'information_schema');"
```

Terminate connections to a database:
```bash
docker exec logscanner-postgres psql -h 127.0.0.1 -U postgres -c "SELECT pg_terminate_backend(pg_stat_activity.pid) FROM pg_stat_activity WHERE datname = 'logscannerdotnet';"
```


Drop the database:
```bash
docker exec logscanner-postgres psql -h 127.0.0.1 -U postgres -c "DROP DATABASE logscannerdotnet;"
```

Clear the log table:
```bash
docker exec logscanner-postgres psql -h 127.0.0.1 -U postgres -d logscannerdotnet -c "DELETE FROM log;"
```