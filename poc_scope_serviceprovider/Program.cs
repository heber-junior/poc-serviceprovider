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

    foo.SetName("Foo 1 - HttpContext");
    bar.SetName("Bar 1 - HttpContext");

    Console.WriteLine("Http Scope");
    Console.WriteLine(foo.GetName());
    Console.WriteLine(bar.GetName());
    Console.WriteLine("----------\n");
    
    //*Note: CreateScope() register the following interfaces automatically.
    // IServiceProvider
    // IServiceScopeFactory
    using (var scope1 = provider.CreateScope())
    {
        IFooSingleton fooService1 = scope1.ServiceProvider.GetService<IFooSingleton>()!;
        IBarScoped barService1 = scope1.ServiceProvider.GetService<IBarScoped>()!;
        // IServiceProvider scope1sp = scope1.ServiceProvider.GetService<IServiceProvider>()!;

        Console.WriteLine("New Scope 1");
        Console.WriteLine(fooService1.GetName());
        Console.WriteLine(barService1.GetName());
        // Console.WriteLine($"SPs are equals {scope1sp.Equals(scope1.ServiceProvider)}");
        // Console.WriteLine($"Provider and Scope1 are  equals {provider.Equals(scope1sp)}");
        Console.WriteLine("----------\n");
        
        fooService1.SetName("Foo 2");
        barService1.SetName("Bar 2");

        var scope1Factory = scope1.ServiceProvider.GetService<IServiceScopeFactory>()!;
        // using (var scope2 = scope1.ServiceProvider.CreateScope())
        using (var scope2 = scope1Factory.CreateScope())
        {
            IFooSingleton fooService2 = scope2.ServiceProvider.GetService<IFooSingleton>()!;
            IBarScoped barService2 = scope2.ServiceProvider.GetService<IBarScoped>()!;
            
            Console.WriteLine("New Scope 2");
            Console.WriteLine(fooService2.GetName());
            Console.WriteLine(barService2.GetName());
        }
        
        IServiceCollection newServiceCollection = new ServiceCollection();
        foreach (ServiceDescriptor serviceDescriptor in provider.GetRequiredService<IServiceCollection>().AsEnumerable())
            newServiceCollection.Add(serviceDescriptor);
        IServiceProvider newServiceProvider = newServiceCollection.BuildServiceProvider();
        
        IFooSingleton fooService3 = newServiceProvider.GetService<IFooSingleton>()!;
        IBarScoped barService3 = newServiceProvider.GetService<IBarScoped>()!;
        
        Console.WriteLine("New Service provider");
        Console.WriteLine(fooService3.GetName());
        Console.WriteLine(barService3.GetName());
        Console.WriteLine("----------\n");
    }
}).WithName("RunTest");

app.Run();