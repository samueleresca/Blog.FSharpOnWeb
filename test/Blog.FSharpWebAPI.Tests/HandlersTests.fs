module HandlersTests

open Xunit
open Blog.FSharpWebAPI
open Blog.FSharpWebAPI.RequestModels
open Fixtures
open Giraffe
open Giraffe.Serialization.Json
open Microsoft.AspNetCore.Http
open NSubstitute
open Newtonsoft.Json
open Blog.FSharpWebAPI.DataAccess
open System.IO
open System.Text
    
let assertFailf format args =
    let msg = sprintf format args
    Assert.True(false, msg)
    
let getContentType (response : HttpResponse) =
    response.Headers.["Content-Type"].[0]
    
let configureContext (dbContext : LabelsContext) = 
        let context = Substitute.For<HttpContext>();
        context.RequestServices.GetService(typeof<LabelsContext>).Returns(dbContext) |> ignore
        context.RequestServices.GetService(typeof<IJsonSerializer>).Returns(NewtonsoftJsonSerializer(NewtonsoftJsonSerializer.DefaultSettings)) |> ignore
        context.Response.Body <- new MemoryStream()
        context.Request.Headers.ReturnsForAnyArgs(new HeaderDictionary()) |> ignore
        context

[<Fact>]
let ``/label should returns the correct response`` () =

    let context =  initializeAndPopulateContext "getAll" getTestLabel
                    |> configureContext;
    
    context.Request.Method.ReturnsForAnyArgs "GET" |> ignore
    context.Request.Path.ReturnsForAnyArgs (PathString("/label")) |> ignore
                   
  
    task {
          let! result = App.webApp next context
          
          match result with
                  | None -> assertFailf "Result was expected to be %s" "[]"
                  | Some ctx ->
                       getBody ctx
                               |> shouldContains "\"code\":\"Test\""
                     
          
        }
    

[<Fact>]
let ``/label/id should returns the correct response with correct id`` () =
   
    let context = initializeAndPopulateContext "getById" getTestLabel
                    |> configureContext;
          
    context.Request.Method.ReturnsForAnyArgs "GET" |> ignore
    context.Request.Path.ReturnsForAnyArgs (PathString("/label/1")) |> ignore
                   
  
    task {
          let! result = App.webApp next context
          match result with
                  | None -> assertFailf "Result was expected to be %s" "[]"
                  | Some ctx ->
                             getBody ctx
                                  |> shouldContains "\"id\":1"
        }
    
[<Fact>]
let ``/label/id should returns not found when id does not exists`` () =
    let context =  initializeInMemoryContext "getById" |> configureContext;
    
    context.Request.Method.ReturnsForAnyArgs "GET" |> ignore
    context.Request.Path.ReturnsForAnyArgs (PathString("/label/999")) |> ignore
  
    task {
          let! result = App.webApp next context
          match result with
                  | None -> assertFailf "Result was expected to be %s" "[]"
                  | Some ctx ->
                         getBody ctx
                                  |> shouldContains "Label not found"
          
        }  
        
[<Fact>]
let ``/label/ POST should add a new label`` () =

    let label = {   
                    Code = "TestAdded"
                    IsoCode = "IT"
                    Content = "Test content"
                    Inactive = false
                }
                
    let postData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(label))
    let context = initializeInMemoryContext "add" |> configureContext
    
    context.Request.Method.ReturnsForAnyArgs "POST" |> ignore
    context.Request.Path.ReturnsForAnyArgs (PathString("/label")) |> ignore
    context.Request.Body <- new MemoryStream(postData)

  
    task {
          let! result = App.webApp next context
          match result with
                  | None -> assertFailf "Result was expected to be %s" "[]"
                  | Some ctx ->
                      getBody ctx
                            |> shouldContains "\"code\":\"TestAdded\""
          
        }  
        
[<Fact>]
let ``/label/ DELETE should delete the label label correctly`` () =
     let context = initializeAndPopulateContext "delete" getTestLabel
                    |> configureContext
      
     context.Request.Method.ReturnsForAnyArgs "DELETE" |> ignore
     context.Request.Path.ReturnsForAnyArgs (PathString("/label/1")) |> ignore
   
     task {
           let! result =  App.webApp next context
           match result with
                   | None -> assertFailf "Result was expected to be %s" "[]"
                   | Some ctx ->
                       getBody ctx
                           |> shouldContains "\"code\":\"Test\""
           
         }   
        
[<Fact>]
let ``/label/id PUT should modify a label`` () =
    let label = {   
                        Code = "TestUpdated"
                        IsoCode = "IT"
                        Content = "Test content"
                        Inactive = false
                    }
                    
    let postData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(label))
    let context = initializeAndPopulateContext "update" getTestLabel |> configureContext
    
    context.Request.Method.ReturnsForAnyArgs "PUT" |> ignore
    context.Request.Path.ReturnsForAnyArgs (PathString("/label/1")) |> ignore
    context.Request.Body <- new MemoryStream(postData)

  
    task {
          let! result = App.webApp next context
          match result with
                  | None -> assertFailf "Result was expected to be %s" "[]"
                  | Some ctx ->
                      getBody ctx
                         |> shouldContains "\"code\":\"TestUpdated\""
          
        }  