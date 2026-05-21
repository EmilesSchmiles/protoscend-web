using System.ComponentModel.DataAnnotations;

namespace Protoscend.Services;

/// <summary>
/// Single definition of ContactFormModel — used by both
/// the Blazor frontend (EditForm) and referenced by EmailService.
/// DELETE the duplicate definition that was inside EmailService.cs
/// </summary>
public class ContactFormModel
{
    [Required(ErrorMessage = "First name is required")]
    public string FirstName { get; set; } = "";

    [Required(ErrorMessage = "Last name is required")]
    public string LastName { get; set; } = "";

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Enter a valid email address")]
    public string Email { get; set; } = "";

    public string Company { get; set; } = "";
    public string Service { get; set; } = "";

    [Required(ErrorMessage = "Message is required")]
    [MinLength(10, ErrorMessage = "Please tell us a bit more (10+ characters)")]
    public string Message { get; set; } = "";
}
