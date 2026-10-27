using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PlacementDashboard.Models;

public class Company
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Name { get; set; } = "";
    public string JobRole { get; set; } = "";
    public double PackageLPA { get; set; }
    public string Location { get; set; } = "";
}