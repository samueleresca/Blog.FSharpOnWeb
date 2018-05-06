module DataAccess.Tests

open Blog.FSharpWebAPI
open Blog.FSharpWebAPI.Models
open Fixtures
open System
open System.Net
open System.Net.Http
open System.IO
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.TestHost
open Microsoft.Extensions.DependencyInjection
open Microsoft.EntityFrameworkCore;
open Xunit
open DataAccess
open Microsoft.EntityFrameworkCore

let createInMemoryContext (databaseName : string) =
    let builder = new DbContextOptionsBuilder<LabelsContext>()
    builder.UseInMemoryDatabase(databaseName).Options;
    
let getTestLabel = { Id = 1 ; Code = "Test"; Content = "Test content"; IsoCode = "IT"; Inactive = false}


[<Fact>]
let ``getAll should not return empty result`` () =
    //Arrange
    let context = new LabelsContext(createInMemoryContext "getAll_db")
    //Act
    getAll context
    //Assert
    |> shouldNotNull


[<Fact>]
let ``getAll should return correct result`` () =
    //Arrange
    let context = new LabelsContext(createInMemoryContext "getAll_db")
    
    getTestLabel 
    |> context.Labels.Add
    |> ignore
    
    context.SaveChanges() |> ignore
    //Act
    getAll context
    //Assert
    |> Seq.toList
    |> List.length 
    |> string
    |> shouldEqual "1"


[<Fact>]
let ``getLabel should return correct result `` () =
    //Arrange
    let context = new LabelsContext(createInMemoryContext "getLabel_db")
    
    getTestLabel 
    |> context.Labels.Add
    |> ignore
    
    context.SaveChanges() |> ignore
    //Act
    let result =getLabel context 1
    //Assert
    //result.IsSome
    //|> shouldEqual true
    
    result.Value.Id
    |> string
    |> shouldEqual "1"
    

