# Probation Access Manager (PAM)

Probation Access Manager (PAM) is a web application that manages the system registration process at the Probation
Department of Los Angeles County.

PAM is developed by Jaime Borunda, Kevork Gilabouchian, James Kang, Brandon Lam, and Yi Wang as their
[senior design project](https://csns.calstatela.edu/department/cs/project/view?id=6636013) at CSULA.

## Set Up Development Environment

ASP.NET Core 2.2 and Microsoft SQL Server are required. As for an IDE, either Visual Studio 2017 or Visual Studio
Code is fine, though Visual Studio 2017 is preferred as it has better support for coding rules in `.editorconfig`.

*If you installed Visual Studio 2017 before ASP.NET 2.2 was released (12/4/2018), you need to download and install
the [.NET Core 2.2 SDK](https://dotnet.microsoft.com/download/dotnet-core/2.2).*

1. Clone the Git repository.
2. Copy `appsettings.sample.json` to `appsettings.json`, and edit `appsettings.json` to match your environment.
   Also copy `appsettings.json` to the PAM.Test project if you want to run the unit tests.
3. Run the following commands inside the project folder to create the database:
```
    $ dotnet ef migrations add InitialSchema
    $ dotnet ef database update
```
If you have an old database, you should drop it first using the following command:
```
    $ dotnet ef database drop
```
4. Run the SQL script `Scripts/InsertData.sql` to populate the database. There are various ways to run an SQL
script, and the easiest is to use the command line tool `sqlcmd`:
```
    $ sqlcmd -S <host> -d <database> -U <user> -i <script>
```
