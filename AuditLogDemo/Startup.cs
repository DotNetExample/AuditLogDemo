using AuditLog.EF;
using AuditLogDemo.EF;
using AuditLogDemo.Fliters;
using AuditLogDemo.Helper;
using AuditLogDemo.Models;
using AuditLogDemo.Services;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuditLogDemo
{
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    /// <summary>
    /// autofac����
    /// </summary>
    public ILifetimeScope AutofacContainer { get; private set; }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        //�����־�洢
        services.AddDbContext<AuditLogDBContent>(options =>
        {
            string conn = Configuration.GetConnectionString("LogDB");
            options.UseSqlite(conn, options =>
            {
                options.MigrationsAssembly("AuditLogDemo");
            });
        });

        //����ע��
        //Scoped��һ�����󴴽�һ��
        //services.AddScoped<IRepository<AuditInfo>, AuditLogRepository>();
        ////ÿ�δ���һ��
        //services.AddTransient<IAuditLogService, AuditLogService>();

        services.AddControllers(options =>
        {
            options.Filters.Add(typeof(AuditLogActionFilter));
        });

    }

    /// <summary>
    /// ������������ConfigureServices��ִ��
    /// </summary>
    /// <param name="builder"></param>
    public void ConfigureContainer(ContainerBuilder builder)
    {
        // ֱ����Autofacע�������Զ���� 
        builder.RegisterModule(new AutofacModuleRegister());
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        //autofac ���� ��ѡ
        this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

    }
}
}
