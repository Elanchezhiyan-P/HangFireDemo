# Hangfire Job Scheduling and Email Sending

This project demonstrates how to use [Hangfire](https://www.hangfire.io/) for job scheduling and sending emails in a .NET application. It utilizes Hangfire's in-memory storage for simplicity and includes a sample page to schedule email jobs.

## Table of Contents

- [Overview](#overview)
- [Prerequisites](#prerequisites)
- [Packages](#packages)
- [Setup](#setup)
  - [Install Packages](#install-packages)
  - [Configure Hangfire](#configure-hangfire)
  - [Basic Authentication for Dashboard](#basic-authentication-for-dashboard)
- [Scheduling Jobs](#scheduling-jobs)
- [Email Sending Page](#email-sending-page)
- [Running the Application](#running-the-application)

## Overview

This project demonstrates how to integrate Hangfire into a .NET application to schedule and process background jobs, with a focus on scheduling email sending tasks.

## Prerequisites

- .NET 8.0 or later
- Visual Studio or another .NET development environment

## Packages

Ensure you have the following NuGet packages installed:

- [Hangfire](https://www.nuget.org/packages/Hangfire) (Version 1.8.14)
- [Hangfire.Dashboard.Basic.Authentication](https://www.nuget.org/packages/Hangfire.Dashboard.Basic.Authentication) (Version 7.0.1)
- [Hangfire.InMemory](https://www.nuget.org/packages/Hangfire.InMemory) (Version 0.10.3)

## Setup

### Install Packages

Add the above NuGet packages to your project using the NuGet Package Manager in Visual Studio or by editing your `.csproj` file.

### Configure Hangfire

Configure Hangfire in your `Program.cs` file. Here is an example configuration for `Program.cs`:

```csharp
 builder.Services.AddHangfire(configuration => configuration
 .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseDefaultTypeSerializer()
        .UseInMemoryStorage());

 //InMemory storage can be used if you are not willing to use the exisitng sql server database

 builder.Services.AddHangfireServer();
 builder.Services.AddTransient<EmailService>();

   app.UseHangfireDashboard("/JobDashboard", new DashboardOptions
  {
      Authorization = new[] { new HangfireAuthorizationFilter() },
      DashboardTitle = "Job Dashboard",
      IsReadOnlyFunc = (DashboardContext context) =>
      {
          /* //This can be enabled when users are added and based on role, it can be made as readonly or not
          var user = context.GetHttpContext().User;
          return !user?.Identity?.IsAuthenticated ?? true; */
          return false;
      },
      DisplayStorageConnectionString = false,
      DefaultRecordsPerPage = 10,
      IgnoreAntiforgeryToken = true
  });
  JobStorage.Current.GetConnection().RemoveTimedOutServers(new TimeSpan(0, 0, 15));

  //RemoveDuplicateServers(); //This can be enabled if the servers are duplicated.

```

The above function can be enabled if multiple servers are shown.

```CSharp
    private static void RemoveDuplicateServers()
    {
        Hangfire.Storage.IMonitoringApi monitoringApi = JobStorage.Current.GetMonitoringApi();
        var allServers = monitoringApi.Servers();
        var serverGroups = allServers.GroupBy(server => server.Name)
            .Where(group => group.Count() > 1);

        foreach (var group in serverGroups)
        {
            var serversToRemove = group.Skip(1);

            foreach (var server in serversToRemove)
            {
                JobStorage.Current.GetConnection().RemoveServer(server.Name);
            }
        }
    }
```

### Basic Authentication for Dashboard

```CSharp
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            /* //This restricts user and can be redirected to unauthorized page.
            var httpContext = context.GetHttpContext();
            return httpContext.User.Identity.IsAuthenticated && httpContext.User.IsInRole("Admin"); */
            return true;
        }
    }
```

### Scheduling Jobs

```CSharp

  /* Schedule Job to run at 2:00 AM - Starts */
  var cronExpression = "0 2 * * *";
  RecurringJob.RemoveIfExists("JobAtTime2");
  RecurringJob.RemoveIfExists("JobEvery2Minutes");

  RecurringJob.AddOrUpdate("JobAtTime2", () => Console.WriteLine("Task ran successfully - " + DateTime.Now), cronExpression);

  RecurringJob.AddOrUpdate("JobEvery2Minutes", () => Console.WriteLine("Task runs every 2 mins - " + DateTime.Now), Cron.MinuteInterval(2));

  /* Schedule Job to run at 2:00 AM - Ends */

```

Queued:
![ScheduledJob](images/ScheduledSS.jpg)

### Email Sending Page

Validation:
 ![Validation](images/ErrorSS.jpg)

Success:
 ![Success](images/SuccessSS.jpg)

Queued:
 ![Queued](images/QueuedSS.jpg)

### Running the Application

To build and run your application, use the following commands:

1. **Clean the project:**

   ```bash
   dotnet clean

2. **Build the project:**

   ```bash
   dotnet build

3. **Run the application:**

   ```bash
   dotnet run

These commands will clean, build, and run your .NET application. After running these commands, navigate to `/JobDashboard` to access the Hangfire Dashboard and use the email scheduling page to manage scheduled jobs.