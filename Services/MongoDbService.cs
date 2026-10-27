using MongoDB.Driver;
using PlacementDashboard.Models;

namespace PlacementDashboard.Services;

public class MongoDbService
{
    private readonly IMongoCollection<Student> _students;
    private readonly IMongoCollection<Company> _companies;
    private readonly IMongoCollection<Placement> _placements;

    public MongoDbService(IConfiguration configuration)
    {
        var connectionString =
            configuration["MongoDB:ConnectionString"]
            ?? configuration.GetConnectionString("MongoDB")
            ?? "mongodb://localhost:27017";

        var databaseName = configuration["MongoDB:DatabaseName"] ?? "PlacementDB";

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);

        _students = database.GetCollection<Student>("Students");
        _companies = database.GetCollection<Company>("Companies");
        _placements = database.GetCollection<Placement>("Placements");
    }

    // Dashboard statistics
    public Task<long> GetTotalAsync() =>
        _students.CountDocumentsAsync(
            FilterDefinition<Student>.Empty);

    public Task<long> GetPlacedAsync() =>
        _students.CountDocumentsAsync(
            s => s.PlacementStatus == "Placed");

    public Task<long> GetTotalCompaniesAsync() =>
        _companies.CountDocumentsAsync(
            FilterDefinition<Company>.Empty);

    // Read records
    public Task<List<Student>> GetStudentsAsync() =>
        _students.Find(FilterDefinition<Student>.Empty)
            .Limit(100)
            .ToListAsync();

    public Task<List<Company>> GetCompaniesAsync() =>
        _companies.Find(FilterDefinition<Company>.Empty)
            .Limit(100)
            .ToListAsync();

    public Task<List<Placement>> GetPlacementsAsync() =>
        _placements.Find(FilterDefinition<Placement>.Empty)
            .Limit(100)
            .ToListAsync();

    // Add records
    public Task AddStudentAsync(Student student) =>
        _students.InsertOneAsync(student);

    public Task AddCompanyAsync(Company company) =>
        _companies.InsertOneAsync(company);

    public Task AddPlacementAsync(Placement placement) =>
        _placements.InsertOneAsync(placement);

    // Delete records
    public Task DeleteStudentAsync(string id) =>
        _students.DeleteOneAsync(s => s.Id == id);

    public Task DeleteCompanyAsync(string id) =>
        _companies.DeleteOneAsync(c => c.Id == id);

    public Task DeletePlacementAsync(string id) =>
        _placements.DeleteOneAsync(p => p.Id == id);

    public Task UpdateStudentStatusAsync(string studentId, string status) =>
        _students.UpdateOneAsync(s => s.Id == studentId, Builders<Student>.Update.Set(s => s.PlacementStatus, status));

    public async Task SeedSampleDataIfEmptyAsync()
    {
        var studentCount = await _students.CountDocumentsAsync(FilterDefinition<Student>.Empty);
        if (studentCount > 0) return;

        var sampleCompanies = new List<Company>
        {
            new() { Name = "Google", JobRole = "Software Development Engineer", PackageLPA = 32.5, Location = "Bangalore" },
            new() { Name = "Microsoft", JobRole = "Cloud Solutions Architect", PackageLPA = 28.0, Location = "Hyderabad" },
            new() { Name = "Amazon", JobRole = "Operations & Data Engineer", PackageLPA = 25.0, Location = "Pune" },
            new() { Name = "Deloitte", JobRole = "Technology Consultant", PackageLPA = 14.5, Location = "Mumbai" },
            new() { Name = "TCS Digital", JobRole = "Full Stack Developer", PackageLPA = 9.0, Location = "Noida" },
            new() { Name = "Infosys", JobRole = "Systems Specialist", PackageLPA = 8.5, Location = "Bangalore" }
        };
        await _companies.InsertManyAsync(sampleCompanies);

        var sampleStudents = new List<Student>
        {
            new() { Name = "Aarav Sharma", Email = "aarav.sharma@campus.edu", Department = "Computer Science", CGPA = 9.42, PlacementStatus = "Placed" },
            new() { Name = "Priya Patel", Email = "priya.patel@campus.edu", Department = "Information Technology", CGPA = 8.95, PlacementStatus = "Placed" },
            new() { Name = "Rohan Verma", Email = "rohan.verma@campus.edu", Department = "Data Science", CGPA = 9.10, PlacementStatus = "Placed" },
            new() { Name = "Ananya Iyer", Email = "ananya.iyer@campus.edu", Department = "Computer Science", CGPA = 8.64, PlacementStatus = "Placed" },
            new() { Name = "Kabir Mehta", Email = "kabir.mehta@campus.edu", Department = "Electronics", CGPA = 7.85, PlacementStatus = "Pending" },
            new() { Name = "Neha Joshi", Email = "neha.joshi@campus.edu", Department = "Information Technology", CGPA = 8.30, PlacementStatus = "Placed" },
            new() { Name = "Aditya Kulkarni", Email = "aditya.k@campus.edu", Department = "Computer Science", CGPA = 7.92, PlacementStatus = "Pending" },
            new() { Name = "Sneha Sen", Email = "sneha.sen@campus.edu", Department = "Data Science", CGPA = 8.75, PlacementStatus = "Placed" }
        };
        await _students.InsertManyAsync(sampleStudents);

        var samplePlacements = new List<Placement>
        {
            new() { StudentId = sampleStudents[0].Id ?? "", CompanyId = sampleCompanies[0].Id ?? "", PackageLPA = 32.5, Status = "Placed", PlacementDate = DateTime.UtcNow.AddDays(-12) },
            new() { StudentId = sampleStudents[1].Id ?? "", CompanyId = sampleCompanies[1].Id ?? "", PackageLPA = 28.0, Status = "Placed", PlacementDate = DateTime.UtcNow.AddDays(-10) },
            new() { StudentId = sampleStudents[2].Id ?? "", CompanyId = sampleCompanies[2].Id ?? "", PackageLPA = 25.0, Status = "Placed", PlacementDate = DateTime.UtcNow.AddDays(-8) },
            new() { StudentId = sampleStudents[3].Id ?? "", CompanyId = sampleCompanies[3].Id ?? "", PackageLPA = 14.5, Status = "Placed", PlacementDate = DateTime.UtcNow.AddDays(-5) },
            new() { StudentId = sampleStudents[5].Id ?? "", CompanyId = sampleCompanies[4].Id ?? "", PackageLPA = 9.0, Status = "Placed", PlacementDate = DateTime.UtcNow.AddDays(-3) },
            new() { StudentId = sampleStudents[7].Id ?? "", CompanyId = sampleCompanies[5].Id ?? "", PackageLPA = 8.5, Status = "Placed", PlacementDate = DateTime.UtcNow.AddDays(-1) }
        };
        await _placements.InsertManyAsync(samplePlacements);
    }
}