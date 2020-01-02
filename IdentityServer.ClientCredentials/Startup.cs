using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Identityerver.ClientCredentials.DataAccess.DbContexts;
using Identityerver.ClientCredentials.DataAccess.SampleData;
using IdentityServer.ClientCredentials.DataAccess.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityServer.ClientCredentials
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AuthDbContext>(options =>
            options.UseNpgsql(Configuration.GetConnectionString("IdentityServerConnection")));

            services.AddIdentityServer()
                    .AddDeveloperSigningCredential()
                    .AddConfigurationStore<CustomConfigurationDbContext>(option =>
                           option.ConfigureDbContext = builder => builder.UseNpgsql(Configuration.GetConnectionString("IdentityServerConnection"), options =>
                           options.MigrationsAssembly("IdentityServer.ClientCredentials.DataAccess")))
                    .AddOperationalStore<CustomPersistedGrantDbContext>(option =>
                           option.ConfigureDbContext = builder => builder.UseNpgsql(Configuration.GetConnectionString("IdentityServerConnection"), options =>
                           options.MigrationsAssembly("IdentityServer.ClientCredentials.DataAccess")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AuthDbContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            DatabaseInitializer.Initialize(app, context);
            app.UseIdentityServer();
        }
    }
}
