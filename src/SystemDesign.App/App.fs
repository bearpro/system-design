module App

open Microsoft.AspNetCore.Http
open Giraffe
open FSharp.Control.Tasks

type DbContext = Database.Database.dataContext

let invar = Some System.Globalization.CultureInfo.InvariantCulture

module CrudCommon =
    let getAll<'entity> all : HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            let db = ctx.GetService<DbContext>()
            let result : 'entity list = all db
            (setStatusCode 200 >=> json result) next ctx

    let addEntity<'entity> add : HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) -> task {
                let db = ctx.GetService<DbContext>()
                let! student = ctx.BindModelAsync<'entity>()
                let! result = Async.StartAsTask (add student db)
                match result with
                | Choice1Of2() -> return! Successful.OK "Ok" next ctx
                | Choice2Of2 e -> return! Successful.OK $"Error: {e}" next ctx
        }

    let updateEntity<'entity> update id : HttpHandler =
            fun (next: HttpFunc) (ctx: HttpContext) -> task {
                let db = ctx.GetService<DbContext>()
                let! student = ctx.BindModelAsync<'entity>()
                let! result = update id student db |> Async.StartAsTask
                match result with
                | Ok () -> return! Successful.OK "Ok" next ctx
                | Error msg -> return! Successful.OK $"Error: {msg}" next ctx
            }

    let getEntityById<'entity> get id : HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) ->
            let db = ctx.GetService<DbContext>()
            let result : 'entity option = get id db
            match result with
            | Some value -> (setStatusCode 200 >=> json value) next ctx
            | None -> setStatusCode 404 next ctx

    let deleteEntityById delete id : HttpHandler =
        fun (next: HttpFunc) (ctx: HttpContext) -> task {
            let db = ctx.GetService<DbContext>()
            let! _ = (delete id db |> Async.StartChildAsTask)
            return! (Successful.OK "Ok" next ctx)
        }

let getStudentsHandler : HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        let db = ctx.GetService<DbContext>()
        let groupFilter = ctx.TryGetQueryStringValue "groupId" |> Option.map int
        let result = 
            match groupFilter with
            | Some groupId -> Database.Student.findByGroupId groupId db
            | None -> Database.Student.all db
        json result next ctx

let getJournalHandler : HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) -> 
        let db = ctx.GetService<DbContext>()
        let studentFilter = ctx.TryGetQueryStringValue "studentId" |> Option.map int
        let groupFilter = ctx.TryGetQueryStringValue "groupId" |> Option.map int
        let result = 
            match studentFilter, groupFilter with
            | Some studentId, _ -> Database.Journal.findByStudentId studentId db
            | None, Some groupId -> Database.Journal.findByGroupId groupId db
            | None, None -> Database.Journal.all db
        json result next ctx

open CrudCommon

let webApp : HttpFunc -> HttpContext -> HttpFuncResult =
    choose [
        subRoute "/api" (
            choose [
                subRoute "/student" (
                    choose [
                        POST >=> addEntity Database.Student.add
                        PUT >=> subRoutef "/%i" (updateEntity Database.Student.update)
                        DELETE >=> subRoutef "/%i" (deleteEntityById Database.Student.delete)
                        GET >=> choose [
                            subRoute "/all" getStudentsHandler
                            subRoutef "/%i" (getEntityById Database.Student.findById)
                        ]
                    ]
                )
                subRoute "/study_group" (
                     choose [
                        POST >=> addEntity Database.StudyGroup.add
                        PUT >=> subRoutef "/%i" (updateEntity Database.StudyGroup.update)
                        DELETE >=> subRoutef "/%i" (deleteEntityById Database.StudyGroup.delete)
                        GET >=> choose [
                            subRoute "/all" (getAll Database.StudyGroup.all)
                            subRoutef "/%i" (getEntityById Database.StudyGroup.findById)
                        ]
                    ]
                )
                subRoute "/journal" (
                    choose [
                        PUT >=> subRoutef "/%i" (updateEntity Database.Journal.update)
                        POST >=> addEntity Database.Journal.add
                        GET >=> choose [
                            subRoute "/all" getJournalHandler
                            subRoutef "/%i" (getEntityById Database.Student.findById)
                        ]
                    ]
                )
            ])
        GET >=> text "This service is provides HTTP-API. Source code and documetation can be found here: https://github.com/BearPro/system-design"
        setStatusCode 404 >=> text "Not Found" ]