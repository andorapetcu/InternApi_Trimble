using Microsoft.Extensions.Options;
using MongoDB.Bson;
using Workshops.Mapping;
using Workshops.Services;
using Workshops.Settings;
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
