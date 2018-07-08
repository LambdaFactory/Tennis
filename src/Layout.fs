module Layout

open Giraffe.GiraffeViewEngine
open System.IO

let layout dir (content: XmlNode list) =
    html [_class "has-navbar-fixed-top"] [
        head [] [
            meta [_charset "utf-8"]
            meta [_name "viewport"; _content "width=device-width, initial-scale=1" ]
            title [] [encodedText ("Tennis | " + dir)]
            link [_rel "stylesheet"; _href "https://maxcdn.bootstrapcdn.com/font-awesome/4.7.0/css/font-awesome.min.css" ]
            link [_rel "stylesheet"; _href "https://cdnjs.cloudflare.com/ajax/libs/bulma/0.6.1/css/bulma.min.css" ]

        ]
        body [] [
            yield nav [ _class "navbar is-fixed-top has-shadow" ] [
                div [_class "navbar-brand"] [
                    a [_class "navbar-item"; _href "/"] [
                        img [_src "https://raw.githubusercontent.com/LambdaFactory/Tennis/master/img/icon.png"; _width "28"; _height "28"]
                    ]
                ]
                div [_class "navbar-menu"; _id "navMenu"] [
                    div [_class "navbar-start"] [
                        span [_class "navbar-item";] [rawText ("Current path - " + dir)]
                    ]
                ]
            ]
            yield! content
        ]
]


let fileList (runningDir: string) (currentDir: string) (dirs: string list) (files: string list) =
    let s =
        section [_class "section"] [
            div [_class "content"] [
                ul [] [
                    let parent = Directory.GetParent(currentDir)
                    let relativeDir = if parent = null then currentDir else parent.FullName
                    yield! dirs |> List.map (fun n ->
                        let p = Path.GetRelativePath(relativeDir, n)
                        let name = Path.GetRelativePath(currentDir, n)
                        li [] [ a [_href p] [rawText name ]])
                    yield! files |> List.map (fun n ->
                        let p = Path.GetRelativePath(relativeDir, n)
                        let name = Path.GetRelativePath(currentDir, n)
                        li [] [a [_href p] [rawText name ]] )
                ]
            ]
        ]
    layout currentDir [s]

let fileNotFound (runningDir: string) (path: string) =
    let p = Path.GetDirectoryName (path)
    let p = Path.GetRelativePath(runningDir,p)
    let s =
        section [_class "section"] [
            div [_class "content"] [

                h2 [] [rawText "File Not Found"]
                a [_href p] [rawText "Go Back"]
            ]
        ]
    layout path [s]