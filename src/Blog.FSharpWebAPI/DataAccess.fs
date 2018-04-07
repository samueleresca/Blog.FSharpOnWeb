module Blog.FSharpWebAPI.DataAccess
open Blog.FSharpWebAPI.Models
open System
open Microsoft.EntityFrameworkCore
open Microsoft.EntityFrameworkCore.Storage.Converters


type LabelsContext =
    inherit DbContext
    
    new() = { inherit DbContext() }
    new(options: DbContextOptions<LabelsContext>) = { inherit DbContext(options) }


    [<DefaultValue>]
    val mutable labels:DbSet<Label>
    member x.Labels 
        with get() = x.labels 
        and set v = x.labels <- v

