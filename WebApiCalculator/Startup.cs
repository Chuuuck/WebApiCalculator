using System;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using WebApiCalculator.Core.Domain.Interfaces;
using WebApiCalculator.Infrastructure;
using WebApiCalculator.Infrastructure.Repositories;
using WebApiCalculator.Infrastructure.Service;

namespace WebApiCalculator
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add Db context.
            services.AddDbContextPool<CalculatorDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("CalculatorDb"));
            });

            // Add repository.
            services.AddScoped<ICalculationRepository, CalculationRepository>();
            // Add calculator service.
            services.AddTransient<ICalculator, Calculator>();
            // Add Automapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddControllers();

            // Add Swagger
            services.AddSwaggerGen(setupAction =>
            {
                setupAction.SwaggerDoc("CalculatorOpenAPISpecification", new OpenApiInfo
                {
                    Title = "Calculator Web API",
                    Version = "1"
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // Configure Swagger
            app.UseSwagger();
            app.UseSwaggerUI(setupAction =>
            {
                setupAction.SwaggerEndpoint("/swagger/CalculatorOpenAPISpecification/swagger.json", "Calculator API");
            });

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
