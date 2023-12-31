using SudokuBackend.Extensions;
using Contracts;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using NLog;
using Service.Contracts;
using SudokuBackend.Presentation.ActionFilters;
using AspNetCoreRateLimit;


NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter() {
    return new ServiceCollection().AddLogging().AddMvc().AddNewtonsoftJson()
        .Services.BuildServiceProvider()
        .GetRequiredService<IOptions<MvcOptions>>().Value.InputFormatters
        .OfType<NewtonsoftJsonPatchInputFormatter>().First();
}


var builder = WebApplication.CreateBuilder(args);
LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.ConfigureCors();
builder.Services.ConfigureIISIntegration();
builder.Services.ConfigureLoggerService();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.ConfigureVersioning();
builder.Services.AddAuthentication();
builder.Services.ConfigureIdentity();
builder.Services.ConfigureJWT(builder.Configuration);
builder.Services.AddMemoryCache();
builder.Services.ConfigureRateLimitingOptions();
builder.Services.AddHttpContextAccessor();
//Microsoft.AspNetCore.ResponseCaching
builder.Services.ConfigureResponseCaching();
//Marvin.Cache.Headers
builder.Services.ConfigureHttpCacheHeaders();

builder.Services.Configure<ApiBehaviorOptions>(options => {
    options.SuppressModelStateInvalidFilter = true; 
});

builder.Services.AddScoped<ValidationFilterAttribute>();

builder.Services.AddControllers(config => {
    config.RespectBrowserAcceptHeader = true;
    config.ReturnHttpNotAcceptable = true;
    config.InputFormatters.Insert(0, GetJsonPatchInputFormatter());    
})
    .AddXmlDataContractSerializerFormatters()
    .AddApplicationPart(typeof(SudokuBackend.Presentation.AssemblyReference).Assembly);

builder.Services.ConfigureSwagger(); 

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILoggerManager>();
app.ConfigureExceptionHandler(logger);

if (app.Environment.IsProduction()) {
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseForwardedHeaders(new ForwardedHeadersOptions {
    ForwardedHeaders = ForwardedHeaders.All
});
app.UseIpRateLimiting();
app.UseCors("CorsPolicy");
//Microsoft.AspNetCore.ResponseCaching (UseCors most be called first https://learn.microsoft.com/en-us/aspnet/core/performance/caching/middleware?view=aspnetcore-7.0)
app.UseResponseCaching();
//Marvin.Cache.Headers
app.UseHttpCacheHeaders();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI(s => {
    s.SwaggerEndpoint("/swagger/v1/swagger.json", "Sudoku v1");
    //s.SwaggerEndpoint("/swagger/v2/swagger.json", "Sudoku v2");
});
app.Run();

