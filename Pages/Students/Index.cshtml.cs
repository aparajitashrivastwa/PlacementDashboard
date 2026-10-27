using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PlacementDashboard.Models;
using PlacementDashboard.Services;

namespace PlacementDashboard.Pages.Students;

public class IndexModel : PageModel
{
    private readonly MongoDbService _db;

    public IndexModel(MongoDbService db)
    {
        _db = db;
    }

    public List<Student> Students { get; set; } = [];

    [BindProperty]
    public Student NewStudent { get; set; } = new();

    [TempData]
    public string? SuccessMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        await _db.SeedSampleDataIfEmptyAsync();
        Students = await _db.GetStudentsAsync();
    }

    public async Task<IActionResult> OnPostAddAsync()
    {
        if (string.IsNullOrWhiteSpace(NewStudent.Name) || string.IsNullOrWhiteSpace(NewStudent.Email))
        {
            ErrorMessage = "Please provide both student name and email.";
            return RedirectToPage();
        }

        if (NewStudent.CGPA < 0 || NewStudent.CGPA > 10)
        {
            ErrorMessage = "CGPA must be between 0.0 and 10.0.";
            return RedirectToPage();
        }

        await _db.AddStudentAsync(NewStudent);
        SuccessMessage = $"Student '{NewStudent.Name}' successfully added to MongoDB!";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(string id)
    {
        if (!string.IsNullOrEmpty(id))
        {
            await _db.DeleteStudentAsync(id);
            SuccessMessage = "Student record deleted successfully.";
        }
        return RedirectToPage();
    }
}
