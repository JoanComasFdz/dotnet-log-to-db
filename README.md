# Log to DB
This is a simple .NET Core console application that reads a log file and writes the log entries to a PostgreSQL database.

The solution contains 3 projects which all implement the same functionality but with different approaches:
- `log-to-db-performance` - The first approach that focuses on performance over all.
- `log-to-db-performance-functional` - The second approach that is based on the performance approach, but attacks the problem in a more functional programming way.
- `log-to-db-ef` - The third approach that uses Entity Framework Core and keeps a functional approach.

The objective was to ingest as much data as possible, while keeping memory consumption stable (instead of growing with the number of log entries). This was achieved with a total memory consumption of 42MB to 52MB, regardles of the log file fize.

Study the README.md files in each project to understand the differences between the approaches.

## Performance comparison

Two files are used to compare the performance of the different approaches:
- file1.log - 38MB
- file2.log - 154MB (aprox 1.7 Million lines)

I tested the performance of the different approaches using the following hardware:
- Windows 11 Version Enterprise 10.0.22621 Build 22621
- HP ZBook Fury 15.6 inch G8 Mobile Workstation PC
- 11th Gen Intel(R) Core(TM) i9-11950H @ 2.60GHz, 2611 Mhz, 8 Core(s), 16 Logical Processor(s)
- 64.0 GB

The following table shows the time it takes to read the log file and write the log entries to the database:

| project                        | file     | avg time |
|--------------------------------|----------|----------|
| log-to-db-performance          | file1.log|  2.5s    |
| log-to-db-performance          | file2.log|  2.9s    |
| log-to-db-performance-functional| file1.log| 2.6s    |
| log-to-db-performance-functional| file2.log| 3.0s    |
| log-to-db-ef                   | file1.log|  4.8s    |
| log-to-db-ef                   | file2.log|  4.9s    |
| log-to-db-parsing              | file1.log|  4.1s    |
| log-to-db-parsing              | file2.log|  4.5s    |

Conclusions:
- Both full performance approach and the functional approach behave similarly in terms of performance.
- The EF approach is 100% slower than the other two, but it is still acceptable.

## Usage

Start a PostgreSQL database in a Docker container:

```bash
docker run --name logscanner-postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_USER=postgres -e POSTGRES_DB=logs -d -p 5432:5432 postgres
```

Run the application:

```bash
dotnet run "C:\path\to\log-folder"
```

> Make sure you are in the correct directory before running the command.

## Makig your own
In order to make this application work with your own log files, you need to modify the `LogLineValidator` and `LogLineParser` to accomodate your log file format and needs.

Sometimes, 1 log will contain new line characters and therefore be stored as multiple lines in the log file. In this case, you need to implement a mechanism to join those lines together before parsing them. I would recommend to use a new class `MultiLineJoiner` that will be responsible for joining the lines together (without new line characters), before being validated and parsed.

## Motivation
Log files are still a tool that is used to troubleshoot issues. It is the daily life of plenty of developers to look at log files to understand what is happening in the system. Those files can be up to multiple gigabytes in size and contain millions of log entries.

Reading them manually is impossible.
Reading them line by line is slow and can be inconsistent: lines may be interleaved due to multiple threads writing to the log file.
Correlation between log entries is difficult.

This application is a simple example of how to read a log file and write the log entries to a database.

Once in the DB, it is possible to query the log entries, filter them, and correlate them using a query language, wihch is more natural and easy to understand than custom algorithms looking for stuff up and down.

6 years ago it was my first attempt at a log file to database ingestion application and although it worked, it was taking minutes to process ~2GB log files and it would take lots of memory. I wanted to revisit this problem and see if I could do better. I am very happy with the results, I have learned a lot in the process and I am happy to share it with everybody.

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

## Future work
This solution just ingest logs for future analyisis using SQL queries.

Some the inmediate thing to do would be to develop a tool to query the logs with SQL.

The drawback is that those queries can prove to be very complex and time consuming to write.

Nowadays log aggregation is a topic on itself, and one of the most used stacks is [Grafana Lok](https://github.com/grafana/loki). This is whay I will explore next, but to anlyze existing log files, not to connect live apps to it.