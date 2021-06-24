using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Kubernetes;
using System;

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
				options.AddPolicy("CorsPolicy",
					builder =>
					{
						builder.WithOrigins("http://localhost:3000", "https://lifelinks.nl", "https://*.vercel.app").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
					});
			});

			var authenticationProviderKey = "TestKey";
			Action<IdentityServerAuthenticationOptions> opt = o =>
			{
				o.Authority = "https://api.lifelinks.nl";
				o.ApiName = "apigateway";
				o.SupportedTokens = SupportedTokens.Both;
				o.RequireHttpsMetadata = false;
				o.ApiSecret = "lifelinksidentitysecret";
			};

			services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
				.AddIdentityServerAuthentication(authenticationProviderKey, opt);


			services.AddOcelot().AddKubernetes();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseCors("CorsPolicy");

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