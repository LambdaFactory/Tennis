open Saturn
open System
open System.IO
open Giraffe
open Layout
open System.IO

let dir = Directory.GetCurrentDirectory()

let handler = scope {
    getf "%s" (fun s ->
        let p = dir + s.Replace("/", "\\")
        printfn "PATH: %s" p
        if Directory.Exists p then
            let dirs = Directory.GetDirectories p |> Seq.toList
            let files = Directory.GetFiles(p, "*.*", SearchOption.TopDirectoryOnly) |> Seq.toList
            printfn "DIRS: %A" dirs
            printfn "FILES: %A" files
            htmlView (fileList dir p dirs files)
        elif File.Exists p then
            streamFile true p None None
        else
            htmlView (layout p [])

    )
}

let app port = application {
    url (sprintf "http://0.0.0.0:%d/" port)
    router handler
}

[<EntryPoint>]
let main _ =
    app 8080
    |> run
    0