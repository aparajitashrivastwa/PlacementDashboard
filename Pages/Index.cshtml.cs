using Microsoft.AspNetCore.Mvc.RazorPages;
using PlacementDashboard.Models;
using PlacementDashboard.Services;

namespace PlacementDashboard.Pages;

public class IndexModel : PageModel
{
    private readonly MongoDbService _db;

    public IndexModel(MongoDbService db)
    {
        _db = db;
    }

    public long TotalStudents { get; private set; }
    public long PlacedStudents { get; private set; }
    public long TotalCompanies { get; private set; }

    public double PlacementRate =>
        TotalStudents == 0
            ? 0
            : Math.Round(PlacedStudents * 100.0 / TotalStudents, 1);

    public List<Student> Students { get; private set; } = [];

    public async Task OnGetAsync()
    {
        await _db.SeedSampleDataIfEmptyAsync();
        TotalStudents = await _db.GetTotalAsync();
        PlacedStudents = await _db.GetPlacedAsync();
        TotalCompanies = await _db.GetTotalCompaniesAsync();
        Students = await _db.GetStudentsAsync();
    }
}