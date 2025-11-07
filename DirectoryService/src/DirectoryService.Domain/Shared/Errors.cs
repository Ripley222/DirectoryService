namespace DirectoryService.Domain.Shared;

public static class Errors
{
    public static class General
    {
        public static Error ValueIsInvalid(string? name = null)
        {
            var label = name == null ? "value " : $"{name} ";
            return Error.Validation("value.is.invalid", $"{label}is invalid", name);
        } 
          
        public static Error NotFound(Guid? id = null)
        {
            var forId = id == null ? "" : $" for id '{id}'";
            return Error.Validation("record.not.found", $"record not found{forId}");
        } 
          
        public static Error ValueIsRequired(string? name = null)
        {
            var label = name == null ? "value " : $"{name} ";
            return Error.Validation("value.is.required", $"{label}is required", name);
        }
    }

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