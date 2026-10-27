using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PlacementDashboard.Models;
using PlacementDashboard.Services;

namespace PlacementDashboard.Pages.Placements;

public class IndexModel : PageModel
{
    private readonly MongoDbService _db;

    public IndexModel(MongoDbService db)
    {
        _db = db;
    }

    public List<Placement> Placements { get; set; } = [];
    public Dictionary<string, Student> StudentsMap { get; set; } = [];
    public Dictionary<string, Company> CompaniesMap { get; set; } = [];

    public List<Student> AllStudents { get; set; } = [];
    public List<Company> AllCompanies { get; set; } = [];

    [BindProperty]
    public Placement NewPlacement { get; set; } = new();

    [TempData]
    public string? SuccessMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        await _db.SeedSampleDataIfEmptyAsync();
        Placements = await _db.GetPlacementsAsync();
        AllStudents = await _db.GetStudentsAsync();
        AllCompanies = await _db.GetCompaniesAsync();

        StudentsMap = AllStudents
            .Where(s => !string.IsNullOrEmpty(s.Id))
            .ToDictionary(s => s.Id!, s => s);

        CompaniesMap = AllCompanies
            .Where(c => !string.IsNullOrEmpty(c.Id))
            .ToDictionary(c => c.Id!, c => c);
    }

    public async Task<IActionResult> OnPostAddAsync()
    {
        if (string.IsNullOrWhiteSpace(NewPlacement.StudentId) || string.IsNullOrWhiteSpace(NewPlacement.CompanyId))
        {
            ErrorMessage = "Please select both a student and a company.";
            return RedirectToPage();
        }

        NewPlacement.PlacementDate = DateTime.UtcNow;
        await _db.AddPlacementAsync(NewPlacement);

        if (NewPlacement.Status == "Placed")
        {
            await _db.UpdateStudentStatusAsync(NewPlacement.StudentId, "Placed");
        }

        SuccessMessage = "Placement record registered successfully!";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(string id)
    {
        if (!string.IsNullOrEmpty(id))
        {
            await _db.DeletePlacementAsync(id);
            SuccessMessage = "Placement record deleted.";
        }
        return RedirectToPage();
    }
}
