module HandlersTests

open Microsoft.EntityFrameworkCore
open Xunit
open Blog.FSharpWebAPI
open Blog.FSharpWebAPI.RequestModels
open DataAccess
open Fixtures
open Giraffe
open Giraffe.Serialization.Json
open Microsoft.AspNetCore.Http
open NSubstitute
open Newtonsoft.Json
open System.Threading.Tasks
open System.IO
open System.Text
open Blog.FSharpWebAPI.Models


let initializeInMemoryContext (databaseName : string) = 
    let builder = new DbContextOptionsBuilder<LabelsContext>()
    let context = new LabelsContext(builder.UseInMemoryDatabase(databaseName).Options)
    context

let populateContext (context : LabelsContext) (label : Label) = 
      label
          |> context.Labels.Add
          |> ignore
      context.SaveChanges() |> ignore

let initializeAndPopulateContext (databaseName:string) (label: Label) =  initializeInMemoryContext databaseName |> populateContext <| label

let next : HttpFunc = Some >> Task.FromResult

let getBody (ctx : HttpContext) =
    ctx.Response.Body.Position <- 0L
    use reader = new StreamReader(ctx.Response.Body, Encoding.UTF8)
    reader.ReadToEnd()
    
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
    initializeAndPopulateContext "getAll" getTestLabel;

    let context =  initializeInMemoryContext "getAll"
                    |> configureContext;
    
    context.Request.Method.ReturnsForAnyArgs "GET" |> ignore
    context.Request.Path.ReturnsForAnyArgs (PathString("/label")) |> ignore
                   
  
    task {
          let! result = App.webApp next context
          
          match result with
                  | None -> assertFailf "Result was expected to be %s" "[]"
                  | Some ctx ->
                      let body = getBody ctx
                      Assert.Contains("\"code\":\"Test\"", body)
                      Assert.Equal("application/json", ctx.Response |> getContentType)
          
        }
    

[<Fact>]
let ``/label/id should returns the correct response with correct id`` () =
    initializeAndPopulateContext "getById" getTestLabel;
    let context = initializeInMemoryContext "getById" |> configureContext;
          
    context.Request.Method.ReturnsForAnyArgs "GET" |> ignore
    context.Request.Path.ReturnsForAnyArgs (PathString("/label/1")) |> ignore
                   
  
    task {
          let! result = App.webApp next context
          match result with
                  | None -> assertFailf "Result was expected to be %s" "[]"
                  | Some ctx ->
                      let body = getBody ctx
                      Assert.Contains("\"id\":1", body)
                      Assert.Equal("application/json", ctx.Response |> getContentType)
          
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
                      let body = getBody ctx
                      Assert.Contains("Label not found", body)
                      Assert.Equal("application/json", ctx.Response |> getContentType)
 
          
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
                      let body = getBody ctx
                      Assert.Contains("\"code\":\"TestAdded\"", body)
                      Assert.Equal("application/json", ctx.Response |> getContentType)
          
        }  
        
[<Fact>]
let ``/label/ DELETE should delete the label label correctly`` () =
     initializeAndPopulateContext "delete" getTestLabel
     let context = initializeInMemoryContext "delete" |> configureContext
      
     context.Request.Method.ReturnsForAnyArgs "DELETE" |> ignore
     context.Request.Path.ReturnsForAnyArgs (PathString("/label/1")) |> ignore
   
     task {
           let! result =  App.webApp next context
           match result with
                   | None -> assertFailf "Result was expected to be %s" "[]"
                   | Some ctx ->
                       let body = getBody ctx
                       Assert.Contains("\"code\":\"Test\"", body)
                       Assert.Equal("application/json", ctx.Response |> getContentType)
           
         }   
        
[<Fact>]
let ``/label/id PUT should modify a label`` () =
    initializeAndPopulateContext "update" getTestLabel;
    
    let label = {   
                        Code = "TestUpdated"
                        IsoCode = "IT"
                        Content = "Test content"
                        Inactive = false
                    }
                    
    let postData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(label))
    let context = initializeInMemoryContext "update" |> configureContext
    
    context.Request.Method.ReturnsForAnyArgs "PUT" |> ignore
    context.Request.Path.ReturnsForAnyArgs (PathString("/label/1")) |> ignore
    context.Request.Body <- new MemoryStream(postData)

  
    task {
          let! result = App.webApp next context
          match result with
                  | None -> assertFailf "Result was expected to be %s" "[]"
                  | Some ctx ->
                      let body = getBody ctx
                      Assert.Contains("\"code\":\"TestUpdated\"", body)
                      Assert.Equal("application/json", ctx.Response |> getContentType)
          
        }  