module HttpFunc
open System.Net
open System.Net.Http
open Xunit

    

let runTask task = 
    task
    |> Async.AwaitTask
    |> Async.RunSynchronously

let get (client : HttpClient) (path : string) = 
    path
    |> client.GetAsync
    |> runTask
    
let post (client : HttpClient) (path : string) (content: HttpContent) = 
    client.PostAsync(path, content) |> runTask
    
let put (client : HttpClient) (path : string) (content: HttpContent) = 
       client.PutAsync(path, content) |> runTask

let delete (client: HttpClient) (path: string) = 
        client.DeleteAsync(path) |> runTask

let createRequest (method : HttpMethod) (path : string) = 
    let url = "http://127.0.0.1" + path
    new HttpRequestMessage(method, url)

let ensureSuccess (response : HttpResponseMessage) = 
    if not response.IsSuccessStatusCode then 
        response.Content.ReadAsStringAsync()
        |> runTask
        |> failwithf "%A"
    else response

let isStatus (code : HttpStatusCode) (response : HttpResponseMessage) = 
    Assert.Equal(code, response.StatusCode)
    response

let isOfType (response : HttpResponseMessage) (contentType : string)  = 
    Assert.Equal(contentType, response.Content.Headers.ContentType.MediaType)
    response

let readText (response : HttpResponseMessage) = response.Content.ReadAsStringAsync() |> runTask
