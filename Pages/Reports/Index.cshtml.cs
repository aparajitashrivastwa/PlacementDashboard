using Microsoft.AspNetCore.Mvc.RazorPages;
using PlacementDashboard.Models;
using PlacementDashboard.Services;

namespace PlacementDashboard.Pages.Reports;

public class IndexModel : PageModel
{
    private readonly MongoDbService _db;

    public IndexModel(MongoDbService db)
    {
        _db = db;
    }

    public long TotalStudents { get; set; }
    public long PlacedCount { get; set; }
    public long PendingCount { get; set; }
    public double PlacementPercentage { get; set; }
    public double HighestPackage { get; set; }
    public double AveragePackage { get; set; }

    public class DeptStat
    {
        public string Department { get; set; } = "";
        public int Total { get; set; }
        public int Placed { get; set; }
        public double Rate => Total == 0 ? 0 : Math.Round(Placed * 100.0 / Total, 1);
        public double AvgCgpa { get; set; }
    }

    public List<DeptStat> DeptStats { get; set; } = [];

    public class RecruiterStat
    {
        public string CompanyName { get; set; } = "";
        public string Location { get; set; } = "";
        public double PackageLPA { get; set; }
        public int OffersCount { get; set; }
    }

    public List<RecruiterStat> TopRecruiters { get; set; } = [];

    public async Task OnGetAsync()
    {
        await _db.SeedSampleDataIfEmptyAsync();

        var students = await _db.GetStudentsAsync();
        var companies = await _db.GetCompaniesAsync();
        var placements = await _db.GetPlacementsAsync();

        TotalStudents = students.Count;
        PlacedCount = students.Count(s => s.PlacementStatus.Equals("Placed", StringComparison.OrdinalIgnoreCase));
        PendingCount = TotalStudents - PlacedCount;

        PlacementPercentage = TotalStudents > 0
            ? Math.Round(PlacedCount * 100.0 / TotalStudents, 1)
            : 0;

        var placedRecords = placements.Where(p => p.Status.Equals("Placed", StringComparison.OrdinalIgnoreCase)).ToList();

        if (placedRecords.Count > 0)
        {
            HighestPackage = placedRecords.Max(p => p.PackageLPA);
            AveragePackage = Math.Round(placedRecords.Average(p => p.PackageLPA), 2);
        }
        else if (companies.Count > 0)
        {
            HighestPackage = companies.Max(c => c.PackageLPA);
            AveragePackage = Math.Round(companies.Average(c => c.PackageLPA), 2);
        }

        DeptStats = students
            .GroupBy(s => string.IsNullOrWhiteSpace(s.Department) ? "General" : s.Department)
            .Select(g => new DeptStat
            {
                Department = g.Key,
                Total = g.Count(),
                Placed = g.Count(s => s.PlacementStatus.Equals("Placed", StringComparison.OrdinalIgnoreCase)),
                AvgCgpa = Math.Round(g.Average(s => s.CGPA), 2)
            })
            .OrderByDescending(d => d.Rate)
            .ToList();

        var companyMap = companies.ToDictionary(c => c.Id ?? "", c => c);

        TopRecruiters = placements
            .GroupBy(p => p.CompanyId)
            .Select(g =>
            {
                companyMap.TryGetValue(g.Key, out var comp);
                return new RecruiterStat
                {
                    CompanyName = comp?.Name ?? "Recruiter (" + g.Key[..Math.Min(6, g.Key.Length)] + ")",
                    Location = comp?.Location ?? "Various",
                    PackageLPA = g.Max(p => p.PackageLPA),
                    OffersCount = g.Count()
                };
            })
            .OrderByDescending(r => r.OffersCount)
            .ThenByDescending(r => r.PackageLPA)
            .Take(10)
            .ToList();
    }
}
