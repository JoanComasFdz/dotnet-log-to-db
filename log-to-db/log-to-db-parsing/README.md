# Performance Functional
This project aims to be very performant while keeping a functional approach.

The following techniques are used:
- Uses the low level driver Npgsql to connect to the database.
- Use static methods to avoid object creation.
- The connection to the database is opened once and kept open for the entire life cyle.
- The log entries are written to the database in batches.
- Max reuse of resources.
- `ReadOnlySpan<T>` is used to minimize impact in memory.
- [Impureheim sandwitch](https://blog.ploeh.dk/2020/03/02/impureim-sandwich/)
- Extension methods for fluent API akin to Bind and Map in functional programming, and following a LINQ style.

## How to read
Just study the [Program.cs](./Program.cs) file, in particular this code:

```csharp
logFileStreamReader.StreamLines() // Impure
    .WhereLineIsRelevant()        // Pure
    .ToLogEntry()                 // Pure
    .InBatchesOf(1000)            // Pure
    .ToSQLCommands(logFileName)   // Pure
    .InsertIntoDatabase(conn);    // Impure
```

## Parsing
This approach parses the message trying to extract the known patterns that will be necessary later on to analye the logs.

Parsing and inserting 5 fields makes the code a bit slower, going up to 4.3 to 4.7 seconds
and consumes up to 74MB of memory. The memory usage is also volatile, going up and down, since each line may have some info parsed
and some other may be null.

This approach is 50% slower and consumes 50% more memory than the functional approach without parsing but the numbers are still
really solid. And who is going to complain about 4.7 seconds to parse and insert 1.000.000 lines? Specially when now the SELECT
queries are going to be much faster. Let's see how much though.

