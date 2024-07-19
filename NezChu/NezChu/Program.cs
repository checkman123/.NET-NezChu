using NezChu.Client.Pages;
using NezChu.Components;
using NpgsqlTypes;
using Serilog.Sinks.PostgreSQL.ColumnWriters;
using Serilog.Sinks.PostgreSQL;
using Serilog;
using Serilog.Events;
using Serilog.Filters;
using NezChu.Database;
using Microsoft.EntityFrameworkCore;
using Carter;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

#region My Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCarter();

builder.Services.AddHttpClient();

builder.Services.AddWebOptimizer(pipeline =>
{
    pipeline.AddCssBundle("/css/site.css", "scss/site.scss");
    pipeline.MinifyJsFiles("js/**/*.js", "js/**/*.js");
});
#endregion

#region DbContext and Logging
//Connection string is from Secret Manager. (Right-click on project and select "Manage User Secrets")
var logConnectionString = builder.Configuration["SupabaseConnectionString"];

if (!string.IsNullOrEmpty(logConnectionString))
{

    //Database
    //https://www.youtube.com/watch?v=CalH0TJrhp8 -> should use dbcontextfactory to dispose dbcontext right away. 
    //It is recommended in the AddDbContextFactory description
    builder.Services.AddDbContextFactory<NezChuDbContext>(option =>
    {
        option.UseNpgsql(logConnectionString, builder =>
         {
             builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
         });
    });

    // Configure serilog logging
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

    logger.Information($"!! Working Serilog !! log str: {logConnectionString?.Substring(1,5)}");
}
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
#region Pipelines
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseDeveloperExceptionPage();
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/StatusCode/{0}");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
#endregion

app.MapCarter(); //Map Minimal Api

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(NezChu.Client._Imports).Assembly);

app.Run();
