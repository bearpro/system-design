module Tests

open System
open Xunit
open SystemDesign.ServiceBusClient

[<Fact>]
let ``Json for ADD_ROW format correct`` () =
    let student = { Id = -1; Surname = "Kek"; Name = "Lol"; SecondName = "Top"; StudyGroupId = -1 }
    let data = Some { IsBinariesChanged = false; EntityName = "student"; PlainData = student; BinaryLinks = Map.empty }
    let makeMassage stringData = { From = "1337"; To = "dean"; Subject = AddRow; Data = stringData }
    let json = serializeToMessage data makeMassage
    let expectedJson = """{
  "from": "1337",
  "to": "dean",
  "subject": "ADD_ROW",
  "data": "{\r\n  \"isBinariesChanged\": false,\r\n  \"entityName\": \"student\",\r\n  \"plainData\": {\r\n    \"id\": -1,\r\n    \"surname\": \"Kek\",\r\n    \"name\": \"Lol\",\r\n    \"second_name\": \"Top\",\r\n    \"study_group_id\": -1\r\n  },\r\n  \"binaryLinks\": {\r\n  }\r\n}"
}"""
    Assert.Equal(expectedJson, json)

[<Fact>]
let ``Json for UPDATE_SUBSCRIPTION format correct`` () =
    let data = Some { Address = "1337"; EntityName = "student"; Type = Common }
    let makeMassage stringData = { From = "1337"; To = "dean"; Subject = UpdateSubscription; Data = stringData }
    let json = serializeToMessage data makeMassage
    let expectedJson = """{
  "from": "1337",
  "to": "dean",
  "subject": "UPDATE_SUBSCRIPTION",
  "data": "{\r\n  \"address\": \"1337\",\r\n  \"entityName\": \"student\",\r\n  \"type\": \"COMMON\"\r\n}"
}"""
    Assert.Equal(expectedJson, json)

[<Fact>]
let ``Json for INIT_INSTANCE format correct`` () =
    let data = None
    let makeMassage data = { From = "1337"; To = "dean"; Subject = InitInstance; Data = data }
    let json = serializeToMessage data makeMassage
    let expectedJson = """{
  "from": "1337",
  "to": "dean",
  "subject": "INIT_INSTANCE",
  "data": null
}"""
    Assert.Equal(expectedJson, json)