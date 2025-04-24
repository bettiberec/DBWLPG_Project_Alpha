module MealMaster.UI

open System

let green s = $"\u001b[32m{s}\u001b[0m"
let red s = $"\u001b[31m{s}\u001b[0m"
let bold s = $"\u001b[1m{s}\u001b[0m"

let box (title: string) =
    let line = String.replicate (title.Length + 4) "="
    printfn "\n%s\n| %s |\n%s" line title line

let printIngredients (ingredients: string list) =
    ingredients |> List.iter (fun i -> printfn "  - %s" i)
