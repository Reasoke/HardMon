
using System.Text.Json.Serialization;
using server.Classes;
using server.Repository;
using server.Services;

namespace server
{
    public class Program {

        public static void Main(string[] args) {

            var builder = WebApplication.CreateBuilder(args);

            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();

            app.UseCors(policy => {
                policy
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                    //.AllowCredentials()
                    //.WithOrigins(allowedHosts)
                    ;
            });
            
            app.UseMiddleware<ErrorHandlingMiddleware>();
            
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI(c => {
                    c.DefaultModelsExpandDepth(-1);
                    // c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1");
                });
            }
            else {
                app.UseHttpsRedirection();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration) {

            // Add services to the container.
            services.AddSingleton<ISettingsProvider>(provider => new SettingsProvider(configuration));

            services.AddSingleton<IUsersService>(provider => new UsersService(
                provider.GetService<IUsersRepository>()
            ));
            services.AddSingleton<IUsersRepository>(provider => new UsersRepository(
                provider.GetService<ISettingsProvider>()
            ));
            
            services.AddSingleton<IAccountsRepository>(provider => new AccountsRepository(
                provider.GetService<ISettingsProvider>()
            ));
            services.AddSingleton<IAccountsService>(provider => new AccountsService(
                provider.GetService<IAccountsRepository>()
            ));
            
            services.AddSingleton<IDevicesRepository>(provider => new DevicesRepository(
                provider.GetService<ISettingsProvider>()
            ));
            services.AddSingleton<IDevicesService>(provider => new DevicesService(
                provider.GetService<IDevicesRepository>()
            ));

            services.AddSingleton<ISensorsService>(provider => new SensorsService(
                provider.GetService<ISensorsRepository>(),
                provider.GetService<IDevicesRepository>()
            ));
            services.AddSingleton<ISensorsRepository>(provider => new SensorsRepository(
                provider.GetService<ISettingsProvider>()
            ));
            
            services.AddSingleton<ISystemAdminService>(provider => new SystemAdminService(
                provider.GetService<ISystemAdminRepository>()
            ));
            services.AddSingleton<ISystemAdminRepository>(provider => new SystemAdminRepository(
                provider.GetService<ISettingsProvider>()
            ));

            //services.AddTransient<BooksRepository>(provider => new BooksRepository(
            //    provider.GetService<ISettingsProvider>()
            //));

            //user -> Controller -> Service -> Repository -> (SettingsProvider) -> DB => Repository => Service (ToDto) => Controller => user

            services.AddControllers(config => {
                config.Filters.Add(typeof(AuthCookieCheckFilterAttribute));
            }).AddJsonOptions(x => {
                x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

        }
    }
}
