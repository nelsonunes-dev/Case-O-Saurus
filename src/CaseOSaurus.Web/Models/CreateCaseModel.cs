using System.ComponentModel.DataAnnotations;

namespace CaseOSaurus.Web.Models;

public class CreateCaseModel
{
    [Required(ErrorMessage = "Title is required")]
    [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required(ErrorMessage = "Priority is required")]
    public string Priority { get; set; } = string.Empty;

    [Required(ErrorMessage = "Case Type is required")]
    public string Type { get; set; } = string.Empty;

    [Required(ErrorMessage = "Requestor email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Requestor { get; set; } = string.Empty;

    public string? AssignedTo { get; set; }
}
