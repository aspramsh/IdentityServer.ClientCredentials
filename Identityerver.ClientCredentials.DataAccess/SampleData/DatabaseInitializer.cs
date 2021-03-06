﻿using Identityerver.ClientCredentials.DataAccess.DbContexts;
using IdentityServer.ClientCredentials.DataAccess.DbContexts;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Identityerver.ClientCredentials.DataAccess.SampleData
{
    public class DatabaseInitializer
    {
        public static void Initialize(IApplicationBuilder app, AuthDbContext context)
        {
            context.Database.EnsureCreated();

            InitializeTokenServerConfigurationDatabase(app);

            context.SaveChanges();
        }

        private static void InitializeTokenServerConfigurationDatabase(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.GetService<IServiceScopeFactory>()
                   .CreateScope();
            scope.ServiceProvider.GetRequiredService<CustomPersistedGrantDbContext>().Database.Migrate();

            var context = scope.ServiceProvider
                .GetRequiredService<CustomConfigurationDbContext>();
            context.Database.Migrate();
            if (!context.Clients.Any())
            {
                foreach (var client in Config.GetClients())
                {
                    context.Clients.Add(client.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in Config.GetIdentityResources())
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.ApiResources.Any())
            {
                foreach (var resource in Config.GetApiResources())
                {
                    context.ApiResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }
        }
    }
}
