using Shared.SharedKernel.Errors;

namespace DirectoryService.Domain.Shared;

public static class Errors
{
    public static class Location
    {
        public static Error NotFound()
        {
            return Error.Validation("record.not.found", $"Location not found");
        }

        public static Error AlreadyExist(string? invalidField = null)
        {
            var errorMassage = invalidField == null
                ? "Location already exist"
                : $"Location already exist with this property: {invalidField}";
            
            return Error.Validation("record.already.exist", errorMassage);
        }
    }
    
    public static class Department
    {
        public static Error NotFound()
        {
            return Error.Validation("record.not.found", "Department not found");
        }

        public static Error AlreadyExist(string? invalidField = null)
        {
            var errorMassage = invalidField == null 
                ? "Department already exist" 
                : $"Department already exist with this property: {invalidField}";
            
            return Error.Validation("record.already.exist", errorMassage);
        }
        
        public static Error NotActive()
        {
            return Error.Validation("record.not.active", "Department is not active");
        }

        public static Error HierarchyFailure()
        {
            return Error.Failure(
                "hierarchy.department.parent", 
                "Child department can not be parent department");
        }
    }
    
    public static class Position
    {
        public static Error NotFound()
        {
            return Error.Validation("record.not.found", $"Position not found");
        }

        public static Error AlreadyExist(string? invalidField = null)
        {
            var errorMassage = invalidField == null
                ? "Position already exist"
                : $"Position already exist with this property: {invalidField}";
            
            return Error.Validation("record.already.exist", errorMassage);
        }
    }
}