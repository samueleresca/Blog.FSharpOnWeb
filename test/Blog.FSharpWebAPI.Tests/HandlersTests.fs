module Handlers.Tests

open Microsoft.EntityFrameworkCore
open Xunit
open Blog.FSharpWebAPI
open Blog.FSharpWebAPI.Models
open Handlers
open Fixtures
open Giraffe
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.TestHost
open Microsoft.AspNetCore.Http



[<Fact>]
let ``/label should returns the correct response`` () =
    use server = new TestServer(createHost())
    use client = server.CreateClient()

    get client "/label"
    |> ensureSuccess
