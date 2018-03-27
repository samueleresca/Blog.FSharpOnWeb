namespace EFCore.DataAccess
open System
open Microsoft.EntityFrameworkCore
open Microsoft.EntityFrameworkCore.Storage.Converters

type SerieContext =
    inherit DbContext
    
    new() = { inherit DbContext() }
    new(options: DbContextOptions<SerieContext>) = { inherit DbContext(options) }
