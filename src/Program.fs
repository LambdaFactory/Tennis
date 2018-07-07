open Saturn
open System.IO
open Giraffe
open Layout

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
let main _ =
    app 8080
    |> run
    0