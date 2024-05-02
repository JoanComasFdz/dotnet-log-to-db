# Using Entity Framework
This project reads a log file and stores each row in a database. It uses **Entity Framework** to interact with the database.

The code has been coded in a way more akin to Functional Programming.

Now you can run the app with a directory containing .log files as an argument:
```
dotnet run -- /path/to/log/files
```

> Make sure you are in the correct directory before running the command.

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