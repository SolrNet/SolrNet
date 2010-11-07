#I @"lib"
#r "FakeLib.dll"
#r "Fake.Gallio.dll"
#r "System.Xml.Linq"

open System
open System.Xml.Linq
open Fake

let version = "0.3.0"
let buildDir = "merged"
let config = getBuildParamOrDefault "config" "debug"
let target = getBuildParamOrDefault "target" "BuildAll"

// helper functions
let (.>) (a: #seq<XElement>) (b: string) = Extensions.Descendants(a, xname b)
let startsWith s (n: XElement) = n.Value.StartsWith s
let contains s (n: XElement) = n.Value.Contains s
let setValue s (n: XElement) = n.Value <- s
let replaceValue orig repl (n: XElement) = n.Value <- n.Value.Replace((orig:string), (repl:string))

let slnBuild sln x = 
    sln |> build (fun p -> { p with 
                                Targets = [x] 
                                Properties = ["Configuration", config] })
let mainSln = slnBuild "solrnet.sln"
let sampleSln = slnBuild "SampleSolrApp.sln"

Target "Clean" (fun _ -> 
    mainSln "Clean"
    sampleSln "Clean"
    DeleteDir buildDir
)

Target "Build" (fun _ -> mainSln "Rebuild")
Target "BuildSample" (fun _ -> sampleSln "Rebuild")

Target "BuildAll" DoNothing

let testAssemblies = !+ "**/bin/**/*Tests.dll" |> Scan
let gallioFilters = "exclude Category: Integration"

Target "Test" (fun _ ->
    testAssemblies |> Gallio.Run (fun p -> { p with Filters = gallioFilters })
)

Target "Coverage" (fun _ ->
    testAssemblies |> Gallio.Run (fun p -> { p with 
                                                Filters = gallioFilters
                                                RunnerType = "NCover"
                                                PluginDirectories = ["lib"] })
)

let libs = ["SolrNet"; "SolrNet.DSL"; "HttpWebAdapters"; "Castle.Facilities.SolrNetIntegration"; "Ninject.Integration.SolrNet"; "NHibernate.SolrNet"; "Structuremap.SolrNetIntegration"]
let dlls = libs |> List.map (fun l -> l + ".dll")
let dirs = libs |> List.map (fun l -> l @@ "bin" @@ config)

Target "Merge" (fun _ ->
    DeleteDir buildDir
    CreateDir buildDir
    let main = "SolrNet\\bin" @@ config @@ "SolrNet.dll"
    let output = buildDir @@ dlls.[0]
    ILMerge (fun p -> { p with 
                            ToolPath = "lib\\ilmerge.exe"
                            Libraries = dlls |> Seq.skip 1
                            SearchDirectories = dirs
                            Internalize = InternalizeExcept "ilmerge.exclude"
                            XmlDocs = true
                       }) output main
)

Target "Version" (fun _ ->
    libs 
    |> Seq.iter (fun l ->
        AssemblyInfo (fun p -> { p with
                                    OutputFileName = l @@ "Properties\\AssemblyInfo.cs"
                                    CLSCompliant = true
                                    AssemblyTitle = l
                                    AssemblyDescription = l
                                    AssemblyProduct = l
                                    AssemblyCopyright = sprintf "Copyright Mauricio Scheffer 2007-%d" DateTime.Now.Year
                                    Guid = "6688f9b4-5f2d-4fd6-aafc-3a81c84a69f1"
                                    AssemblyVersion = version
                                    AssemblyFileVersion = version }))
)

Target "ReleasePackage" (fun _ -> 
    let outputPath = "build"
    DeleteDir outputPath
    CreateDir outputPath
    !+ (buildDir @@ "SolrNet.*")
        ++ "license.txt" ++ "lib\\Microsoft.Practices.*"
        |> Scan 
        |> Copy outputPath
    !+ (outputPath @@ "*") 
        |> Scan 
        |> Zip outputPath (sprintf "SolrNet-%s.zip" version)
    DeleteDir outputPath
)

Target "PackageSampleApp" (fun _ ->
    //System.Diagnostics.Debugger.Break()
    let solr = "solr-1.4.0"
    let outputSolr = buildDir @@ solr
    CopyDir outputSolr solr allFiles
    DeleteDir (outputSolr @@ "solr\\data")
    let logs = outputSolr @@ "logs"
    DeleteDir logs
    CreateDir logs

    CopyDir (buildDir @@ "tools\\Cassini") "tools\\Cassini" allFiles

    let sampleApp = "SampleSolrApp"
    let outputSampleApp = buildDir @@ sampleApp
    CopyDir outputSampleApp sampleApp allFiles
    DeleteDir (outputSampleApp @@ "obj")
    DeleteFile (outputSampleApp @@ "log.txt")
    DeleteFile (outputSampleApp @@ "SampleSolrApp.sln.cache")
    CreateDir (outputSampleApp @@ "lib")    

    !+ (outputSampleApp @@ "bin\\*") 
        -- "**\\SampleSolrApp.*" -- "**\\SolrNet.*"
        |> Scan
        |> Copy (outputSampleApp @@ "lib")
   
    ["pingsolr.js"; "license.txt"; "runsample.bat"] |> Copy buildDir

    let csproj = outputSampleApp @@ "SampleSolrApp.csproj"
    let xml = XDocument.Load csproj
    let refs = xml.Elements() .> "ItemGroup" .> "Reference" .> "HintPath"
    refs
    |> Seq.filter (startsWith @"..\lib")
    |> Seq.iter (replaceValue @"..\" "")
    refs
    |> Seq.filter (contains "SolrNet.dll")
    |> Seq.iter (setValue @"..\SolrNet.dll")
    xml.Save csproj
    
    !+ (buildDir @@ "**\\*") 
        |> Scan 
        |> Zip buildDir (sprintf "SolrNet-%s-sample.zip" version)
)

"Test" <== ["BuildAll"]
"BuildAll" <== ["Build";"Merge";"BuildSample"]
"ReleasePackage" <== ["Clean";"Version";"BuildAll"]

Run target