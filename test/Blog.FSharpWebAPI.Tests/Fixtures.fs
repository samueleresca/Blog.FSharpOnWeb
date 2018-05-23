module Fixtures

open Blog.FSharpWebAPI.Models
open Xunit
open Microsoft.EntityFrameworkCore
open Blog.FSharpWebAPI.DataAccess
open Giraffe
open System.Threading.Tasks
open Microsoft.AspNetCore.Http
open System.IO


let getTestLabel = 
    { Id = 1
      Code = "Test"
      Content = "Test content"
      IsoCode = "IT"
      Inactive = false }

      
let initializeInMemoryContext (databaseName : string) = 
    let builder = new DbContextOptionsBuilder<LabelsContext>()
    let context = new LabelsContext(builder.UseInMemoryDatabase(databaseName).Options)
    context

let populateContext (context : LabelsContext) (label : Label) = 
      label
          |> context.Labels.Add
          |> ignore
      context.SaveChanges() |> ignore
      context

let initializeAndPopulateContext (databaseName:string) (label: Label) =  initializeInMemoryContext databaseName |> populateContext <| label

let next : HttpFunc = Some >> Task.FromResult

let getBody (ctx : HttpContext) =
    ctx.Response.Body.Position <- 0L
    use reader = new StreamReader(ctx.Response.Body, System.Text.Encoding.UTF8)
    reader.ReadToEnd()

let shouldContains actual expected = Assert.Contains(actual, expected) 
let shouldEqual expected actual = Assert.Equal(expected, actual)
let shouldNotNull expected = Assert.NotNull(expected)
