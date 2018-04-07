module Blog.FSharpWebAPI.Models
// ---------------------------------
// Models
// ---------------------------------

type Label =
    {
        Id : int
        Code: string
        IsoCode: string
        Content: string
        Inactive: bool
    }



type Message =
    {
        Text: string
    }
