using Contracts;
using LoggerService;
using Repository;
using Service.Contracts;
using Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Marvin.Cache.Headers;
using AspNetCoreRateLimit;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Entities.ConfigurationModels;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;
using AutoMapper.Configuration.Annotations;

static class ServiceExtensions {

    public static void ConfigureCors(this IServiceCollection services) {
        services.AddCors(options => {
            options.AddPolicy("CorsPolicy", builder => {
                builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("X-Pagination"); 
            });
        });

    }


    public static void ConfigureIISIntegration(this IServiceCollection services) {
        services.Configure<IISOptions>(options => {

        });
    }


    public static void ConfigureLoggerService(this IServiceCollection services) {
        services.AddSingleton<ILoggerManager, LoggerManager>();
    }


    public static void ConfigureRepositoryManager(this IServiceCollection services) {
        services.AddScoped<IRepositoryManager, RepositoryManager>();
    }


    public static void ConfigureServiceManager(this IServiceCollection services) {
        services.AddScoped<IServiceManager, ServiceManager>();
    }


    public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration) {
        services.AddDbContext<RepositoryContext>(opts => {
            opts.UseSqlServer(configuration.GetConnectionString("sqlConnection"));
        });
    }


    public static void ConfigureVersioning(this IServiceCollection services) {
        services.AddApiVersioning(opt => {
            opt.ReportApiVersions = true;
            opt.AssumeDefaultVersionWhenUnspecified = true;
            opt.DefaultApiVersion = new ApiVersion(1, 0);
        });
    }

    //Microsoft.AspNetCore.ResponseCaching
    public static void ConfigureResponseCaching(this IServiceCollection services) {
        services.AddResponseCaching(opt => {
            opt.MaximumBodySize = 1024*5;
            opt.UseCaseSensitivePaths = false;
        });
    }

 
    //Marvin.Cache.Headers (https://github.com/KevinDockx/HttpCacheHeaders)
    public static void ConfigureHttpCacheHeaders(this IServiceCollection services) {
        services.AddHttpCacheHeaders(
            (expirationOpt) => {
                expirationOpt.MaxAge = 60;
                expirationOpt.CacheLocation = CacheLocation.Public;
            },
            (validationOpt) => {
                validationOpt.MustRevalidate = true;
            }
        );
    }


    public static void ConfigureRateLimitingOptions(this IServiceCollection services) {
        var rateLimitRules = new List<RateLimitRule> {
            new RateLimitRule {
                Endpoint = "*", 
                Limit = 30, 
                Period = "5m" } 
        }; 
        
        services.Configure<IpRateLimitOptions>(opt => { opt.GeneralRules = rateLimitRules; }); 
        services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>(); 
        services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>(); 
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>(); 
        services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>(); 
    }


    public static void ConfigureIdentity(this IServiceCollection services) { 
        var builder = services.AddIdentity<User, IdentityRole>(o => { 
            o.Password.RequireDigit = true; 
            o.Password.RequireLowercase = false; 
            o.Password.RequireUppercase = false; 
            o.Password.RequireNonAlphanumeric = false; 
            o.Password.RequiredLength = 8; 
            o.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<RepositoryContext>()
        .AddDefaultTokenProviders(); 
    }


    public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration) {
        var jwtConfiguration = new JwtConfiguration();
        configuration.Bind(jwtConfiguration.Section, jwtConfiguration);

        string secretKey = jwtConfiguration.Key;

        services.AddAuthentication(opt => {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
       .AddJwtBearer(options => { 
           options.TokenValidationParameters = new TokenValidationParameters { 
               ValidateIssuer = true, 
               ValidateAudience = true, 
               ValidateLifetime = true, 
               ValidateIssuerSigningKey = true,
               ValidIssuer = jwtConfiguration.ValidIssuer, 
               ValidAudience = jwtConfiguration.ValidAudience,
               IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)) 
           }; 
       }); 
    }


    public static void ConfigureSwagger(this IServiceCollection services) { 
        services.AddSwaggerGen(s => { 
            s.SwaggerDoc("v1", new OpenApiInfo {
                Title = "Sudoku API", Version = "v1" }); 

            s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {  
                In = ParameterLocation.Header, 
                Description = "Place to add JWT with Bearer", 
                Name = "Authorization", 
                Type = SecuritySchemeType.ApiKey, 
                Scheme = "Bearer" });

            s.AddSecurityRequirement(new OpenApiSecurityRequirement() {{ 
                    new OpenApiSecurityScheme { 
                        Reference = new OpenApiReference { 
                            Type = ReferenceType.SecurityScheme, 
                            Id = "Bearer"
                        }, 
                        Name = "Bearer" }, 
                    new List<string>() 
                } 
            });
        }); 
    }

}


