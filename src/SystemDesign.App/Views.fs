module Views

open Giraffe.ViewEngine
open Models

let layout (content: XmlNode list) =
    html [] [
        head [] [
            title []  [ encodedText "SystemDesign.App" ]
            link [ _rel  "stylesheet"
                   _type "text/css"
                   _href "/main.css" ]
        ]
        body [] content
    ]

let partial () =
    h1 [] [ encodedText "SystemDesign.App" ]

let index (model : Message) =
    [
        partial()
        p [] [ encodedText model.Text ]
    ] |> layout