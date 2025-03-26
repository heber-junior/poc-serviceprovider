using poc_scope_serviceprovider.Contracts;
using poc_scope_serviceprovider.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IFooSingleton, FooSingleton>();
builder.Services.AddScoped<IBarScoped, BarScoped>();
builder.Services.AddSingleton<IServiceCollection>(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
                                                                                                                                                                                                                        
app.MapGet("/test", (HttpContext httpContext) =>
{
    IServiceProvider provider = httpContext.RequestServices;
    IFooSingleton foo = provider.GetService<IFooSingleton>()!;
    IBarScoped bar = provider.GetService<IBarScoped>()!;

    foo.SetName("Foo 1");
    bar.SetName("Bar 1");

    Console.WriteLine("Http Scope");
    Console.WriteLine(foo.GetName());
    Console.WriteLine(bar.GetName());
    Console.WriteLine("----------");
    
    using (var scope = provider.CreateScope())
    {
        IFooSingleton fooService = scope.ServiceProvider.GetService<IFooSingleton>()!;
        IBarScoped barService = scope.ServiceProvider.GetService<IBarScoped>()!;

        Console.WriteLine("New Scope");
        Console.WriteLine(fooService.GetName());
        Console.WriteLine(barService.GetName());
        
        IServiceCollection newServiceCollection = new ServiceCollection();
        foreach (ServiceDescriptor serviceDescriptor in provider.GetRequiredService<IServiceCollection>().AsEnumerable())
            newServiceCollection.Add(serviceDescriptor);
        IServiceProvider newServiceProvider = newServiceCollection.BuildServiceProvider();
        
        IFooSingleton fooService2 = newServiceProvider.GetService<IFooSingleton>()!;
        IBarScoped barService2 = newServiceProvider.GetService<IBarScoped>()!;
        
        Console.WriteLine("New Service provider");
        Console.WriteLine(fooService2.GetName());
        Console.WriteLine(barService2.GetName());
    }
}).WithName("RunTest");

app.Run();