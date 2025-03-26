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
    
    using (var scope1 = provider.CreateScope())
    {
        IFooSingleton fooService1 = scope1.ServiceProvider.GetService<IFooSingleton>()!;
        IBarScoped barService1 = scope1.ServiceProvider.GetService<IBarScoped>()!;

        Console.WriteLine("New Scope 1");
        Console.WriteLine(fooService1.GetName());
        Console.WriteLine(barService1.GetName());
        Console.WriteLine("----------\n");
        
        fooService1.SetName("Foo 2");
        barService1.SetName("Bar 2");
        
        using (var scope2 = scope1.ServiceProvider.CreateScope())
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

        
        // IServiceCollection spCollection = provider.GetRequiredService<IServiceCollection>();
        // IServiceProvider newServiceProviderWithSingletonCopy = ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(spCollection, true);
        //
        // IFooSingleton fooService4 = newServiceProviderWithSingletonCopy.GetService<IFooSingleton>()!;
        // IBarScoped barService4 = newServiceProviderWithSingletonCopy.GetService<IBarScoped>()!;
        //
        // Console.WriteLine("New Service provider with copy of singletons");
        // Console.WriteLine(fooService4.GetName());
        // Console.WriteLine(barService4.GetName());
        // Console.WriteLine("----------\n");
        
    }
}).WithName("RunTest");

app.Run();