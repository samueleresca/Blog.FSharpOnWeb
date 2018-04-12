namespace Blog.FSharpWebAPI.Repositories

open Blog.FSharpWebAPI.DataAccess
open Blog.FSharpWebAPI.Models
open Microsoft.EntityFrameworkCore

module LabelsRepository = 
    let getAll (context : LabelsContext) = context.Labels
    let getLabel (context : LabelsContext) id = context.Labels |> Seq.tryFind (fun f -> f.Id = id)
    
    let addLabelAsync (context : LabelsContext) (entity : Label) = 
        async { 
            context.Labels.AddAsync(entity)
            |> Async.AwaitTask
            |> ignore
            
            let! result = context.SaveChangesAsync true |> Async.AwaitTask
            let result = if result > 1  then Some(entity) else None
            return result
        }
    
    let addLabel (context : LabelsContext) (entity : Label) = 
        context.Labels.Add(entity) |> ignore
        context.SaveChanges true |> ignore
    
    let updateLabel (context : LabelsContext) (entity : Label) = 
        let currentEntry = context.Labels.Find(entity.Id)
        context.Entry(currentEntry).CurrentValues.SetValues(entity)
        if context.SaveChanges true > 1  then Some(currentEntry) else None
  