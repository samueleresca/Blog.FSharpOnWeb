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
let ``/label/id PUT should modify a label`` () =
    use server = new TestServer(createHost()) 
    use client =  server.CreateClient()
    use content = new StringContent(JsonConvert.SerializeObject(getTestLabel), Encoding.UTF8, "application/json")
    
    post client "label" content |> ignore
 
    let label = {   
                   Code = "TestUpdated"
                   IsoCode = "IT"
                   Content = "Test content"
                   Inactive = false
                        }
    use content = new StringContent(JsonConvert.SerializeObject(label), Encoding.UTF8, "application/json");
    
    put client "label/1" content
    |> ensureSuccess
    |> readText
    |> shouldContains "\"code\":\"TestUpdated\""

[<Fact>]
let ``/label/ POST should add a new label`` () =
    use server =  new TestServer(createHost()) 
    use client = server.CreateClient()
    use content = new StringContent(JsonConvert.SerializeObject(getTestLabel), Encoding.UTF8, "application/json");
    
    
    post client "label" content
    |> ensureSuccess
    |> readText
    |> shouldContains "\"code\":\"Test\""
   
       
[<Fact>]
let ``DELETE /label/id should modify a label`` () =
    use server = new TestServer(createHost()) 
    use client = server.CreateClient()
    use content = new StringContent(JsonConvert.SerializeObject(getTestLabel), Encoding.UTF8, "application/json")
    
    post client "label" content |> ignore
    delete client "label/1"
    |> ensureSuccess
    |> readText
    |> shouldContains "\"inactive\":true"


