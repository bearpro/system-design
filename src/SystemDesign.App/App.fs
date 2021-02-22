module App

open System.Globalization
open Microsoft.AspNetCore.Http
open Giraffe
open Models
open FSharp.Control.Tasks

type DbContext = Database.Database.dataContext

let indexHandler (name : string) =
    let greetings = sprintf "Hello %s, from Giraffe!" name
    let model     = { Text = greetings }
    let view      = Views.index model
    htmlView view

let withService<'Service> (consumer: 'Service -> HttpHandler) : HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        let service = ctx.GetService<'Service>()
        consumer service next ctx

let withDbContext = withService<DbContext>

let addStudent : HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) -> task {
            let db = ctx.GetService<DbContext>()
            let! student = ctx.BindModelAsync<Database.Student>()
            let! result = Async.StartAsTask (Database.Student.add student db)
            match result with
            | Choice1Of2() -> return! Successful.OK "Ok" next ctx
            | Choice2Of2 e -> return! Successful.OK $"Error\n{e}" next ctx
    }


let webApp : HttpFunc -> HttpContext -> HttpFuncResult =
    choose [
        GET >=>
            choose [
                route "/" >=> indexHandler "world"
                routef "/hello/%s" indexHandler
            ]
        subRoute "/api" (
            choose [
                subRoute "/student" (
                    choose [
                        PUT >=> addStudent
                        GET >=> withDbContext(Database.Student.all >> json)
                    ]
                )
                subRoute "/study_plan" (
                    choose [
                        GET >=> withDbContext(Database.StudyPlan.all >> json) 
                    ])
            ])
        setStatusCode 404 >=> text "Not Found" ]