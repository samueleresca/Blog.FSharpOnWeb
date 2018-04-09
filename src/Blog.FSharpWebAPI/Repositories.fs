namespace Blog.FSharpWebAPI.Repositories
open Microsoft.EntityFrameworkCore
open Blog.FSharpWebAPI.Models
open Blog.FSharpWebAPI.DataAccess

module LabelsRepository =

    let getAll (context: LabelsContext) =
            context.Labels
        
    let getLabel (context: LabelsContext) id =
        context.Labels
        |> Seq.tryFind (fun f -> f.Id = id) 

    let addLabelAsync (context: LabelsContext) (entity: Label) =
        async {
            context.Labels.AddAsync(entity) |> Async.AwaitTask |> ignore
            let! _ = context.SaveChangesAsync true |> Async.AwaitTask
            return entity
        }   

    let addLabel (context: LabelsContext) (entity: Label) =
        context.Labels.Add(entity) |> ignore
        context.SaveChanges |> ignore

    let updateLabel (context: LabelsContext) (entity: Label) = 
        let currentEntry = context.Labels.Find(entity.Id)
        context.Entry(currentEntry).CurrentValues.SetValues(entity)
        context.SaveChanges |> ignore   

    let deleteLabel (context: LabelsContext) (entity: Label) = 
        context.Labels.Remove entity |> ignore
        context.SaveChanges |> ignore

