using StockSF2_Clientes;

var builder = WebApplication.CreateBuilder(args);

// llamo a la startup
// donde estan las configuraciones de
// servicios
// middlewares

var startup= new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app=builder.Build();

startup.Configure(app,app.Environment);

//

app.Run();
