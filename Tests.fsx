﻿#I "lib"
#r "System.Configuration"
#r @"gallio.dll"
#r @"MbUnit.dll"
#r @"packages\Fuchu.0.4.0.0\lib\Fuchu.dll"
#r @"SolrNet.Tests\bin\Debug\SolrNet.Tests.dll"
#r @"SolrNet.Tests.Integration\bin\Debug\SolrNet.Tests.Integration.dll"

open System
open System.Globalization
open System.Threading
open Fuchu
open SolrNet.Tests
open SolrNet.Tests.Integration


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

let solrUrl = "http://localhost:8983/solr"
System.Configuration.ConfigurationManager.AppSettings.["solr"] <- solrUrl

let test = 
    Fuchu.MbUnit.MbUnitTestToFuchu typeof<SolrQueryExecuterTests>
    |> Test.filter (fun s -> s.Contains "GetCollapseExpandParameters")
    |> testWithCultures [CultureInfo "en-US"; CultureInfo "fr-FR"]
run test
