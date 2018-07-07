open Saturn
open System.IO
open Giraffe
open Layout
open Argu

type Arguments =
    | [<AltCommandLine("-p")>] Port of port:int
with
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Port _ -> "Specify a port."

let dir = Directory.GetCurrentDirectory()

let handler = scope {
    getf "%s" (fun s ->
        let p = dir + s.Replace("/", "\\")
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

let app port = application {
    url (sprintf "http://0.0.0.0:%d/" port)
    router handler
}

[<EntryPoint>]
let main argv =

    let parser = ArgumentParser.Create<Arguments>(programName = "tennis")
    let results = parser.Parse(argv, raiseOnUsage = false, ignoreUnrecognized = true)
    if not results.IsUsageRequested then
        let port = results.GetResult (Port, defaultValue = 8080)
        app port
        |> run
    else
        parser.PrintUsage() |> printfn "%s"
    0