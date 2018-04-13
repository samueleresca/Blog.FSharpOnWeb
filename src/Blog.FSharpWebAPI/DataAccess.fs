module Blog.FSharpWebAPI.DataAccess
open Blog.FSharpWebAPI.Models
open Microsoft.EntityFrameworkCore

type LabelsContext(options: DbContextOptions<LabelsContext>) =
    inherit DbContext(options)
    
      override __.OnModelCreating modelBuilder = 
          let expr =  modelBuilder.Entity<Label>().HasKey(fun label -> (label.Id) :> obj) 
          modelBuilder.Entity<Label>().Property(fun label -> label.Id).ValueGeneratedOnAdd() |> ignore
      
    [<DefaultValue>]
    val mutable labels:DbSet<Label>
    member x.Labels 
        with get() = x.labels 
        and set v = x.labels <- v

module LabelsRepository = 

    let getAll (context : LabelsContext) = context.Labels
    let getLabel (context : LabelsContext) id = context.Labels |> Seq.tryFind (fun f -> f.Id = id)
    
    let addLabelAsync (context : LabelsContext) (entity : Label) = 
        async { 
            context.Labels.AddAsync(entity)
            |> Async.AwaitTask
            |> ignore

            let! result = context.SaveChangesAsync true |> Async.AwaitTask
            let result = if result >= 1  then Some(entity) else None
            return result
        }
    
    let updateLabel (context : LabelsContext) (entity : Label) (id : int) = 
        let current = context.Labels.Find(id)
        let updated = { entity with Id = id }
        context.Entry(current).CurrentValues.SetValues(updated)
        if context.SaveChanges true >= 1  then Some(updated) else None
    
    let deleteLabel (context : LabelsContext) (id : int) = 
        let current = context.Labels.Find(id)
        let deleted = { current with Inactive = true }
        updateLabel context deleted id

let getAll  = LabelsRepository.getAll 
let getLabel  = LabelsRepository.getLabel 
let addLabelAsync = LabelsRepository.addLabelAsync 
let updateLabel = LabelsRepository.updateLabel 
let deleteLabel = LabelsRepository.deleteLabel 