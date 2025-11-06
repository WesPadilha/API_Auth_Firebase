using FirebaseAdmin;
using FirebaseAuthApi.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;

// Criação e configuração do ambiente do aplicação
var builder = WebApplication.CreateBuilder(args);

// Caminho da Chave .Json
var jsonPath = @"C:\Users\wesley\Desktop\Códigos\Auth\AuthFirebase\Data\unicoins-4178c-firebase-adminsdk-fbsvc-804494ec8e.json";

// Cria a credencial explicitando o tipo ServiceAccountCredential
var serviceAccountCredential = CredentialFactory.FromFile<ServiceAccountCredential>(jsonPath).ToGoogleCredential();

// Inicializa o banco de dados Firebase pela Chave .Json
FirebaseApp.Create(new AppOptions
{
    Credential = serviceAccountCredential
});

// Registra o sistema e Serivços (mais especificamente o Firebase)
builder.Services.AddScoped<FirebaseAuthService>();
// Registra o sistema dos Controllers, sem ele, o arquivo não consegue interpretar as rotas dos controllers
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Insira o token JWT no formato: Bearer {seu token}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


// Constroi o aplicativo
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

// Ativa a rota dos Controllers
app.MapControllers();
// Roda a aplicaçõa
app.Run();

