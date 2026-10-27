using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PlacementDashboard.Models;

public class Placement
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string StudentId { get; set; } = "";
    public string CompanyId { get; set; } = "";
    public double PackageLPA { get; set; }
    public string Status { get; set; } = "Applied";
    public DateTime PlacementDate { get; set; } = DateTime.UtcNow;
}