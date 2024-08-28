using Hangfire;
using Hangfire.Dashboard;
using HangFireSample.Helpers.Filters;
using HangFireSample.Service;

namespace HangFireSample;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
               .UseSimpleAssemblyNameTypeSerializer()
               .UseDefaultTypeSerializer()
               .UseInMemoryStorage());

        //InMemory storage can be used if you are not willing to use the exisitng sql server database

        builder.Services.AddHangfireServer();
        builder.Services.AddTransient<EmailService>();

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

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

        /* Schedule Job to run at 2:00 AM - Starts */
        var cronExpression = "0 2 * * *";
        RecurringJob.RemoveIfExists("JobAtTime2");
        RecurringJob.RemoveIfExists("JobEvery2Minutes");

        RecurringJob.AddOrUpdate("JobAtTime2", () => Console.WriteLine("Task ran successfully - " + DateTime.Now), cronExpression);

        RecurringJob.AddOrUpdate("JobEvery2Minutes", () => Console.WriteLine("Task runs every 2 mins - " + DateTime.Now), Cron.MinuteInterval(2));

        /* Schedule Job to run at 2:00 AM - Ends */

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }

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
}
