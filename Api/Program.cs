using Api.Conversoes;
using Api.Extensoes;
using Application;
using Application.Middlewares;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Host.ConfigurarLogs();

builder.Services.AddControllers()
	.AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter()); });

builder.Services.ConfigurarSwagger();

builder.Services.AddApplication()
	.AddInfrastructure(builder.Configuration);

builder.Services.ConfigurarAuth(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<TratamentoErrosMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();