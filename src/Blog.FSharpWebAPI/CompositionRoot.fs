module CompostionRoot

open Blog.FSharpWebAPI.Repositories
open Blog.FSharpWebAPI.DataAccess
open Blog.FSharpWebAPI.Models

open Microsoft.EntityFrameworkCore

let configureSqlServerContext = 
    (fun () ->
        let optionsBuilder = new DbContextOptionsBuilder<LabelsContext>();
        optionsBuilder.UseSqlServer(@"Data Source=(localdb)\Projects;Initial Catalog=ContentDataDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False") |> ignore
        new LabelsContext(optionsBuilder.Options)
    )

let getContext = configureSqlServerContext()

let getAll  = LabelsRepository.getAll getContext
let getLabel  = LabelsRepository.getLabel getContext
let addLabelAsync = LabelsRepository.addLabelAsync getContext
let addLabel = LabelsRepository.addLabel getContext
let updateLabel = LabelsRepository.updateLabel getContext
