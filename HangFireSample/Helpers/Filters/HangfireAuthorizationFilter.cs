using Hangfire.Dashboard;

namespace HangFireSample.Helpers.Filters
{
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
}
