namespace poc_scope_serviceprovider.Contracts;

public interface IBarScoped
{
    string? GetName();
    void SetName(string name);
}