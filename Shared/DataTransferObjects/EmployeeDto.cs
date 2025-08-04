using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects
{
    public record EmployeeDto(Guid Id, string Name, int Age, string Position);
    public record EmployeeForCreationDto :EmployeeForManipulationDto;

    public record EmployeeForUpdateDto : EmployeeForManipulationDto;


}
