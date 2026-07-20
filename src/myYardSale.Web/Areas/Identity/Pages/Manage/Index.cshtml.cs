using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using myYardSale.Application.Services;
using myYardSale.Domain.Entities;

namespace myYardSale.Web.Areas.Identity.Pages.Manage;

[Authorize]
public class IndexModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ListingService _listingService;

    public IndexModel(UserManager<ApplicationUser> userManager, ListingService listingService)
    {
        _userManager = userManager;
        _listingService = listingService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PickupNotes { get; set; }
        public string? ContactPhone { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactNotes { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return NotFound();
        }

        Input = new InputModel
        {
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            PickupNotes = user.PickupNotes,
            ContactPhone = user.ContactPhone,
            ContactEmail = user.ContactEmail,
            ContactNotes = user.ContactNotes
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return NotFound();
        }

        user.FullName = Input.FullName ?? string.Empty;
        user.PhoneNumber = Input.PhoneNumber ?? string.Empty;
        user.PickupNotes = Input.PickupNotes ?? string.Empty;
        user.ContactPhone = Input.ContactPhone ?? string.Empty;
        user.ContactEmail = Input.ContactEmail ?? string.Empty;
        user.ContactNotes = Input.ContactNotes ?? string.Empty;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }

        TempData["Success"] = "Profile updated successfully.";
        return RedirectToPage();
    }
}