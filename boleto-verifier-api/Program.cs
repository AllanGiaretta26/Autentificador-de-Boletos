using BoletoVerifier.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<BoletoParser>();
builder.Services.AddScoped<EmailService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Swagger sempre ativo em desenvolvimento
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("PermitirFrontend");
// Removido o UseHttpsRedirection — não precisa em dev
app.MapControllers();

app.Run();