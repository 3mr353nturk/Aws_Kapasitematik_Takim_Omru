using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using AWS_Kapasitematik_Takim_Omru_CoreAPI_v1.Domain;
using AWS_Kapasitematik_Takim_Omru_CoreAPI_v1.Hubs;
//using AWS_Kapasitematik_Takim_Omru_CoreAPI_v1.Domain;
using AWSServerless_CoreAPI_v5.Domain.Services;
using AWSServerless_CoreAPI_v5.Domain.UnitOfWork;
using AWSServerless_CoreAPI_v5.Repositories;
using AWSServerless_CoreAPI_v5.Security;
using AWSServerless_CoreAPI_v5.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using WebApplication16.Domain;

namespace AWS_Kapasitematik_Takim_Omru_CoreAPI_v1
{
    public class Startup
    {
        public const string AppS3BucketKey = "AppS3Bucket";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUserService, UserService>();
            //services.AddAWSService<AmazonS3Client>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITokenHandler, TokenHandler>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();


            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddAWSService<Amazon.S3.IAmazonS3>();
            services.AddLogging();
            services.AddSingleton<IAwsS3HelperService, AwsS3HelperService>();
            services.Configure<AwsS3BucketOptions>(Configuration.GetSection(nameof(AwsS3BucketOptions)))
                .AddSingleton(x => x.GetRequiredService<IOptions<AwsS3BucketOptions>>().Value);

            services.AddCors(opts =>
            {
                opts.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();



                });

            });

            //services.AddCors(opts => 
            //{
            //    opts.AddPolicy("CorsPolicy", builder =>
            //    {
            //        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().WithOrigins("https://1ns5809cw0.execute-api.eu-west-2.amazonaws.com/Prod");



            //    });

            //});

            services.Configure<TokenOptions>(Configuration.GetSection("TokenOptions"));

            var tokenOptions = Configuration.GetSection("TokenOptions").Get<TokenOptions>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(jwtbeareroptions =>
            {
                jwtbeareroptions.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = tokenOptions.Issuer,
                    ValidAudience = tokenOptions.Audience,
                    IssuerSigningKey = SignHandler.GetSecurityKey(tokenOptions.SecurityKey),
                    ClockSkew = TimeSpan.Zero


                };
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddDbContext<TakimOmruDBContext>(options =>
            {
                options.UseSqlServer(Configuration["ConnectionStrings:DefaultConnectionString"]);
            });

            // Add S3 to the ASP.NET Core dependency injection framework.
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //else
            //{
            //    app.UseHsts();
            //}
            app.UseWebSockets(new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
            });
            app.UseHttpsRedirection();
            app.UseAuthentication();
            //app.UseCors("CorsPolicy");
            app.UseSignalR(routes =>
            {
                routes.MapHub<SignalRHub>("/hubs/signalRHub");
            });
            

            app.UseMvc();
        }
    }
}
