using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BattleShip.App;
using Blazored.LocalStorage;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddScoped(sp =>
{
    var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
    var channel = GrpcChannel.ForAddress("https://localhost:5002", new GrpcChannelOptions { HttpClient = httpClient });
    return new BattleShipService.BattleShipServiceClient(channel);
});
builder.Services.AddScoped<BattleShipServiceImpl>();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:5002") });

builder.Services.AddBlazoredLocalStorage();


await builder.Build().RunAsync();
