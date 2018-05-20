module Client.Tests

open System
open System.Net
open System.Net.Http
open System.Text
open System.IO
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.TestHost
open Microsoft.Extensions.DependencyInjection
open Xunit
open Newtonsoft.Json
open Fixtures
open Blog.FSharpWebAPI.RequestModels


[<Fact>]
let ``GET /label should respond empty`` () =
    use server = new TestServer(createHost())
    use client = server.CreateClient()

    get client "label"
    |> ensureSuccess
    |> readText
    |> shouldEqual "[]"


[<Fact>]
let ``/label/ POST should add a new label`` () =
    use server = new TestServer(createHost())
    use client = server.CreateClient()
    let content = new StringContent(JsonConvert.SerializeObject(getTestLabel), Encoding.UTF8, "application/json");
    
    
    post client "label" content
    |> ensureSuccess
    |> readText
    |> shouldContains "\"code\":\"Test\""
   
    
[<Fact>]
let ``/label/id PUT should modify a label`` () =
    use server = new TestServer(createHost())
    use client = server.CreateClient()
    let content = new StringContent(JsonConvert.SerializeObject(getTestLabel), Encoding.UTF8, "application/json");
    post client "label" content |> ignore
    
    let insertedLalbel = get client "label" |> ensureSuccess |> readText
    Console.WriteLine(insertedLalbel)
    let label = {   
                   Code = "TestUpdated"
                   IsoCode = "IT"
                   Content = "Test content"
                   Inactive = false
                        }
    let content = new StringContent(JsonConvert.SerializeObject(label), Encoding.UTF8, "application/json");
    
    
    put client "label/1" content
    |> ensureSuccess
    |> readText
    |> shouldContains "\"code\":\"TestUpdated\""


