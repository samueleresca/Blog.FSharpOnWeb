module Blog.FSharpWebAPI.Handlers
open Microsoft.AspNetCore.Http
open Giraffe
open Blog.FSharpWebAPI.RequestModels
open Blog.FSharpWebAPI.DataAccess

let labelsHandler = fun (next : HttpFunc) (ctx : HttpContext) ->
                        let context = ctx.RequestServices.GetService(typeof<LabelsContext>) :?> LabelsContext
                        getAll context 
                        |> ctx.WriteJsonAsync

let labelHandler (id : int) = 
    fun (next : HttpFunc) (ctx : HttpContext) -> 
        let context = ctx.RequestServices.GetService(typeof<LabelsContext>) :?> LabelsContext
        getLabel context id |> function 
        | Some l -> ctx.WriteJsonAsync l
        | None -> (setStatusCode 404 >=> json "Label not found") next ctx

let labelAddHandler : HttpHandler = 
    fun (next : HttpFunc) (ctx : HttpContext) -> 
        task { 
            let context = ctx.RequestServices.GetService(typeof<LabelsContext>) :?> LabelsContext
            let! label = ctx.BindJsonAsync<CreateUpdateLabelRequest>()
            match label.HasErrors with
                | Some msg -> return! (setStatusCode 400 >=> json msg) next ctx
                | None -> return! addLabelAsync context label.GetLabel
                                |> Async.RunSynchronously
                                |> function 
                                | Some l -> Successful.CREATED l next ctx
                                | None -> (setStatusCode 400 >=> json "Label not added") next ctx
        }

let labelUpdateHandler (id: int) = 
    fun (next : HttpFunc) (ctx : HttpContext) -> 
        task { 
            let context = ctx.RequestServices.GetService(typeof<LabelsContext>) :?> LabelsContext
            let! label = ctx.BindJsonAsync<CreateUpdateLabelRequest>()
            match label.HasErrors with
                | Some msg -> return! (setStatusCode 400 >=> json msg) next ctx
                | None ->  return! updateLabel context label.GetLabel id |> function 
                                | Some l -> ctx.WriteJsonAsync l
                                | None -> (setStatusCode 400 >=> json "Label not updated") next ctx
        }

let labelDeleteHandler (id: int) =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        let context = ctx.RequestServices.GetService(typeof<LabelsContext>) :?> LabelsContext
        deleteLabel context id |> function 
        | Some l -> ctx.WriteJsonAsync l
        | None -> (setStatusCode 404 >=> json "Label not deleted") next ctx
