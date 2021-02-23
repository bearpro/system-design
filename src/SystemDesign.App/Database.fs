module Database

open FSharp.Data.Sql

type Database = SqlDataProvider<Common.DatabaseProviderTypes.MYSQL, "Server=********">

type DbContext = Database.dataContext

[<CLIMutable>]
type StudyPlan = { Id: int; SubjectId: int; ExamTypeId: int }

module StudyPlan =
    let toRecord (source: DbContext.``systemdesign.study_planEntity``) =
        { Id = source.Id; SubjectId = source.SubjectId; ExamTypeId = source.ExamTypeId}
    
    let all (ctx: DbContext) =
        ctx.Systemdesign.StudyPlan |> Seq.map toRecord |> List.ofSeq

[<CLIMutable>]
type StudyGroup = { Id: int; Name: string }

module StudyGroup =
    let toRecord (source: DbContext.``systemdesign.study_groupEntity``) =
        { Id = source.Id; Name = source.Name }
    
    let all (ctx: DbContext) =
        ctx.Systemdesign.StudyGroup |> Seq.map toRecord |> List.ofSeq

    let findById id (ctx: Database.dataContext) =
        query { for group in ctx.Systemdesign.StudyGroup do 
                where (group.Id = id) 
                select (Some(toRecord group)) 
                exactlyOneOrDefault } 

    let add group (ctx: DbContext) =
        let row = ctx.Systemdesign.StudyGroup.Create()
        row.Name <- group.Name
        ctx.SubmitUpdatesAsync() |> Async.Catch

    let update groupId group (ctx: DbContext) =
        let oldGroup = query { for s in ctx.Systemdesign.StudyGroup do
                                 where (s.Id = groupId) 
                                 select (Some s) 
                                 exactlyOneOrDefault }
        async {
            match oldGroup with
            | None -> return Error ($"Group with id {groupId} not found.")
            | Some found -> 
                found.Name <- group.Name
                let! updateResult = ctx.SubmitUpdatesAsync() |> Async.Catch
                match updateResult with
                | Choice1Of2 () -> return Ok()
                | Choice2Of2 e -> return Error ($"{e}")
        }

    let delete groupId (ctx: DbContext) =
        let group = query { for group in ctx.Systemdesign.StudyGroup do 
                              where (group.Id = groupId)
                              select (Some group)
                              exactlyOneOrDefault }
        async {
            match group with
            | Some found -> found.Delete()
                            do! ctx.SubmitUpdatesAsync()
                            return Ok()
            | None -> return Error($"No group with id {groupId} found.")
        }

[<CLIMutable>]
type Student = 
  { Id: int
    Surname: string
    Name: string
    SecondName: string
    StudyGroupId: int }

module Student =
    let toRecord (source: DbContext.``systemdesign.studentEntity``) =
      { Id = source.Id
        Surname = source.Surname
        Name = source.Name
        SecondName = source.SecondName
        StudyGroupId = source.StudyGroupId }
    
    let all (ctx: Database.dataContext) = 
        ctx.Systemdesign.Student |> Seq.map toRecord |> List.ofSeq
    
    let findById id (ctx: Database.dataContext) =
        query { for student in ctx.Systemdesign.Student do 
                where (student.Id = id) 
                select (Some(toRecord student)) 
                exactlyOneOrDefault } 

    let add student (ctx: DbContext) =
        let row = ctx.Systemdesign.Student.Create()
        row.Name <- student.Name
        row.Surname <- student.Surname
        row.SecondName <- student.SecondName
        row.StudyGroupId <- student.StudyGroupId
        ctx.SubmitUpdatesAsync() |> Async.Catch

    let update studentId student (ctx: DbContext) =
        let oldStudent = query { for s in ctx.Systemdesign.Student do
                                 where (s.Id = studentId) 
                                 select (Some s) 
                                 exactlyOneOrDefault }
        async {
            match oldStudent with
            | None -> return Error ($"Student with id {studentId} not found.")
            | Some foundStudent -> 
                foundStudent.Name <- student.Name
                foundStudent.SecondName <- student.SecondName
                foundStudent.StudyGroupId <- student.StudyGroupId
                foundStudent.Surname <- student.Surname
                let! updateResult = ctx.SubmitUpdatesAsync() |> Async.Catch
                match updateResult with
                | Choice1Of2 () -> return Ok()
                | Choice2Of2 e -> return Error ($"{e}")
        }

    let delete studentId (ctx: DbContext) =
        let student = query { for student in ctx.Systemdesign.Student do 
                              where (student.Id = studentId)
                              select (Some student)
                              exactlyOneOrDefault }
        async {
            match student with
            | Some foundStudent -> foundStudent.Delete()
                                   do! ctx.SubmitUpdatesAsync()
                                   return Ok()
            | None -> return Error($"No student with id {studentId} found.")
        }

    let findByGroupId groupId (ctx: DbContext) = 
        query { for student in ctx.Systemdesign.Student do 
                where (student.StudyGroupId = groupId) 
                select student } |> Seq.map toRecord |> List.ofSeq