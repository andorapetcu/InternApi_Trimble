using Microsoft.Extensions.Options;
using MongoDB.Bson;
using InternApi.Mapping;
using InternApi.Services;
using InternApi.Settings;
var builder = WebApplication.CreateBuilder(args);


MongoDB.Bson.Serialization.BsonSerializer.RegisterSerializer(
    typeof(Guid),
    new MongoDB.Bson.Serialization.Serializers.GuidSerializer(MongoDB.Bson.GuidRepresentation.Standard)
);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<MongoDBSettings>(
    builder.Configuration.GetSection(nameof(MongoDBSettings)));

builder.Services.AddSingleton<IMongoDBSettings>(sp =>
    sp.GetRequiredService<IOptions<MongoDBSettings>>().Value);

builder.Services.AddSingleton<IInternService, InternService>();

builder.Services.AddAutoMapper(typeof(MapperProfile).Assembly);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
