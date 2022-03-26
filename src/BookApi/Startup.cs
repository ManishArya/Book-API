using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using BookApi.DataAccess;
using BookApi.models;
using BookApi.services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Context;

namespace BookApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            Log.Logger = new LoggerConfiguration().
                 ReadFrom.Configuration(configuration)
                 .CreateLogger();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(o => o.AddPolicy("policy", builder =>
            {
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            }));

            services.Configure<BookstoreDatabaseSettings>(
              Configuration.GetSection(nameof(BookstoreDatabaseSettings)));

            services.AddSingleton<IBookstoreDatabaseSettings>(sp =>
                    sp.GetRequiredService<IOptions<BookstoreDatabaseSettings>>().Value);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidAudience = Configuration["jwt:audience"],
                            ValidIssuer = Configuration["jwt:issuer"],
                            NameClaimType = ClaimTypes.NameIdentifier,
                            IssuerSigningKey = new
                               SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["jwt:secret_key"]))
                        };
                    });
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddHttpContextAccessor()
            .AddSingleton<IDatabaseClient, DatabaseClient>()
            .AddScoped<IBookService, BookService>().
            AddScoped(typeof(IBaseDAL<Book>), typeof(BookDAL))
            .AddScoped<IGenreService, GenreService>()
            .AddScoped(typeof(IBaseDAL<Genre>), typeof(GenreDAL))
            .AddScoped<IMyListService, MyListService>()
            .AddScoped<IMyListDAL, MyListDAL>()
            .AddControllers()
            .AddDataAnnotationsLocalization(options =>
            {
                options.DataAnnotationLocalizerProvider = (type, factory) =>
                    factory.Create(typeof(localizations));
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BookApi", Version = "v1" });
                c.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement{
                     {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "bearerAuth" }
                            },
                            new List<string>()
                        }
                   });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var supportedCultures = new[] { "en-US", "hi" };
            var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            app.UseRequestLocalization(localizationOptions);

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("policy");

            app.UseAuthorization();

            app.Use(async (context, next) =>
           {
               var userId = context.User.Identity.IsAuthenticated ? context.User.Identity.Name : "";
               LogContext.PushProperty("UserName", userId);
               LogContext.PushProperty("IP", context.Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4());
               await next();
           });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
