using AspNetCoreRateLimit;
using AutoMapper;
using Library.API.Authorization;
using Library.API.Authorization.Service;
using Library.API.Configuration;
using Library.API.Domain.Mappers.Profiles;
using Library.API.Domain.Models;
using Library.API.Domain.Results.Exceptions;
using Library.API.Extension;
using Library.API.Filters;
using Library.API.Infrastructure.Entity;
using Library.API.Infrastructure.Repository;
using Library.API.Infrastructure.Repository.Interfaces;
using Library.API.Middleware.Auth;
using Library.API.Middleware.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Library.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region Configuration        

            EnableFlags enableFlags = new EnableFlags();
            this.Configuration.GetSection("Features:EnableFlags").Bind(enableFlags);

            AuthOptions authOptions = new AuthOptions();
            this.Configuration.GetSection("Authentication").Bind(authOptions);

            services.Configure<EnableFlags>(this.Configuration.GetSection("Features:EnableFlags"));
            services.Configure<AuthAuthenticationOptions>(o => { o.Scheme = authOptions.Schema; });

            services.Configure<IpRateLimitOptions>((options =>
            {
                options.EnableEndpointRateLimiting = true;
                options.DisableRateLimitHeaders = true;
                options.StackBlockedRequests = false;
                options.IpWhitelist = new List<string>() { "192.168.0.0/24" };
                //options.EndpointWhitelist = new List<string>() { "get:*api/v1/books*" };
                options.GeneralRules = new System.Collections.Generic.List<RateLimitRule>()
                {
                     new RateLimitRule()
                    {
                        Endpoint = "*",
                        Limit = 4,
                        Period = "1s"
                    },
                    new RateLimitRule()
                    {                        
                        Endpoint = "*",
                        Limit = 15,
                        Period = "1m"
                    },
                     new RateLimitRule()
                    {
                        Endpoint = "*",
                        Limit = 100,
                        Period = "15m"
                    }
                };
            }));
            #endregion

            #region LifeCycles 
            #region Generics
            services.AddScoped(typeof(IRepository<>), typeof(RepositorySync<>));
            services.AddScoped(typeof(IPagedRepository<>), typeof(PagedRepository<>));
            #endregion

            services.AddScoped(typeof(IBookRepository), typeof(BookRepository));
            services.AddScoped(typeof(IAuthorRepository), typeof(AuthorRepository));
            services.AddScoped(typeof(LinkFilter));

            services.AddSingleton<IServiceSTS, ServiceSTS>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();                       
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            #endregion
                      
            services.AddMvc(setup =>
            {
                setup.ReturnHttpNotAcceptable = true;
                setup.OutputFormatters.Add(new XmlSerializerOutputFormatter());
            })
            .ConfigureApiBehaviorOptions(o =>
            {
                o.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var errors = actionContext.ModelState.Values.Where(e => e.ValidationState != Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid).SelectMany(e => e.Errors).Select(e => e.Exception == null ? e.ErrorMessage : e.Exception.Message).ToArray();
                    var msg = string.Join("| ", errors);

                    ErrorMessage error = new ErrorMessage((int)HttpStatusCode.NotFound, "Error validating parameters", msg);
                    return new BadRequestObjectResult(error);
                };
            })
            .AddJsonOptions(o => o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore)
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            if (enableFlags.UseSqlite)
            {                
                services.AddDbContext<ApplicationDbContext>(o => o.UseSqlite($"Data Source={Environment.ContentRootPath}/dblibrary.db"));
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(o => o.UseSqlServer(Configuration.GetConnectionString("LibraryConnection"), options => options.CommandTimeout(60)));
            }
            
            services.AddAutoMapper(typeof(MapperProfile));           
         
            services.AddApiVersioning((o =>
            {
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.ApiVersionReader = ApiVersionReader.Combine(new HeaderApiVersionReader("X-Version"), new QueryStringApiVersionReader("api-version"));
            }));
                   
            services.AddMemoryCache();
            services.AddHttpClient();

            services.AddResponseWithLink();
            services.AddSTS(authOptions.Url);

            if (enableFlags.UseSwagger)
            {
                services.AddSwaggerGen(o =>
                {
                    o.SwaggerDoc("v1", new OpenApiInfo()
                    {
                        Title = "Library API",
                        Description = "Sample Library API To Store Your Books",
                        Version = "v1",
                        Contact = new OpenApiContact
                        {
                            Name = "José Christiano Viana Junior",
                            Email = "library-api@gmail.com",
                            Url = new Uri("https://library-api.com"),
                        }
                    });
                    o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                    {
                        Scheme = "Bearer",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Name = "Authorization",
                        Description = "JWT authorization header"
                    });

                    o.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                            },
                            new List<string>()
                        }
                    });
                    

                    // Add a custom filter for settint the default values  
                    o.OperationFilter<SwaggerFilter>();
                });
            }          
            
            services.AddAuthentication(o =>
            {
                o.DefaultScheme = AuthAuthenticationDefaults.Schema;
                o.DefaultChallengeScheme = AuthAuthenticationDefaults.Schema;
            }).AddScheme<AuthAuthenticationOptions, AuthAuthenticationHandler>(AuthAuthenticationDefaults.Schema, null);          
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, 
                              IHostingEnvironment env, 
                              IOptionsMonitor<EnableFlags> options,
                              IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                if (options.CurrentValue.UseSqlite)
                {
                    // Initialize the data just for sqlite
                    app.InitializerDatabase(serviceProvider.GetRequiredService<ApplicationDbContext>());
                }
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            app.UseMiddleware<GlobalExceptionMiddleware>();

            if (options.CurrentValue.UseIpRateLimit)
            {
                app.UseMiddleware<GlobalIpRateLimitMiddleware>();
                //app.UseIpRateLimiting();
            }

            #region UseExceptionHandler
            //app.UseExceptionHandler(builder =>
            //{
            //    builder.Run(async context =>
            //    {
            //        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();

            //        if (exceptionHandlerFeature != null)
            //        {                      
            //            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            //            context.Response.ContentType = "application/json";

            //            var json = new
            //            {
            //                context.Response.StatusCode,
            //                Message = "An error occurred whilst processing your request",
            //                Detailed = exceptionHandlerFeature.Error
            //            };

            //            await context.Response.WriteAsync(JsonConvert.SerializeObject(json));
            //        }
            //    });
            //});
            #endregion

            app.UseAuthentication();  
            app.UseHttpsRedirection();

            if (options.CurrentValue.UseSwagger)
            {
                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger();

                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
                // specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Library API");
                });
            }           

            app.UseMvc();
        }
    }
}
