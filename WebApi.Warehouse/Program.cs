using Swashbuckle.AspNetCore.SwaggerUI;
using WebApi.Warehouse;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddWebApiServices(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(p =>
{
    p.DocExpansion(DocExpansion.None);
    p.EnableDeepLinking();
});

app.UseRouting();
app.MapControllers();
app.Run();
