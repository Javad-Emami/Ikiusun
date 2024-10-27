using WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Config(builder.Configuration);
builder.Build().Config(builder.Configuration).Run();

