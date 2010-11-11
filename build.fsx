#I @"lib"
#r "FakeLib.dll"
#r "Fake.Gallio.dll"
#r "System.Xml.Linq"
#load "fake.fsx"

open System
open System.Xml.Linq
open Fake
open Fake.FileUtils

let version = "0.3.0"
let buildDir = "merged"
let config = getBuildParamOrDefault "config" "debug"
let target = getBuildParamOrDefault "target" "BuildAll"

let slnBuild sln x = 
    sln |> build (fun p -> { p with 
                                Targets = [x] 
                                Properties = ["Configuration", config] })

let mainSln = slnBuild "solrnet.sln"
let sampleSln = slnBuild "SampleSolrApp.sln"

Target "Clean" (fun _ -> 
    mainSln "Clean"
    sampleSln "Clean"
    rm_rf buildDir
)

Target "Build" (fun _ -> mainSln "Rebuild")
Target "BuildSample" (fun _ -> sampleSln "Rebuild")

Target "BuildAll" DoNothing

let testAssemblies = !+ "**/bin/**/*Tests.dll" |> Scan
let noIntegrationTests = "exclude Category: Integration"
let onlyIntegrationTests = "Category: Integration"

Target "Test" (fun _ ->
    testAssemblies |> Gallio.Run (fun p -> { p with Filters = noIntegrationTests })
)

Target "Coverage" (fun _ ->
    testAssemblies |> Gallio.Run (fun p -> { p with 
                                                Filters = noIntegrationTests
                                                RunnerType = "NCover"
                                                PluginDirectories = ["lib"] })
)

Target "IntegrationTest" (fun _ ->
    Solr.start()
    try
        testAssemblies |> Gallio.Run (fun p -> { p with Filters = onlyIntegrationTests })
    finally
        Solr.stop()
)

let libs = ["SolrNet"; "SolrNet.DSL"; "HttpWebAdapters"; "Castle.Facilities.SolrNetIntegration"; "Ninject.Integration.SolrNet"; "NHibernate.SolrNet"; "Structuremap.SolrNetIntegration"]
let dlls = libs |> List.map (fun l -> l + ".dll")
let dirs = libs |> List.map (fun l -> l @@ "bin" @@ config)

Target "Merge" (fun _ ->
    rm_rf buildDir
    mkdir buildDir
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
    for l in libs do
        AssemblyInfo (fun p -> { p with
                                    OutputFileName = l @@ "Properties\\AssemblyInfo.cs"
                                    CLSCompliant = true
                                    AssemblyTitle = l
                                    AssemblyDescription = l
                                    AssemblyProduct = l
                                    AssemblyCopyright = "Copyright Mauricio Scheffer 2007-" + DateTime.Now.Year.ToString()
                                    Guid = "6688f9b4-5f2d-4fd6-aafc-3a81c84a69f1"
                                    AssemblyVersion = version
                                    AssemblyFileVersion = version })
)

Target "ReleasePackage" (fun _ -> 
    let outputPath = "build"
    rm_rf outputPath
    mkdir outputPath
    !+ (buildDir @@ "SolrNet.*")
        ++ "license.txt" ++ "lib\\Microsoft.Practices.*"
        |> Scan 
        |> Copy outputPath
    !+ (outputPath @@ "*") 
        |> Scan 
        |> Zip outputPath ("SolrNet-"+version+".zip")
    rm_rf outputPath
)

Target "PackageSampleApp" (fun _ ->
    let outputSolr = buildDir @@ solr
    cp_r solr outputSolr
    rm_rf (outputSolr @@ "solr\\data")
    let logs = outputSolr @@ "logs"
    rm_rf logs
    mkdir logs

    cp_r "tools\\Cassini" (buildDir @@ "tools\\Cassini")

    let sampleApp = "SampleSolrApp"
    let outputSampleApp = buildDir @@ sampleApp
    cp_r sampleApp outputSampleApp
    rm_rf (outputSampleApp @@ "obj")
    rm_rf (outputSampleApp @@ "log.txt")
    rm_rf (outputSampleApp @@ "SampleSolrApp.sln.cache")
    mkdir (outputSampleApp @@ "lib")

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
        |> Zip buildDir ("SolrNet-"+version+"-sample.zip")
)

"Test" <== ["BuildAll"]
"BuildAll" <== ["Build";"Merge";"BuildSample"]
"ReleasePackage" <== ["Clean";"Version";"BuildAll"]

Run target