module Blog.FSharpWebAPI.Handlers
open Giraffe;
open Microsoft.AspNetCore.Http
open Blog.FSharpWebAPI.Models
open CompostionRoot
                               

let labelsHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
           getAll |>  ctx.WriteJsonAsync 


let labelHandler(id: int) =
    fun (next : HttpFunc) (ctx : HttpContext) ->
           getLabel id
           |> function
                     | Some l -> ctx.WriteJsonAsync l
                     | None   -> (setStatusCode 400 >=> json "label not found") next ctx
  
  
let labelAddHandler : HttpHandler =
      fun (next : HttpFunc) (ctx : HttpContext) ->
          task {
              let! label = ctx.BindJsonAsync<Label>()
                          
              let addedLabel = addLabelAsync label 
                               |> Async.RunSynchronously
              return! Successful.OK addedLabel next ctx
           }