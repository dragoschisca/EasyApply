var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer(); //pentru SWAGGER
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Easy Apply API");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.Run();
