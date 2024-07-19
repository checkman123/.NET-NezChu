using NezChu.Client.Pages;
using NezChu.Components;
using NpgsqlTypes;
using Serilog.Sinks.PostgreSQL.ColumnWriters;
using Serilog.Sinks.PostgreSQL;
using Serilog;
using System.Net.Sockets;
using System.Net;
using Serilog.Events;
using Serilog.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

#region Logging
// Configure serilog logging
//Connection string is from Secret Manager. (Right-click on project and select "Manage User Secrets")
var logConnectionString = builder.Configuration["SupabaseConnectionString"];

if (!string.IsNullOrEmpty(logConnectionString))
{
    IDictionary<string, ColumnWriterBase> columnOptions = new Dictionary<string, ColumnWriterBase>
        {
            { "message", new RenderedMessageColumnWriter(NpgsqlDbType.Text) },
            { "message_template", new MessageTemplateColumnWriter(NpgsqlDbType.Text) },
            { "level", new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
            { "raise_date", new TimestampColumnWriter(NpgsqlDbType.TimestampTz) },
            { "exception", new ExceptionColumnWriter(NpgsqlDbType.Text) },
            { "properties", new LogEventSerializedColumnWriter(NpgsqlDbType.Jsonb) },
            { "props_test", new PropertiesColumnWriter(NpgsqlDbType.Jsonb) },
            { "machine_name", new SinglePropertyColumnWriter("MachineName", PropertyWriteMethod.ToString, NpgsqlDbType.Text, "l") }
        };

    var loggerConfiguration = new LoggerConfiguration().Filter.ByExcluding(le => Matching.FromSource("Microsoft").Invoke(le)
                         && (le.Level == LogEventLevel.Verbose
                         || le.Level == LogEventLevel.Debug
                         || le.Level == LogEventLevel.Information))
                    .WriteTo.PostgreSQL(
                                connectionString: logConnectionString,
                                columnOptions: columnOptions,
                                needAutoCreateTable: true,
                                tableName: "Serilog"
                                ).WriteTo.Console();

    var logger = loggerConfiguration.CreateLogger();

    builder.Services.AddLogging(loggingBuilder =>
    {
        loggingBuilder.AddSerilog(logger);
    });

    logger.Information("Working Serilog");
    var host = Dns.GetHostEntry(Dns.GetHostName());
    foreach (var ip in host.AddressList)
    {
        if (ip.AddressFamily == AddressFamily.InterNetwork)
        {
            logger.Information("Application starting. IP Address: {IpAddress}", ip.ToString());
            break;
        }
    }
}
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(NezChu.Client._Imports).Assembly);

app.Run();
