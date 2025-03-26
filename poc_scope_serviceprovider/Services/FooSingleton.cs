using poc_scope_serviceprovider.Contracts;

namespace poc_scope_serviceprovider.Services;

public class FooSingleton : IFooSingleton
{
    private string? _name;
    public string? GetName()
    {
        return _name;
    }

    public void SetName(string name)
    {
        _name = name;
    }
}