module Blog.FSharpWebAPI.Handlers
open Microsoft.AspNetCore.Http
open Giraffe
open Blog.FSharpWebAPI.RequestModels
open CompostionRoot

let labelsHandler = fun (next : HttpFunc) (ctx : HttpContext) -> getAll |> ctx.WriteJsonAsync

let labelHandler (id : int) = 
    fun (next : HttpFunc) (ctx : HttpContext) -> 
        getLabel id |> function 
        | Some l -> ctx.WriteJsonAsync l
        | None -> (setStatusCode 404 >=> json "Label not found") next ctx

let labelAddHandler : HttpHandler = 
    fun (next : HttpFunc) (ctx : HttpContext) -> 
        task { 
            let! label = ctx.BindJsonAsync<CreateUpdateLabelRequest>()
            match label.HasErrors with
                | Some msg -> return! (setStatusCode 400 >=> json msg) next ctx
                | None -> return! addLabelAsync label.GetLabel
                                |> Async.RunSynchronously
                                |> function 
                                | Some l -> Successful.CREATED l next ctx
                                | None -> (setStatusCode 400 >=> json "Label not added") next ctx
        }

let labelUpdateHandler (id: int) = 
    fun (next : HttpFunc) (ctx : HttpContext) -> 
        task { 
            let! label = ctx.BindJsonAsync<CreateUpdateLabelRequest>()
            match label.HasErrors with
                | Some msg -> return! (setStatusCode 400 >=> json msg) next ctx
                | None ->  return! updateLabel label.GetLabel id |> function 
                                | Some l -> ctx.WriteJsonAsync l
                                | None -> (setStatusCode 400 >=> json "Label not updated") next ctx
        }

let labelDeleteHandler (id: int) =
    fun (next : HttpFunc) (ctx : HttpContext) -> 
        deleteLabel id |> function 
        | Some l -> ctx.WriteJsonAsync l
        | None -> (setStatusCode 404 >=> json "Label not deleted") next ctx
