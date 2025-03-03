using BAL.Interfaces;
using BAL.Services;
using DAL.Database;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);

var jwtConfig = builder.Configuration.GetSection("jwt");

builder.Services.AddAuthentication(x=>{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        // Remove the line that sets the AccessDeniedPath
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtConfig["issuer"],  // The issuer of the token (e.g., your app's URL)
            ValidAudience = jwtConfig["audience"], // The audience for the token (e.g., your API)
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["key"] ?? "")), // The key to validate the JWT's signature
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.Name 
        };

        options.Events = new JwtBearerEvents
        {
            // OnMessageReceived = context =>
            // {
            //     // Check for the token in cookies
            //     var token = context.Request.Cookies["AuthToken"]; // Change "AuthToken" to your cookie name if it's different
            //     if (!string.IsNullOrEmpty(token))
            //     {
            //         context.Request.Headers["Authorization"] = "Bearer " + token;
            //     }
            //     return Task.CompletedTask;
            // }

            OnMessageReceived = context =>
            {
                var token = context.HttpContext.Request.Cookies["AuthToken"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            }
        };
    }
);

builder.Services.AddDbContext<PizzaShopDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IHashServices, HashServices>();
builder.Services.AddScoped<IAuthServices, AuthServices>();
builder.Services.AddScoped<IJwtservices, JwtServices>();
builder.Services.AddScoped<IEmailServices, EmailServices>();
builder.Services.AddScoped<IDashServices, DashServices>();
builder.Services.AddScoped<IAdminUsersServices, AdminUsersServices>();
builder.Services.AddScoped<IRolePermissionServices, RolePermissionServices>();
builder.Services.AddScoped<IMenuServices, MenuServices>();




// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

app.Run();
