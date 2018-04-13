module Blog.FSharpWebAPI.DataAccess
open Blog.FSharpWebAPI.Models
open Microsoft.EntityFrameworkCore


type LabelsContext =
    inherit DbContext
    
    new() = { inherit DbContext() }
    new(options: DbContextOptions<LabelsContext>) = { inherit DbContext(options) }
      
    
      override __.OnModelCreating modelBuilder = 
          let expr =  modelBuilder.Entity<Label>().HasKey(fun label -> (label.Id) :> obj) 
          modelBuilder.Entity<Label>().Property(fun label -> label.Id).ValueGeneratedOnAdd() |> ignore

    
      
    [<DefaultValue>]
    val mutable labels:DbSet<Label>
    member x.Labels 
        with get() = x.labels 
        and set v = x.labels <- v

