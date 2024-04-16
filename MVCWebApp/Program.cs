using AzureStorageLibrary;
using AzureStorageLibrary.Services;

namespace MVCWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            ConnectionStrings.AzureStorageConnectionString = builder.Configuration.GetConnectionString("StorageConStr");

            builder.Services.AddScoped(typeof(INoSqlStorage<>),typeof(TableStorage<>));
            builder.Services.AddSingleton<IBlobStorage, BlobStorage>();

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

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Blobs}/{action=Index}/{id?}");

            app.Run();
        }
    }
}