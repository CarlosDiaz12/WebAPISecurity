using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Text;

namespace WebAPISecurity
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private const string _corsPolicy = "corsPolicyName";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // add https protocol
            /*
            services.Configure<MvcOptions>(o =>
            {
                o.Filters.Add<RequireHttpsAttribute>();
            });
            */

            // cors policy
            services.AddCors(options =>
            {
                options.AddPolicy(_corsPolicy,
                    c => c
                            .WithOrigins("http://127.0.0.1:5500", "http://localhost:4200")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                    );
            });

            services.AddControllers();
            // jwt authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
             .AddJwtBearer(options =>
             {
                 options.RequireHttpsMetadata = false;
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidateLifetime = true,
                     ValidateIssuerSigningKey = true,
                     ValidIssuer = Configuration["Auth:Issuer"],
                     ValidAudience = Configuration["Auth:Audience"],
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Auth:Key"]))
                 };
             });

            services.AddAuthorization(config =>
            {
                config.AddPolicy("SuperUsers", p => p.RequireClaim("SuperUser", "True"));
            });


            // add data protection API
            services.AddDataProtection();

            var serviceProvider = services.FirstOrDefault(x => x.ServiceType == typeof(IDataProtectionProvider));
            services.AddAutoMapper(options =>
            {
                options.ConstructServicesUsing(type => new IdProtectorConverter(serviceProvider as IDataProtectionProvider));
            },
            AppDomain.CurrentDomain.GetAssemblies());

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            app.UseCors(_corsPolicy);
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
