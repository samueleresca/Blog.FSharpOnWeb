module DataAccessTests

open Xunit
open Blog.FSharpWebAPI
open Blog.FSharpWebAPI.Models
open DataAccess
open Fixtures


[<Fact>]
let ``getAll should not return empty result``() = 
    //Arrange
    let context = initializeInMemoryContext "getAll_db"
    //Act
    getAll context //Assert
                   |> shouldNotNull

[<Fact>]
let ``getAll should return correct result``() = 
    //Arrange
    let context = initializeAndPopulateContext "getAll_db" getTestLabel
  
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
    let context = initializeAndPopulateContext "getLabel_db" getTestLabel
    //Act
    let result = getLabel context 1
    //Assert
    Assert.Equal(result.Value.Id, 1)

[<Fact>]
let ``addLabelAsync should return inserted result ``() = 
    async { 
        //Arrange
        let context = initializeInMemoryContext "addLabelAsync_db"
        //Act
        let! result = addLabelAsync context getTestLabel
        //Assert
        result |> shouldNotNull
    }

[<Fact>]
let ``updateLabel should update correct record ``() = 
    //Arrange
    let context = initializeAndPopulateContext "updateLabel_db" getTestLabel
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
    let context = initializeAndPopulateContext "deleteLabel_db" getTestLabel
    //Act
    let result = deleteLabel context 1
    //Assert
    Assert.Equal(result.Value.Id, 1)
