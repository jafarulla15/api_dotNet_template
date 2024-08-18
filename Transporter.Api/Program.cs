using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Transporter.API.Auth;
using Transporter.Common.Helper.FileFormatCheck;
using Transporter.Common.QueryHelper;
using Transporter.DataAccess;
using Transporter.Services.Interface;
using Transporter.Services.Services;
using Transporter.Services;
using Transporter.Repository;
using Transporter.Services.Services.Log;
using DocumentFormat.OpenXml.Office.CustomUI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "CorsPolicy",
        policyBuilder =>
        {
            policyBuilder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });

    //options.AddPolicy(name: "AllowSpecificOrigin",
    //   policyBuilder =>
    //   {
    //        policyBuilder.WithOrigins("http://10.32.2.198:5055")
    //            .AllowAnyMethod()
    //            .AllowAnyHeader()
    //            .AllowAnyOrigin();
    //   });

});


builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                                  \r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,

            },
            new List<string>()
        }
    });
    c.CustomSchemaIds(i => i.FullName);
});

builder.Services.AddDbContext<SbDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DBCON"));
});

//builder.Services.AddDbContext<SbDbContext>(options =>
//        options.UseSqlServer(builder.Configuration.GetConnectionString("DBCON")),
//    ServiceLifetime.Scoped);


builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

builder.Services.AddScoped<IAccessTokenService, AccessTokenService>();
builder.Services.AddScoped<IActionsService, ActionsService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<ISystemUserService, SystemUserService>();
builder.Services.AddScoped<IUserSessionService, UserSessionService>();
builder.Services.AddScoped<ISecurityService, SecurityService>();
builder.Services.AddScoped<BuildDynamicFilter>();
builder.Services.AddScoped<IFileFormatChecker, FileFormatChecker>();

builder.Services.AddScoped<IUnitOfWork<SbDbContext>, UnitOfWork<SbDbContext>>();
builder.Services.AddScoped<IExceptionLogService, ExceptionLogService>();

builder.Services.AddScoped<IEmployeeServices, EmployeeServices>();
builder.Services.AddScoped<ICompanyServices, CompanyServices>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else if (app.Environment.IsProduction())
{
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Versioned API v1.0");
    });
}


app.UseSwagger();
app.UseSwaggerUI();
app.UseDeveloperExceptionPage();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.UseAuthorization();
app.MapControllers();
app.UseNSVAuthentication();
app.UseNSVAuthorization();
app.Run();
