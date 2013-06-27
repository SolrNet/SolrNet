#I "lib"
#r @"gallio.dll"
#r @"MbUnit.dll"
#r @"packages\Fuchu.0.2.2\lib\net40-client\Fuchu.dll"
#r @"SolrNet.Tests\bin\Debug\SolrNet.Tests.dll"

open System
open Fuchu
open SolrNet.Tests


let test = 
    typeof<SolrQueryResultsParserTests>.Assembly.GetExportedTypes()
    |> Seq.filter (fun (t: Type) -> not t.IsAbstract)
    |> Seq.map Fuchu.MbUnit.MbUnitTestToFuchu
    |> TestList


//let test = Fuchu.MbUnit.MbUnitTestToFuchu typeof<SolrQueryResultsParserTests>
run test
