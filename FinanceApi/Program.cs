using FinanceApi.Data;
using FinanceApi.Models;
using FinanceApi.Repositories;
using FinanceApi.Repositories.Interfaces;
using FinanceApi.Services;
using FinanceApi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

public class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IIncomeRepository, IncomeRepository>();
        builder.Services.AddScoped<IGoalRepository, GoalRepository>();
        builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
        builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
        builder.Services.AddScoped<IBudgetRepository, BudgetRepository>();
        builder.Services.AddScoped<IAuthorizationInviteRepository, AuthorizationInviteRepository>();
        builder.Services.AddScoped<IAuthorizeRepository, AuthorizeRepository>();
        builder.Services.AddScoped<IRoleRepository, RoleRepository>();

        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IIncomeService, IncomeService>();
        builder.Services.AddScoped<IGoalService, GoalService>();
        builder.Services.AddScoped<IExpenseService, ExpenseService>();
        builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<IBudgetService, BudgetService>();
        builder.Services.AddScoped<IFinancialService, FinancialService>();
        builder.Services.AddScoped<IAuthorizeService, AuthorizeService>();





        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });
            options.OperationFilter<SecurityRequirementsOperationFilter>();
        });


        builder.Services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

        builder.Services.AddAuthorization();
        //RequireConfirmedAccount set to false just for development purposes.
        builder.Services.AddIdentityApiEndpoints<User>(options => options.SignIn.RequireConfirmedAccount = false)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<DataContext>();


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapIdentityApi<User>();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();


        using (var scope = app.Services.CreateScope())
        {

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var roles = new[] { "Admin", "Manager", "Member", "View", "Edit" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

        }

        using (var scope = app.Services.CreateScope())
        {

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            string email = "Admin@Admin.com";
            string password = "Testing!2";
            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new User();
                user.Email = email;
                user.UserName = email;


                // set to true to overide the need to confirm when registering
                user.EmailConfirmed = true;



                await userManager.CreateAsync(user, password);

                await userManager.AddToRoleAsync(user, "Admin");
            }



        }



        app.Run();
    }
}