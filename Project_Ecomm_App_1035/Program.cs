using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Project_Ecomm_App_1035.DataAccess.Data;
using Project_Ecomm_App_1035.DataAccess.Repository;
using Project_Ecomm_App_1035.DataAccess.Repository.IRepository;
using Project_Ecomm_App_1035.Utility;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("conStr");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
//builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
//builder.Services.AddScoped<ICoverTypeRepository, CoverTypeRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
builder.Services.Configure<EmailSetting>(builder.Configuration.GetSection("EmailSetting"));
//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddIdentity<IdentityUser, IdentityRole>().
    AddDefaultTokenProviders().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddRazorPages();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied" ;
    options.LogoutPath = "/Identity/Account/Logout" ;
});
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly=true;
    options.Cookie.IsEssential = true;

});

builder.Services.AddAuthentication().AddFacebook(option =>
{
    option.AppId = "1207105856648462";
    option.AppSecret = "3a6fb3b158d8c84a5810b7f7e8e99f7f";
});
builder.Services.AddAuthentication().AddGoogle(option =>
{
    option.ClientId = "78127766607-t8hve2ut7mhsplcfmdi6hvc7kfvv904e.apps.googleusercontent.com";
    option.ClientSecret = "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";
});

builder.Services.AddAuthentication().AddTwitter(option =>
{
    option.ConsumerKey = "9h1ZckNZQ5mJX0N4vG2SUOk5V";
    option.ConsumerSecret = "YUIWO85ssa2xtyK0k7N6UlYFYTYVCssuMXbwZRmff94FE7fE1z";

});

builder.Services.AddAuthentication().AddInstagram(option =>
{
    option.ClientId = "9778024168936677";
    option.ClientSecret = "b8728a0c5cecd974af32012dc233464d";
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
StripeConfiguration.ApiKey = builder.Configuration.GetSection("stripe")["Secretkey"];
app.UseSession(); // for use sessions
app.UseAuthentication(); // for login user authentication
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();

