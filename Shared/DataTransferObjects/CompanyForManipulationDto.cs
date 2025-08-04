

using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects
{
    public class CompanyForManipulationDto
    {
        [MaxLength(30, ErrorMessage = "Maximum length for the Name is 30 characters.")]
        public string? Name { get; init; }
        [MaxLength(100, ErrorMessage = "Maximum length for the Full Address is 100 characters.")]
        public string? FullAddress { get; init; }
    }
}
