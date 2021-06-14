using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Kubernetes;

namespace API_Gateway
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
			services.AddCors(options =>
			{
				options.AddDefaultPolicy(
					builder =>
					{
						builder.WithOrigins("http://localhost:3000", "https://lifelinks.nl").AllowAnyMethod().AllowAnyHeader();
						builder.WithOrigins("https://*.vercel.app").AllowAnyMethod().AllowAnyHeader();
					});
			});

			services.AddOcelot().AddKubernetes();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseCors();

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();

			app.UseAuthentication();

			app.UseOcelot().Wait();

			app.UseEndpoints(endpoints =>
			{
			});
		}
	}
}