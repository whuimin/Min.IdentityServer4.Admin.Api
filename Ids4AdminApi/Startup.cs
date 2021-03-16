#pragma warning disable CS1591
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Net.Mime;
using System.Reflection;

using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

using IdentityServer4.EntityFramework.Storage;

using Ids4AdminApi.Models;

namespace Ids4AdminApi
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
			services.AddConfigurationDbContext(options =>
			{
				options.ConfigureDbContext = builder =>
				{
					builder.UseSqlServer(Configuration["ConnectionStrings:Ids4ConfigurationDb"]);
				};
			});

			services.AddOperationalDbContext(options =>
			{
				options.ConfigureDbContext = builder =>
				{
					builder.UseSqlServer(Configuration["ConnectionStrings:Ids4OperationalDb"]);
				};
			});

			services.AddCors(options =>
			{
				options.AddDefaultPolicy(
				builder =>
				{
					builder.WithOrigins(Configuration.GetSection("AppSettings:Origins").Get<string[]>()).AllowAnyHeader();
				});
			});

			services.AddControllers(options =>
			{
				var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireClaim("scope", Configuration["AppSettings:ServiceScope"]).Build();
				options.Filters.Add(new AuthorizeFilter(policy));
			})
			.AddJsonOptions(options => {
				options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
			})
			.ConfigureApiBehaviorOptions(options =>
			{
				options.InvalidModelStateResponseFactory = context =>
				{
					var actionExecutingContext = context as ActionExecutingContext;
					var request = actionExecutingContext.ActionArguments.Values.Select(v => (v as Request)).FirstOrDefault(v => v != null);

					var response = new Response()
					{
						Header = new ResponseHeader()
						{
							Version = request?.Header?.Version ?? 0,
							ResponseId = request?.Header?.RequestId,
							IsSuccess = false
						}
					};

					response.Header.Errors = context.ModelState.SelectMany(ms => ms.Value.Errors.Select(e => new ResponseError()
					{
						Code = ResultCode.ParameterInvalid.ToString(),
						Message = e.ErrorMessage
					}));

					return new ObjectResult(response);
				};
			});

			// accepts any access token issued by identity server
			services.AddAuthentication("Bearer")
			.AddJwtBearer("Bearer", options =>
			{
				options.Authority = Configuration["AppSettings:IdpServer"];

				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateAudience = false
				};
			});

			services.AddSwaggerGen(options =>
			{
				options.SwaggerDoc("v1", new OpenApiInfo
				{
					Title = "Ids4AdminApi",
					Version = "v1",
					Contact = new OpenApiContact
					{
						Name = "Huimin Wang",
						Email = "w_huimin@tom.com"
					}
				});

				var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
				options.IncludeXmlComments(xmlPath);
			});

			services.AddHealthChecks();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.Use(async (context, next) =>
			{
				context.Request.EnableBuffering();
				await next.Invoke();
			});

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ids4AdminApi v1"));
			}
			else
			{
				app.UseExceptionHandler(builder =>
				{
					builder.Run(async httpContext =>
					{
						var jsonSerializerOptions = new JsonSerializerOptions()
						{
							PropertyNameCaseInsensitive = true
						};
						httpContext.Request.Body.Seek(0, SeekOrigin.Begin);
						int version = 0;
						string requestId = null;
						try
						{
							var request = await httpContext.Request.ReadFromJsonAsync<Request>(jsonSerializerOptions);
							version = request?.Header?.Version ?? 0;
							requestId = request?.Header?.RequestId;
						}
						catch { }

						var response = new Response()
						{
							Header = new ResponseHeader()
							{
								Version = version,
								ResponseId = requestId,
								IsSuccess = false,
								Errors = new List<ResponseError>()
								{
									new ResponseError
									{
										Code = ResultCode.SystemError.ToString(),
										Message = "System error."
									}
								}
							}
						};
						var jsonSerializerOptions1 = new JsonSerializerOptions()
						{
							PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
							Encoder = JavaScriptEncoder.Create(new TextEncoderSettings(UnicodeRanges.All))
						};
						var result = JsonSerializer.Serialize(response, jsonSerializerOptions1);

						httpContext.Response.ContentType = "application/json; charset=utf-8";
						await httpContext.Response.WriteAsync(result);
					});
				});
			}

			//Use health check service¡£
			app.UseHealthChecks("/health", new HealthCheckOptions()
			{
				ResponseWriter = async (httpContext, healthReport) =>
				{
					httpContext.Response.ContentType = MediaTypeNames.Application.Json;

					var jsonSerializerOptions = new JsonSerializerOptions()
					{
						PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
						Encoder = JavaScriptEncoder.Create(new TextEncoderSettings(UnicodeRanges.All))
					};
					var result = JsonSerializer.Serialize(new
					{
						Status = healthReport.Status.ToString(),
						Errors = healthReport.Entries.Select(e => new { e.Key, Value = Enum.GetName(typeof(HealthStatus), e.Value.Status) })
					}, jsonSerializerOptions);

					httpContext.Response.ContentType = "application/json; charset=utf-8";
					await httpContext.Response.WriteAsync(result);
				}
			});

			//app.UseHttpsRedirection();

			app.UseRouting();

			app.UseCors();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapControllerRoute(name: "default", pattern: "{controller=Index}/{action=Index}/{id?}");
			});
		}
	}
}
