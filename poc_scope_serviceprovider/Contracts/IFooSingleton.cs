namespace poc_scope_serviceprovider.Contracts;

public interface IFooSingleton
{
    string? GetName();
    void SetName(string name);
}