module Blog.FSharpWebAPI.Handlers
open Giraffe;
open Microsoft.AspNetCore.Http
open Blog.FSharpWebAPI.Models
open CompostionRoot

let labelsHandler(name: string) = 
                     json(getLabel 1)
                                                                         
                           