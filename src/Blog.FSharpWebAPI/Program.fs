module Blog.FSharpWebAPI.App

open Blog.FSharpWebAPI.DataAccess
open Blog.FSharpWebAPI.Handlers
open Blog.FSharpWebAPI.Models
open Giraffe
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.EntityFrameworkCore
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open System
open System.IO

// ---------------------------------
// Web app
// ---------------------------------
let indexHandler (name : string) = 
    let greetings = sprintf "Hello %s, from Giraffe!" name
    let model = { Text = greetings }
    json model

let webApp = 
    choose [ GET >=> choose [ route "/" >=> indexHandler "world"
                              route "/label" >=> labelsHandler
                              routef "/label/%i" labelHandler ]
             POST >=> route "/label" >=> labelAddHandler
             PUT >=> routef "/label/%i" labelUpdateHandler
             DELETE >=> routef "/label/%i" labelDeleteHandler
             setStatusCode 404 >=> text "Not Found" ]

// ---------------------------------
// Error handler
// ---------------------------------
let errorHandler (ex : Exception) (logger : ILogger) = 
    logger.LogError(EventId(), ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message

// ---------------------------------
// Config and Main
// ---------------------------------
let configureCors (builder : CorsPolicyBuilder) = 
    builder.WithOrigins("http://localhost:8080").AllowAnyMethod().AllowAnyHeader() |> ignore

let configureApp (app : IApplicationBuilder) = 
    let env = app.ApplicationServices.GetService<IHostingEnvironment>()
    (match env.IsDevelopment() with
     | true -> app.UseDeveloperExceptionPage()
     | false -> app.UseGiraffeErrorHandler errorHandler).UseCors(configureCors).UseStaticFiles().UseGiraffe(webApp)

let configureServices (services : IServiceCollection) = 
    services.AddDbContext<LabelsContext>
        (fun (options : DbContextOptionsBuilder) -> 
        options.UseSqlServer
            (@"Server=localhost;Database=ContentDataDB2;User Id=sa;Password=P@55w0rd;") 
        |> ignore) |> ignore
    services.AddCors() |> ignore
    services.AddGiraffe() |> ignore

let configureLogging (builder : ILoggingBuilder) = 
    let filter (l : LogLevel) = l.Equals LogLevel.Error
    builder.AddFilter(filter).AddConsole().AddDebug() |> ignore

[<EntryPoint>]
let main _ = 
    let contentRoot = Directory.GetCurrentDirectory()
    let webRoot = Path.Combine(contentRoot, "WebRoot")
    WebHostBuilder().UseKestrel().UseContentRoot(contentRoot).UseIISIntegration().UseWebRoot(webRoot)
        .Configure(Action<IApplicationBuilder> configureApp).ConfigureServices(configureServices)
        .ConfigureLogging(configureLogging).Build().Run()
    0
