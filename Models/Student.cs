using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PlacementDashboard.Models;

public class Student
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Department { get; set; } = "";
    public double CGPA { get; set; }
    public string PlacementStatus { get; set; } = "Pending";
}