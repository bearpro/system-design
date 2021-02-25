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

type DisplayJournalItem = 
  { Raw: JournalItem
    Subject: string
    Student: string
    ExamType: string
    Mark: string }

type Model = Waiting | Showing of journal: DisplayJournalItem list * groups: StudyGroup list

type Message = 
    | Show of journal: DisplayJournalItem list * groups: StudyGroup list 
    | FilterGroup of groupId: int option

let sudyPlanDisplay studyPlan =
    let subjectName =
        match studyPlan.SubjectId with 
        | 1 -> "ПрИС"
        | 2 -> "СИИ"
        | 3 -> "ПИ"
        | 4 -> "НСиБ"
        | 5 -> "СисАнал"
        | 6 -> "РБД"
        | 7 -> "СПО"
        | _ -> failwith "Unknown subject ID"
    let examType =
        match studyPlan.ExamTypeId with
        | 1 -> "Экзамен"
        | 2 -> "Зачёт"
        | 3 -> "Зачёт с оценкой"
        | 4 -> "Курсовая"
        | _ -> failwith "Unknown exam type ID"
    subjectName, examType

let markName markId =
    match markId with 
    | 1 -> "5"
    | 2 -> "4"
    | 3 -> "3"
    | 4 -> "2"
    | 5 -> "з"
    | 6 -> "н"
    | 7 -> ""
    | _ -> failwith "Unknown markId"

let getJournalItems groupFilter dispatch = promise {
    let hostAndPort = window.location.host.Split(':')
    let host = hostAndPort.[0]
    let journalUri = match groupFilter with
                     | None -> $"http://{host}:8082/api/journal/all"
                     | Some id -> $"http://{host}:8082/api/journal/all?groupId=%i{id}"
    let! (rawJournal: JournalItem list) = Fetch.get(journalUri, caseStrategy = Thoth.Json.CamelCase)
    let! (studyPlan: StudyPlan list) = Fetch.get($"http://{host}:8082/api/study_plan", caseStrategy = Thoth.Json.CamelCase)
    let! (students: Student list) = Fetch.get($"http://{host}:8082/api/student/all", caseStrategy = Thoth.Json.CamelCase)
    let! (groups: StudyGroup list) = Fetch.get($"http://{host}:8082/api/study_group/all", caseStrategy = Thoth.Json.CamelCase)
    let journal = 
        rawJournal 
        |> List.map (fun x -> 
           let subject, exam = studyPlan |> List.find (fun studyPlan -> studyPlan.Id = x.StudyPlanId) |> sudyPlanDisplay
           let student = students |> List.find (fun student -> student.Id = x.StudentId)
           { Raw = x;
             Subject = subject
             Student = $"{student.Name} {student.SecondName}"
             Mark = markName x.MarkId
             ExamType = exam })
    dispatch (Show(journal, groups))
}

let init() = Waiting, Cmd.ofSub(getJournalItems None >> ignore)

let update msg model =
    console.log $"{msg}"
    match msg with
    | Show(items, groups) -> Showing(items, groups), Cmd.none
    | FilterGroup filter -> model, Cmd.ofSub(getJournalItems filter >> ignore)

let journalLine item =
    tr [] [
        td [] [str (item.Student)]
        td [] [str (item.Subject)]
        td [Style [ Color ( if [4; 6; 7] |> List.contains item.Raw.MarkId then "red" else "black" ) ] ] [str (item.Mark)]
        td [] [str (item.ExamType)]
        td [] [str (string item.Raw.Count)]
    ]

let view model (dispatch: Dispatch<_>) =
    div 
        []
        [
            div []
                (
                    match model with 
                    | Waiting -> [ str "waiting..." ]
                    | Showing(_, groups) -> [ 
                        select [ 
                            OnChange (
                                fun event -> 
                                    let id = groups |> List.tryFind (fun x -> x.Name.ToUpper() = string event.Value) |> Option.map (fun x -> x.Id)
                                    dispatch (FilterGroup id)
                                    ()
                            )] 
                            (( option [ OnSelect (fun _ -> dispatch (FilterGroup None)) ] [str "Все"]) 
                            :: [for group in groups -> option [] [str (group.Name.ToUpper())]]
                            )
                        ]
                )
            div [] 
                [
                    match model with 
                    | Waiting -> str "waiting..."
                    | Showing(x, _) -> table [ Class "table" ] ([
                        thead [ Class "thead" ]
                            [
                                tr [ ] [
                                    th [] [str "Имя студента"]
                                    th [] [str "Предмет"]
                                    th [] [str "Оценка"]
                                    th [] [str "Тип экзамена"]
                                    th [] [str "Попытка"]
                            ]
                        ]
                    ] 
                    @ List.map journalLine x )
                ]
        ]
