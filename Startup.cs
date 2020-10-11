using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeXieCheng.API.Database;
using FakeXieCheng.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Formatters;
using AutoMapper;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using FakeXieCheng.API.Models;

namespace FakeXieCheng.API
{
    public class Startup
    {
        IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                                .AddJwtBearer(options =>
                                {
                                    var secretByte = Encoding.UTF8.GetBytes(Configuration["Authentication:SecretKey"]);
                                    options.TokenValidationParameters = new TokenValidationParameters()
                                    {
                                        ValidateIssuer = true,
                                        ValidIssuer = Configuration["Authentication:Issuer"],
                                
                                        ValidateAudience = true,
                                        ValidAudience = Configuration["Authentication:Audience"],
                                
                                        ValidateLifetime = true,
                                
                                        IssuerSigningKey = new SymmetricSecurityKey(secretByte)
                                    };
                                });

            services.AddControllers(setupAction =>
            {
                setupAction.ReturnHttpNotAcceptable = true;
                //setupAction.OutputFormatters.Add(
                //    new XmlDataContractSerializerOutputFormatter()
                //    );
            })
                .AddNewtonsoftJson(setupAction =>
                {
                    setupAction.SerializerSettings.ContractResolver =
                        new CamelCasePropertyNamesContractResolver();
                })
                .AddXmlDataContractSerializerFormatters()
            .ConfigureApiBehaviorOptions(setupAction =>
            {
                setupAction.InvalidModelStateResponseFactory = context =>
                  {
                      var problemDetail = new ValidationProblemDetails(context.ModelState)
                      {
                          Type = "无所谓",
                          Title = "数据验证失败",
                          Status = StatusCodes.Status422UnprocessableEntity,
                          Detail = "请看详细说明",
                          Instance = context.HttpContext.Request.Path
                      };
                      problemDetail.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);
                      return new UnprocessableEntityObjectResult(problemDetail)
                      {
                          ContentTypes = { "application/problem+json" }
                      };
                  };
            });
            //services.AddTransient<ITouristRouteRepository,MockTouristRouteRepository>();
            services.AddTransient<ITouristRouteRepository, TouristRouteRepository>();

            services.AddDbContext<AppDbContext>(option =>
            {
                //option.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=FakeXiechengDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
                //option.UseSqlServer("server=localhost;Database=FakeXiechengDb;User Id=sa;Password=PaSSword12!");
                option.UseSqlServer(Configuration["DbContext:ConnectionString"]);
            });
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //去哪
            app.UseRouting();
            //是谁
            app.UseAuthentication();
            //有何权限
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapGet("/test",async context=>
                //{
                //    await context.Response.WriteAsync("Hello My test!");
                //});
                //endpoints.MapGet("/", async context =>
                //{
                //    await context.Response.WriteAsync("Hello World!");
                //});
                endpoints.MapControllers();
            });
        }
    }
}
