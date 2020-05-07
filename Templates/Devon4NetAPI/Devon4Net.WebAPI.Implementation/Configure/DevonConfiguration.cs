﻿using System.Reflection;
using System.Security.Claims;
using Devon4Net.Domain.UnitOfWork.Common;
using Devon4Net.Domain.UnitOfWork.Enums;
using Devon4Net.Infrastructure.Common.Helpers;
using Devon4Net.Infrastructure.JWT.Common;
using Devon4Net.Infrastructure.JWT.Common.Const;
using Devon4Net.Infrastructure.RabbitMQ.Common;
using Devon4Net.Infrastructure.RabbitMQ.Samples.Handllers;
using Devon4Net.WebAPI.Implementation.Domain.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Devon4Net.WebAPI.Implementation.Configure
{
    /// <summary>
    /// DevonConfiguration
    /// </summary>
    public static class DevonConfiguration
    {
        /// <summary>
        /// Sets up the service dependency injection
        /// For example:
        /// services.AddTransient"ITodoService, TodoService"();
        /// services.AddTransient"ITodoRepository, TodoRepository"();
        /// Put your DI declarations here
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void SetupDevonDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            var assemblyToScan = Assembly.GetAssembly(typeof(DevonConfiguration));

            services.RegisterAssemblyPublicNonGenericClasses(assemblyToScan)
                .Where(x => x.Name.EndsWith("Service"))
                .AsPublicImplementedInterfaces();

            services.RegisterAssemblyPublicNonGenericClasses(assemblyToScan)
                .Where(x => x.Name.EndsWith("Repository"))
                .AsPublicImplementedInterfaces();

            SetupDatabase(services,configuration);
            SetupJwtPolicies(services);
            SetupRabbitHandlers(services);
        }

        private static void SetupRabbitHandlers(IServiceCollection services)
        {
            services.AddRabbitMqHandler<UserSampleRabbitMqHandler>(true);
        }

        /// <summary>
        /// Setup here your database connections.
        /// To use RabbitMq message backup declare the 'RabbitMqBackupContext' database setup
        /// PE: services.SetupDatabase&lt;RabbitMqBackupContext&gt;($"Data Source={FileOperations.GetFileFullPath("RabbitMqBackupSqLite.db")}", DatabaseType.Sqlite);
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        private static void SetupDatabase(IServiceCollection services, IConfiguration configuration)
        {
            services.SetupDatabase<TodoContext>(configuration, "Default", DatabaseType.InMemory);
            services.SetupDatabase<EmployeeContext>(configuration, "Employee", DatabaseType.InMemory);
        }

        private static void SetupJwtPolicies(IServiceCollection services)
        {
            services.AddJwtPolicy(AuthConst.DevonSamplePolicy, ClaimTypes.Role, AuthConst.DevonSampleUserRole);
        }
    }
}
