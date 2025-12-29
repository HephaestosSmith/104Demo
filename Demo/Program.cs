using Demo;
using Demo.Helpers;
using Demo.Interface;
using Demo.Middlewares;
using Demo.Settings;
using Demo.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Demo.Data.Context;

static async Task WaitForDb(string connStr)
{
    for (int i = 0; i < 10; i++)
    {
        try
        {
            using var conn = new SqlConnection(connStr);
            await conn.OpenAsync();
            return;
        }
        catch (Exception ex)
        {
            await Task.Delay(3000);
        }
    }

    throw new Exception("Database not ready");
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<JwtBearerHelper>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();

// 註冊 DbContext（使用 Default connection string）
// 先讀取連線字串以便後續 WaitForDb 使用
var connStr = builder.Configuration.GetConnectionString("Default");
if (string.IsNullOrEmpty(connStr))
{
    throw new InvalidOperationException("Connection string 'Default' not found in configuration.");
}

builder.Services.AddDbContext<DemoDbContext>(options =>
{
    options.UseSqlServer(connStr);
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// Swagger 設定
builder.Services.AddSwaggerServices();

var jwtBearer = new JwtBearer();
builder = jwtBearer.SetJwtBearer(builder);

// 等待資料庫可用（使用先前讀取的 connStr）
await WaitForDb(connStr!);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseExceptionMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();




class JwtBearer
{
    public WebApplicationBuilder SetJwtBearer(WebApplicationBuilder builder)
    {    
        var jwtSettingConfigSection = builder.Configuration.GetSection("JwtParameter");
        var jwtSetting = jwtSettingConfigSection.Get<JwtSetting>();
        builder.Services.Configure<JwtSetting>(jwtSettingConfigSection);

        builder.Services.AddScoped(x => JwtBearerHelper.GetTokenValidationParametersAsymmetric(jwtSetting));

        var tokenValidationParameters = JwtBearerHelper.GetTokenValidationParametersAsymmetric(jwtSetting);

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            o.TokenValidationParameters = tokenValidationParameters;
        });

        return builder;
    }

}
