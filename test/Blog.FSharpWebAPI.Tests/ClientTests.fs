module ClientTests

open System.Net.Http
open System.Text
open Microsoft.AspNetCore.TestHost
open Xunit
open Newtonsoft.Json
open Fixtures
open HttpFunc
open Blog.FSharpWebAPI.RequestModels
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open System
open System.IO

let createHost() =
    WebHostBuilder()
        .UseContentRoot(Directory.GetCurrentDirectory()) 
        .UseEnvironment("Test")
        .Configure(Action<IApplicationBuilder> Blog.FSharpWebAPI.App.configureApp)
        .ConfigureServices(Action<IServiceCollection> Blog.FSharpWebAPI.App.configureServices)


[<Fact>]
let ``GET /label should respond empty`` () =
    use server = new TestServer(createHost()) 
    use client = server.CreateClient()

    get client "label"
    |> ensureSuccess
    |> readText
    |> shouldEqual "[]"

[<Fact>]
let ``POST /label/ should add a new label`` () =
    use server =  new TestServer(createHost()) 
    use client = server.CreateClient()
    use content = new StringContent(JsonConvert.SerializeObject(getTestLabel), Encoding.UTF8, "application/json");
    
    
    post client "label" content
    |> ensureSuccess
    |> readText
    |> shouldContains "\"code\":\"Test\""
