module Journal

open Browser.Dom
open Fable.Core
open Fable.React
open Fable.Import
open Thoth.Fetch
open Fable.React.Helpers
open Fable.React.Props
open Elmish
open Elmish.React
open SystemDesign.Types
open Fable.Core.JS

type Model = Waiting | Showing of JournalItem list

type Message = Show of JournalItem list

let getJournalItems dispatch = promise {
    let! (response: JournalItem list) = Fetch.get("http://localhost:8082/api/journal/all", caseStrategy = Thoth.Json.CamelCase)
    dispatch (Show response)
}

let init() = Waiting, Cmd.ofSub(getJournalItems >> ignore)

let update msg model = 
    match msg with
    | Show items -> Showing items, Cmd.none

let journalLine item =
    tr [] [
        td [] [str (string item.StudentId)]
        td [] [str (string item.InTime)]
        td [] [str (string item.StudyPlanId)]
        td [] [str (string item.MarkId)]
    ]
let journalBottom = tr [] [ button [] [str "Add"] ]

let view model (dispatch: Dispatch<_>) =
    div 
        []
        [
            match model with 
            | Waiting -> str "waiting..."
            | Showing x -> table [ ] ([
                tr [ ] [
                    th [] [str "Student id"]
                    th [] [str "in time"]
                    th [] [str "study plan"]
                    th [] [str "mark"]
                ]
            ] 
            @ List.map journalLine x 
            @ [ journalBottom ] )
        ]
