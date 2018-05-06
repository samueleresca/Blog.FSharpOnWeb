module DataAccess.Tests

open Blog.FSharpWebAPI
open Blog.FSharpWebAPI.Models
open DataAccess
open Fixtures
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.TestHost
open Microsoft.EntityFrameworkCore
open Microsoft.Extensions.DependencyInjection
open System
open System.IO
open System.Net
open System.Net.Http
open Xunit

let createInMemoryContext (databaseName : string) = 
    let builder = new DbContextOptionsBuilder<LabelsContext>()
    builder.UseInMemoryDatabase(databaseName).Options

let getTestLabel = 
    { Id = 1
      Code = "Test"
      Content = "Test content"
      IsoCode = "IT"
      Inactive = false }

[<Fact>]
let ``getAll should not return empty result``() = 
    //Arrange
    let context = new LabelsContext(createInMemoryContext "getAll_db")
    //Act
    getAll context //Assert
                   |> shouldNotNull

[<Fact>]
let ``getAll should return correct result``() = 
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
let ``getLabel should return correct result ``() = 
    //Arrange
    let context = new LabelsContext(createInMemoryContext "getLabel_db")
    getTestLabel
    |> context.Labels.Add
    |> ignore
    context.SaveChanges() |> ignore
    //Act
    let result = getLabel context 1
    //Assert
    Assert.Equal(result.Value.Id, 1)

[<Fact>]
let ``addLabelAsync should return inserted result ``() = 
    async { 
        //Arrange
        let context = new LabelsContext(createInMemoryContext "addLabelAsync_db")
        //Act
        let! result = addLabelAsync context getTestLabel
        //Assert
        result |> shouldNotNull
    }

[<Fact>]
let ``updateLabel should update correct record ``() = 
    //Arrange
    let context = new LabelsContext(createInMemoryContext "updateLabel_db")
    getTestLabel
    |> context.Labels.Add
    |> ignore
    context.SaveChanges() |> ignore
    //Act
    let result = 
        updateLabel context { Id = 1
                              Code = "Test"
                              Content = "Test content updated"
                              IsoCode = "IT"
                              Inactive = false } 1
    //Assert
    result.Value.Content |> shouldEqual "Test content updated"

[<Fact>]
let ``deleteLabel should delete correct record ``() = 
    //Arrange
    let context = new LabelsContext(createInMemoryContext "deleteLabel_db")
    getTestLabel
    |> context.Labels.Add
    |> ignore
    context.SaveChanges() |> ignore
    //Act
    let result = deleteLabel context 1
    //Assert
    Assert.Equal(result.Value.Id, 1)
