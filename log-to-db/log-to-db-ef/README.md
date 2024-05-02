# Entity Framework
It uses [Entity Framework](https://en.wikipedia.org/wiki/Entity_Framework) to interact with the database.

The following techniques are used:
- Use static methods to avoid object creation.
- The DBContext is instantiatet once and used and kept open for the entire life cyle.
- The log entries are written to the database in batches.
- Max reuse of resources.
- `ReadOnlySpan<T>` is used to minimize impact in memory.
- [Impureheim sandwitch](https://blog.ploeh.dk/2020/03/02/impureim-sandwich/)
- Extension methods for fluent API akin to Bind and Map in functional programming, and following a LINQ style.

Interestingly, I could keep the functional approach even when using Entity Framework and it did not affect performance. It also looks 100% like the *performance-functional* approach.

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

## Updating the DB schema
If you modify the LogEntry record, you have two options:

### 1. Keep the existing data
Run the following command to update the database schema:

```
dotnet ef migrations add <migration-name>
```

Then run the application.

### 2. Disregard the existing data
Manually delete the table with the following command:
````
DROP TABLE logef;
````

Delete the "Migrations" folder of this project.

Run the following command to create a new migration:

```
dotnet ef migrations add InitialCreate
```

Then run the application.