using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using BookApi.models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BookApi.DataAccess;
using BookApi.services;
using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Serilog.Context;
using System.Linq;
using BookApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(builder.Configuration));
var configuration = builder.Configuration.AddEnvironmentVariables().Build();

var serviceCollections = builder.Services;
serviceCollections.AddCors(c => c.AddPolicy("policy", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
serviceCollections.Configure<BookstoreDatabaseSettings>(configuration.GetSection(nameof(BookstoreDatabaseSettings)));
serviceCollections.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
                    {
                        var jwtOptions = configuration.GetSection("JWT").Get<JwtOption>();
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidAudience = jwtOptions.Audience,
                            ValidIssuer = jwtOptions.Issuer,
                            NameClaimType = ClaimTypes.NameIdentifier,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.secret_key))
                        };
                    });

serviceCollections.AddLocalization(options => options.ResourcesPath = "Resources");
serviceCollections.AddHttpContextAccessor()
                   .AddSingleton<IBookDBContext, BookDBContext>()
                   .AddScoped<IBookService, BookService>()
                   .AddScoped<IBookDAL, BookDAL>()
                   .AddControllers()
                   .AddDataAnnotationsLocalization(options => options.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(Localizations)));

serviceCollections.AddSwaggerGen(c =>
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

var cultures = configuration.GetSection("SupportedCultures").Get<string[]>();
var supportedCultures = cultures.Select(c => new System.Globalization.CultureInfo(c)).ToList();

var app = builder.Build();

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(configuration["DefaultCulture"]),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures,

}.AddInitialRequestCultureProvider(new CustomRequestCultureProvider()));

if (app.Environment.IsDevelopment())
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
    LogContext.PushProperty("UserId", userId);
    LogContext.PushProperty("IP", context.Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4());
    await next();
});

app.MapControllers();
app.Run();