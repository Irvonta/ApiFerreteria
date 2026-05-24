var builder = WebApplication.CreateBuilder(args);

// ======================
// CORS
// ======================
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirFrontend",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// ======================
// CONTROLLERS
// ======================
builder.Services.AddControllers();

// ======================
// SWAGGER
// ======================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ======================
// SWAGGER
// ======================
app.UseSwagger();
app.UseSwaggerUI();

// ======================
// CORS
// ======================
app.UseCors("PermitirFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();
