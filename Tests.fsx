#I "lib"
#r @"gallio.dll"
#r @"MbUnit.dll"
#r @"packages\Fuchu.0.3.0.1\lib\net40-client\Fuchu.dll"
#r @"SolrNet.Tests\bin\Debug\SolrNet.Tests.dll"

open System
open System.Globalization
open System.Threading
open Fuchu
open SolrNet.Tests


let withCulture culture f x = 
    let c = Thread.CurrentThread.CurrentCulture
    try
        Thread.CurrentThread.CurrentCulture <- culture
        f x
    finally
        Thread.CurrentThread.CurrentCulture <- c

let testWithCultures (cultures: #seq<CultureInfo>) = 
    Test.replaceTestCode <| fun name test ->
        testList name [
            for c in cultures ->
                testCase c.Name (withCulture c test)
        ]

//let test = 
//    typeof<SolrQueryResultsParserTests>.Assembly.GetExportedTypes()
//    |> Seq.filter (fun (t: Type) -> not t.IsAbstract)
//    |> Seq.map Fuchu.MbUnit.MbUnitTestToFuchu
//    |> TestList


let test = 
    Fuchu.MbUnit.MbUnitTestToFuchu typeof<CollapseResponseParserTests>
    //|> Test.filter (fun s -> s.Contains "ParseResultsWithGroups")
    //|> testWithCultures [CultureInfo "en-US"; CultureInfo "fr-FR"]
run test
