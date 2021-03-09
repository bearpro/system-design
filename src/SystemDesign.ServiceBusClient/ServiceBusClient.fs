namespace SystemDesign

open FSharp.Json
open FsHttp
open FsHttp.DslCE

module ServiceBusClient =

    type Subject = 
    | [<JsonUnionCase("INIT_INSTANCE")>]        InitInstance 
    | [<JsonUnionCase("UPDATE_SUBSCRIPTION")>]  UpdateSubscription 
    | [<JsonUnionCase("ADD_ROW")>]              AddRow

    type Message = 
      { [<JsonField("from")>]    From: string
        [<JsonField("to")>]      To: string
        [<JsonField("subject")>] Subject: Subject
        [<JsonField("data")>]    Data: string option }

    type Student = 
      { [<JsonField("id")>]             Id: int
        [<JsonField("surname")>]        Surname: string
        [<JsonField("name")>]           Name: string
        [<JsonField("second_name")>]    SecondName: string
        [<JsonField("study_group_id")>] StudyGroupId: int }

    type AddRowData = 
      { [<JsonField("isBinariesChanged")>]  IsBinariesChanged: bool
        [<JsonField("entityName")>]         EntityName: string
        [<JsonField("plainData")>]          PlainData: Student
        [<JsonField("binaryLinks")>]        BinaryLinks: Map<unit, unit> }

    type UpdateSubscriptionType = 
    | [<JsonUnionCase("COMMON")>] Common

    type UpdateSubscriptionData =
      { [<JsonField("address")>]    Address: string
        [<JsonField("entityName")>] EntityName: string
        [<JsonField("type")>]       Type: UpdateSubscriptionType }

    let serializeToMessage data (makeMassage) =
        let stringData = data |> Option.map Json.serialize
        let message = Json.serialize (makeMassage stringData)
        message

    let send payload =
        http {
            POST "http://up-lab1.mirea.ru/bus"
            Accept "text/plain, application/json, application/*+json, */*"
            UserAgent "Java/15"
            Connection "keep-alive"
            body
            json payload
        }