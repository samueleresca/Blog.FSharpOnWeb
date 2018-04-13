module Blog.FSharpWebAPI.Handlers

open Blog.FSharpWebAPI.Models
open CompostionRoot
open Giraffe
open Microsoft.AspNetCore.Http

let labelsHandler = fun (next : HttpFunc) (ctx : HttpContext) -> getAll |> ctx.WriteJsonAsync

let labelHandler (id : int) = 
    fun (next : HttpFunc) (ctx : HttpContext) -> 
        getLabel id |> function 
        | Some l -> ctx.WriteJsonAsync l
        | None -> (setStatusCode 404 >=> json "Label not found") next ctx

let labelAddHandler : HttpHandler = 
    fun (next : HttpFunc) (ctx : HttpContext) -> 
        task { 
            let! label = ctx.BindJsonAsync<Label>()
            let result = 
                addLabelAsync label
                |> Async.RunSynchronously
                |> function 
                | Some l -> Successful.CREATED l next ctx
                | None -> (setStatusCode 400 >=> json "Label not added") next ctx
            return! result
        }

let labelUpdateHandler (id: int) = 
    fun (next : HttpFunc) (ctx : HttpContext) -> 
        task { 
            let! label = ctx.BindJsonAsync<Label>()
            let result = 
                updateLabel label id |> function 
                | Some l -> ctx.WriteJsonAsync l
                | None -> (setStatusCode 400 >=> json "Label not updated") next ctx
            return! result
        }

let labelDeleteHandler (id: int) = 
    fun (next : HttpFunc) (ctx : HttpContext) -> 
        deleteLabel id |> function 
        | Some l -> ctx.WriteJsonAsync l
        | None -> (setStatusCode 404 >=> json "Label not deleted") next ctx
