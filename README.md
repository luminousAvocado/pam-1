# Probation Access Manager (PAM)

Probation Access Manager (PAM) is a web application that manages the system registration process at the Probation
Department of Los Angeles County.

PAM is developed by Jaime Borunda, Kevork Gilabouchian, James Kang, and Brandon Lam as their
[senior design project](https://csns.calstatela.edu/department/cs/project/view?id=6636013) at CSULA.

## Set Up Development Environment

ASP.NET Core 2.2 and Microsoft SQL Server are required. As for an IDE, either Visual Studio 2017 or Visual Studio
Code is fine, though Visual Studio 2017 is preferred as it has better support for coding rules in `.editorconfig`.

*Currently the version of .NET Core bundled in Visual Studio 2017 is 2.1. You need to download and install
[.NET Core 2.2 SDK](https://dotnet.microsoft.com/download/dotnet-core/2.2).*

A SMTP server is also needed to send out email notifications. It is possible to use a remote SMTP server like GMail,
though it would be eaiser, especially for testing during development, if you set up a local email server like
[hMailServer](https://csns.calstatela.edu/wiki/content/cysun/course_materials/hmailserver).

1. Clone the Git repository.
2. Copy `appsettings.sample.json` to `appsettings.json`, and edit `appsettings.json` to match your environment.
   Also copy `appsettings.json` to the PAM.Test project if you want to run the unit tests.
3. Run the following commands inside the project folder to create the database:
```
    $ dotnet ef migrations add InitialSchema
    $ dotnet ef database update
```
If you have an old database, you should drop it first either manually or using the following command:
```
    $ dotnet ef database drop
```
4. Run the SQL script `Scripts/InsertData.sql` to populate the database. There are various ways to run an SQL
script, and the easiest is to use the command line tool `sqlcmd`:
```
    $ sqlcmd -S <host> -d <database> -U <user> -i <script>
```
