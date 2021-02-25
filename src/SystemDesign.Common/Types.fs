module SystemDesign.Types

open System

[<CLIMutable>]
type Student = 
  { Id: int
    Surname: string
    Name: string
    SecondName: string
    StudyGroupId: int }

type JournalItem = 
  { Id: int
    StudentId: int
    StudyPlanId: int
    InTime: int
    Count: int
    MarkId: int }

[<CLIMutable>]
type StudyGroup = { Id: int; Name: string }

[<CLIMutable>]
type StudyPlan = { Id: int; SubjectId: int; ExamTypeId: int }