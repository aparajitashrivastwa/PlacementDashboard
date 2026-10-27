using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PlacementDashboard.Models;
using PlacementDashboard.Services;

namespace PlacementDashboard.Pages.Companies;

public class IndexModel : PageModel
{
    private readonly MongoDbService _db;

    public IndexModel(MongoDbService db)
    {
        _db = db;
    }

    public List<Company> Companies { get; set; } = [];

    [BindProperty]
    public Company NewCompany { get; set; } = new();

    [TempData]
    public string? SuccessMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        await _db.SeedSampleDataIfEmptyAsync();
        Companies = await _db.GetCompaniesAsync();
    }

    public async Task<IActionResult> OnPostAddAsync()
    {
        if (string.IsNullOrWhiteSpace(NewCompany.Name))
        {
            ErrorMessage = "Company name is required.";
            return RedirectToPage();
        }

        await _db.AddCompanyAsync(NewCompany);
        SuccessMessage = $"Company '{NewCompany.Name}' registered successfully!";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(string id)
    {
        if (!string.IsNullOrEmpty(id))
        {
            await _db.DeleteCompanyAsync(id);
            SuccessMessage = "Company record removed.";
        }
        return RedirectToPage();
    }
}
