# Performance
This project aims to be the most performant way to read a log file and write the log entries to a database.

The following techniques are used:
- Uses the low level driver Npgsql to connect to the database.
- Use static methods to avoid object creation.
- The connection to the database is opened once and kept open for the entire life cyle.
- The log entries are written to the database in batches.
- Max reuse of resources.
- Wierd things like `ref struct` and `ReadOnlySpan<T>` are used to minimize impact in memory.

## How to read
Just study the [Program.cs](./Program.cs) file.