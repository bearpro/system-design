module App

open Browser.Dom
open Fable.Core
open Fable.React
open Fable.Import
open Fable.React.Helpers
open Fable.React.Props
open Elmish
open Elmish.React

type Model = { JournalModel : Journal.Model }

type Message = JournalMsg of Journal.Message

let init _ = 
    let journalModel, journalCmd = Journal.init ()
    { JournalModel = journalModel }, Cmd.map JournalMsg journalCmd

let update msg model =
    match msg with
    // | ChangeView view -> { model with CurrentView = view }, Cmd.Empty
    | JournalMsg msg' -> 
        let updatedModel, command = Journal.update msg' model.JournalModel
        { model with JournalModel = updatedModel }, Cmd.map JournalMsg command

let view model dispatch =
    div 
        [ Class "container" ]
        [
            Journal.view model.JournalModel (JournalMsg >> dispatch)
        ]

Program.mkProgram init update view
|> Program.withReactSynchronous "elmish-main"
|> Program.run