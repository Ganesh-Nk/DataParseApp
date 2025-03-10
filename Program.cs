using DataParseApp.Services;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient<XmlService>();  
builder.Services.AddControllersWithViews();    
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
