open Saturn
open System.IO
open Giraffe
open Layout
open Argu
open System.Runtime.InteropServices

type Arguments =
    | [<AltCommandLine("-p")>] Port of port:int
    | [<AltCommandLine("-d")>] Dir of dir:string
with
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Port _ -> "Specify a port."
            | Dir _ -> "Specify a root directory."


let handler dir = scope {
    getf "%s" (fun s ->
        let isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
        let s = if s = null then "/" else s
        let s = if isWindows then s.Replace("/", "\\") else s
        let p = dir + s
        if Directory.Exists p then
            let dirs = Directory.GetDirectories p |> Seq.toList
            let files = Directory.GetFiles(p, "*.*", SearchOption.TopDirectoryOnly) |> Seq.toList
            htmlView (fileList dir p dirs files)
        elif File.Exists p then
            streamFile true p None None
        else
            htmlView (fileNotFound dir p)
    )
}

let app port dir = application {
    url (sprintf "http://0.0.0.0:%d/" port)
    router (handler dir)
}

[<EntryPoint>]
let main argv =

    let parser = ArgumentParser.Create<Arguments>(programName = "tennis")
    let results = parser.Parse(argv, raiseOnUsage = false, ignoreUnrecognized = true)
    if not results.IsUsageRequested then
        let port = results.GetResult (Port, defaultValue = 8080)
        let dir = results.GetResult (Dir, defaultValue =  Directory.GetCurrentDirectory())
        app port dir
        |> run
    else
        parser.PrintUsage() |> printfn "%s"
    0