using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BookApi.DataAccess;
using BookApi.models;
using BookApi.services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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

            Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext().
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

            services.AddHttpContextAccessor()
            .AddSingleton<IDatabaseClient, DatabaseClient>()
            .AddSingleton<IBookService, BookService>().
            AddSingleton(typeof(IBaseDAL<Book>), typeof(BookDAL))
            .AddSingleton<IGenreService, GenreService>()
            .AddSingleton(typeof(IBaseDAL<Genre>), typeof(GenreDAL))
            .AddSingleton<IMyListService, MyListService>()
            .AddSingleton<IMyListDAL, MyListDAL>()
            .AddControllers();

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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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
               await next();
           });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
