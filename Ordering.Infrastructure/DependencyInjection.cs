﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Domain.Repositories.Base;
using Ordering.Domain.Repositories;
using Ordering.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Ordering.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<OrderContext>(opt => opt.UseInMemoryDatabase(databaseName: "InMemoryDb"),
                                                ServiceLifetime.Singleton,
                                                ServiceLifetime.Singleton);

            //services.AddDbContext<OrderContext>(options =>
            //        options.UseSqlServer(
            //            configuration.GetConnectionString("OrderConnection"),
            //            b => b.MigrationsAssembly(typeof(OrderContext).Assembly.FullName)), ServiceLifetime.Singleton);

            //Add Repositories
            //services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            //services.AddTransient<IOrderRepository, OrderRepository>();

            return services;
        }
        }
}
