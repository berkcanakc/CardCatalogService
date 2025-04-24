using Hangfire.Dashboard;

namespace CardCatalogService.Infrastructure.Authorization
{
    public class AllowAllUsersFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return true; // Herkese açık olsun
        }
    }
}
