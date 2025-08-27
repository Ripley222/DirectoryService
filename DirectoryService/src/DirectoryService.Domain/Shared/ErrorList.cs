using System.Collections;

namespace DirectoryService.Domain.Shared;

public class ErrorList(IEnumerable<Error> errorsList) : IEnumerable<Error>
{
    private readonly List<Error> _errorsList = errorsList.ToList();

    public IEnumerator<Error> GetEnumerator()
    {
        return _errorsList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public static implicit operator ErrorList(List<Error> errorsList)
        => new(errorsList);

    public static implicit operator ErrorList(Error error)
        => new([error]);
}