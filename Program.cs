using Kpakam;
using Kpakam.CodeBased;
using Kpakam.Helper;
using Kpakam.Interface;
using Kpakam.Model;

var builder = WebApplication.CreateBuilder(args);
//setup cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(hostname => true); });
});
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

CoupleServices(builder); 

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
//use cors
app.UseCors(builder => builder
                   .AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader());

app.Run();

void CoupleServices(WebApplicationBuilder builder)
{ 
    builder.Services.Configure<appSettings>(builder.Configuration.GetSection(Constant.appKeys));
    string LogPath = Path.Combine(AppContext.BaseDirectory, Constant.writepath);

    string connectionString = builder.Configuration[Constant.ConnectionStringKey];
    builder.Services.AddSingleton<ISqlHelperService, SqlHelperService>(sp => new SqlHelperService(connectionString)); 
    //builder.Services.AddScoped<IUsers, Users>();
    builder.Services.AddScoped<IErrorLogger, ErrorLogger>(sp => new ErrorLogger(LogPath));
    //  builder.Services.AddSingleton(sp => new clsNINMgt(connectionString));         
    builder.Services.AddScoped<IdataAccess, dataAccess>(sp => new dataAccess(connectionString)); 
}
 