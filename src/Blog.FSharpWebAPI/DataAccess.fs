module Blog.FSharpWebAPI.DataAccess
open Blog.FSharpWebAPI.Models
open System
open Microsoft.EntityFrameworkCore
open Microsoft.EntityFrameworkCore.Storage.Converters


type LabelsContext =
    inherit DbContext
    
    new() = { inherit DbContext() }
    new(options: DbContextOptions<LabelsContext>) = { inherit DbContext(options) }
      
    
    //override __.OnModelCreating modelBuilder = 
//         modelBuilder.Entity<Label>().HasKey(fun label -> (label.Id) :> obj) |> ignore
//         modelBuilder.Entity<Label>().Property(fun label -> (label.Code)).IsRequired() |> ignore
    
      
    [<DefaultValue>]
    val mutable labels:DbSet<Label>
    member x.Labels 
        with get() = x.labels 
        and set v = x.labels <- v

