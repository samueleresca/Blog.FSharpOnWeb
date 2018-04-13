module Blog.FSharpWebAPI.Models
// ---------------------------------
// Models
// ---------------------------------

[<CLIMutable>]
type Label =
    {
        Id : int
        Code: string
        IsoCode: string
        Content: string
        Inactive: bool
    }

type CreateUpdateLabelRequest =
    {
        Code: string
        IsoCode: string
        Content: string
        Inactive: bool
    }

    member this.HasErrors =
        if this.Code = null || this.Code = "" then Some "Code is required"
        else if this.Code.Length > 255  then Some "Code is too long"
        else if this.IsoCode.Length > 2 then Some "IsoCode is too long"
        else None

    member this.GetLabel = {     
                                Id= 0;
                                Code = this.Code;
                                IsoCode = this.IsoCode;
                                Content = this.Content;
                                Inactive= this.Inactive
                           }


type Message =
    {
        Text: string
    }

