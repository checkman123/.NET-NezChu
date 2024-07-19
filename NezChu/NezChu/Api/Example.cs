using Carter;
using NezChu.Database;
using NezChu.Database.Entities;

namespace NezChu.Api
{
    public class ExampleModule : CarterModule
    {
        private readonly ILogger<ExampleModule> _logger;
        public ExampleModule(ILogger<ExampleModule> logger) : base("/api/example")
        {
            base.WithTags("Examples");
            this._logger = logger;
        }
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            //Get Request
            app.MapGet("/", () =>
            {
                return Results.Ok("Ok");
            });

            app.MapPost("/test", async (NezChuDbContext nezChuDbContext) =>
            {
                IpLog newLog = new IpLog()
                {
                    Ipaddress = "test",
                    Date = DateTime.Now.ToUniversalTime(),
                };

                await nezChuDbContext.AddAsync(newLog);
                await nezChuDbContext.SaveChangesAsync();

                return Results.Ok();
            });
        }
    }
}
