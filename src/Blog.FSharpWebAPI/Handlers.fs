module Blog.FSharpWebAPI.Handlers

open Blog.FSharpWebAPI.Models
open CompostionRoot
open Giraffe
open Microsoft.AspNetCore.Http
open System

let labelsHandler = fun (next : HttpFunc) (ctx : HttpContext) -> getAll |> ctx.WriteJsonAsync

let labelHandler (id : int) = 
    fun (next : HttpFunc) (ctx : HttpContext) -> 
        getLabel id |> function 
        | Some l -> ctx.WriteJsonAsync l
        | None -> (setStatusCode 400 >=> json "label not found") next ctx

let labelAddHandler : HttpHandler = 
    fun (next : HttpFunc) (ctx : HttpContext) -> 
        task { 
            let! label = ctx.BindJsonAsync<Label>()
            let result = 
                addLabelAsync label
                |> Async.RunSynchronously
                |> function 
                | Some l -> Successful.CREATED l next ctx
                | None -> (setStatusCode 400 >=> json "label not found") next ctx
            return! result
        }

let labelUpdateHandler = 
    fun (next : HttpFunc) (ctx : HttpContext) -> 
        task { 
            let! label = ctx.BindJsonAsync<Label>()
            let result = 
                updateLabel label |> function 
                | Some l -> ctx.WriteJsonAsync l
                | None -> (setStatusCode 400 >=> json "label not found") next ctx
            return! result
        }
