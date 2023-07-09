using CRUDProject.Middlewares;
using CRUDProject.StartUpExtensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddServices(builder.Configuration);

builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(context.Configuration).ReadFrom.Services(services));

var app = builder.Build();


if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{ 
    app.UseExceptionHandler("/home/error");
    app.UseExceptionHandlingMiddleware();
}

app.UseHsts();
app.UseHttpsRedirection();
app.UseHttpLogging();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }