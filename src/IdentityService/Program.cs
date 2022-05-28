using IdentityService;
using IdentityService.Pages;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddIdentityServer()
	.AddInMemoryIdentityResources(Config.IdentityResources)
	.AddInMemoryApiScopes(Config.ApiScopes)
	.AddInMemoryClients(Config.Clients)
	.AddTestUsers(TestUsers.Users);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();
app.UseIdentityServer();
app.UseAuthorization();

app.MapRazorPages().RequireAuthorization();

app.Run();
