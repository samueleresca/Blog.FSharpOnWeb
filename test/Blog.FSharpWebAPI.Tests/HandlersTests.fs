module Handlers.Tests

open Microsoft.EntityFrameworkCore
open Xunit
open Blog.FSharpWebAPI
open Blog.FSharpWebAPI
open Blog.FSharpWebAPI.Models
open Blog.FSharpWebAPI.RequestModels
open DataAccess
open Handlers
open Fixtures
open Giraffe
open Giraffe.Serialization.Json
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.TestHost
open Microsoft.AspNetCore.Http
open NSubstitute
open Newtonsoft.Json
open System.Threading.Tasks
open System.IO
open System.Text


let createInMemoryContext (databaseName : string) = 
    let builder = new DbContextOptionsBuilder<LabelsContext>()
    new LabelsContext(builder.UseInMemoryDatabase(databaseName).Options)

      
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
    use dbContext = createInMemoryContext "getAll";
    let context = configureContext dbContext;
    
    context.Request.Method.ReturnsForAnyArgs "GET" |> ignore
    context.Request.Path.ReturnsForAnyArgs (PathString("/label")) |> ignore
                   
  
    task {
          let! result = App.webApp next context
          
          match result with
                  | None -> assertFailf "Result was expected to be %s" "[]"
                  | Some ctx ->
                      let body = getBody ctx
                      Assert.Equal("[]", body)
                      Assert.Equal("application/json", ctx.Response |> getContentType)
          
        }
    

[<Fact>]
let ``/label/id should returns the correct response with correct id`` () =
    use dbContext = createInMemoryContext "getById";
     
    getTestLabel
          |> dbContext.Labels.Add
          |> ignore
    dbContext.SaveChanges() |> ignore
    
    let context = configureContext dbContext;
          
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
    use dbContext= createInMemoryContext "getById";
    let context = configureContext dbContext;
    
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
    use dbContext = createInMemoryContext "add";
    let label = {   
                    Code = "Test"
                    IsoCode = "IT"
                    Content = "Test content"
                    Inactive = false
                }
                
    let postData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(label))
    let context = configureContext dbContext
    
    context.Request.Method.ReturnsForAnyArgs "POST" |> ignore
    context.Request.Path.ReturnsForAnyArgs (PathString("/label")) |> ignore
    context.Request.Body <- new MemoryStream(postData)

  
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
let ``/label/id PUT should modify a label`` () =
    use dbContext= createInMemoryContext "update";
    getTestLabel
      |> dbContext.Labels.Add
      |> ignore
    dbContext.SaveChanges() |> ignore
    
    let label = {   
                        Code = "TestUpdated"
                        IsoCode = "IT"
                        Content = "Test content"
                        Inactive = false
                    }
                    
    let postData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(label))
    let context = configureContext dbContext
    
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

[<Fact>]
let ``/label/ DELETE should delete the label label correctly`` () =
    use dbContext= createInMemoryContext "delete";
   
    getTestLabel
      |> dbContext.Labels.Add
      |> ignore
      
    dbContext.SaveChanges() |> ignore
    
    let context = configureContext dbContext
     
    context.Request.Method.ReturnsForAnyArgs "DELETE" |> ignore
    context.Request.Path.ReturnsForAnyArgs (PathString("/label/1")) |> ignore

  
    task {
          let! result =  App.webApp next context
          match result with
                  | Some ctx ->
                      let body = getBody ctx
                      Assert.Contains("\"code\":\"Test\"", body)
                      Assert.Equal("application/json", ctx.Response |> getContentType)
          
        }   