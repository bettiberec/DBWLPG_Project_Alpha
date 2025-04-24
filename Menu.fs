module MealMaster.Menu

open System
open System.IO
open MealMaster.Meal
open MealMaster.Loader
open MealMaster.Logic
open MealMaster.UI

let saveGroceryListToFile (filename: string) (list: (string * int) list) =
    let lines =
        list
        |> List.map (fun (ingredient, count) ->
            if count > 1 then sprintf "- %s (%dx)" ingredient count
            else sprintf "- %s" ingredient)
    File.WriteAllLines(filename, lines)
    printfn "%s" (green $"Your grocery list saved to %s{filename}")

let rec promptMealDetails (meals: Meal list) =
    printfn "\nEnter the number of a meal to see the details, or press Enter to go back:"
    match Console.ReadLine() with
    | input when input.Trim() = "" -> ()
    | input ->
        match Int32.TryParse(input) with
        | true, n when n > 0 && n <= meals.Length ->
            let meal = meals.[n - 1]
            box meal.Name
            printfn "%sPrep time:%s %d minutes" (bold "") "" meal.PrepTime
            printfn "%sIngredients:%s" (bold "") ""
            printIngredients meal.Ingredients
            promptMealDetails meals
        | _ ->
            printfn "%s" (red "Invalid input. Please try again.")
            promptMealDetails meals

let rec mainMenu (meals: Meals) =
    box "Meal Master"
    printfn "1. About this app"
    printfn "2. Meals by your ingredients"
    printfn "3. Meals by prep time"
    printfn "4. Mealy by type"
    printfn "5. Surprise me!"
    printfn "6. Create grocery list from meals"
    printfn "7. Search meals by ingredient"
    printfn "8. Exit"
    printf "> "
    match Console.ReadLine() with

    | "1" ->
        box "About Meal Master"
        printfn "Meal Master is a command-line meal planning app written in F#. It helps you decide what to cook based on"
        printfn"the ingredients you have, how much time you have, or simply by the type of meal you're in the mood for."
        mainMenu meals
    | "2" ->
        printfn "\nWhat ingredients do you have? (please seperate them with comma):"
        let input = Console.ReadLine().ToLower()
        let available = input.Split(",") |> Array.map (fun s -> s.Trim()) |> Set.ofArray
        let exact = meals |> List.filter (canMakeMeal available)
        let near = meals |> List.choose (fun m ->
            let (meal, missing) = getNearMatches available m
            if missing.Length > 0 && missing.Length <= 2 then Some (meal, missing) else None)

        if exact.IsEmpty && near.IsEmpty then
            printfn "%s" (red "I'm sorry, it looks like you can't make anything with these ingredients. :( ")
        else
            if not exact.IsEmpty then
                box "You can make these meals"
                exact |> List.iter (fun m -> printfn "- %s" m.Name)
            if not near.IsEmpty then
                box "Meals you could make with some extra ingredients"
                near |> List.iter (fun (m, missing) ->
                    printfn "For %s you're missing: %s" m.Name (String.concat ", " missing))
        mainMenu meals

    | "3" ->
        printf "\nWhat is the max time you want to spend cooking the meal? (in minutes): "
        match Int32.TryParse(Console.ReadLine()) with
        | true, maxTime ->
            let filtered = meals |> List.filter (fun m -> m.PrepTime <= maxTime)
            if filtered.IsEmpty then
                printfn "%s" (red $"No meals found under %d{maxTime} minutes.")
            else
                filtered |> List.iteri (fun i m -> printfn "%d. %s" (i+1) m.Name)
                promptMealDetails filtered
        | _ -> printfn "%s" (red "Invalid input.")
        mainMenu meals

    | "4" ->
        printf "\nDo you want to make breakfast, luch or dinner? "
        let inputType = Console.ReadLine().Trim().ToLower()
        let filtered = meals |> List.filter (fun m -> m.MealType.ToLower() = inputType)
        if filtered.IsEmpty then
            printfn "%s" (red $"No meals found for %s{inputType}")
        else
            filtered |> List.iteri (fun i m -> printfn "%d. %s" (i+1) m.Name)
            promptMealDetails filtered
        mainMenu meals

    | "5" ->
        let rnd = Random()
        let meal = meals.[rnd.Next(meals.Length)]
        box "Your surprise Meal"
        printfn "%s" (bold meal.Name)
        printfn "Prep time: %d minutes" meal.PrepTime
        printfn "Ingredients:"
        printIngredients meal.Ingredients
        mainMenu meals

    | "6" ->
        box "All the meals we currently have"
        meals |> List.iteri (fun i m -> printfn "%d. %s" (i+1) m.Name)
        printf "\nEnter the meal numbers to include in your grocery list (please seperate them with comma): "
        let input = Console.ReadLine()
        let indices =
            input.Split(",")
            |> Array.choose (fun s -> match Int32.TryParse(s.Trim()) with | true, n -> Some n | _ -> None)
            |> Array.toList
        let selected = getMealByIndices indices meals
        if selected.IsEmpty then
            printfn "%s" (red "No valid meals selected.")
        else
            let list = generateGroceryList selected
            box "Grocery List"
            list |> List.iter (fun (ingredient, count) ->
                if count > 1 then printfn "- %s (%dx)" ingredient count
                else printfn "- %s" ingredient)
            printf "\nDo you want to save your list to a file? (y/n): "
            let answer = Console.ReadLine().Trim().ToLower()
            if answer = "y" then
                saveGroceryListToFile "grocery-list.txt" list
        mainMenu meals

    | "7" ->
        printf "\nEnter an ingredient to search for: "
        let keyword = Console.ReadLine().Trim().ToLower()
        let filtered = meals |> List.filter (fun m -> m.Ingredients |> List.exists (fun i -> i.Contains(keyword)))
        if filtered.IsEmpty then
            printfn "%s" (red $"No meals found with %s{keyword} :(")
        else
            box $"Meals that have '%s{keyword}'"
            filtered |> List.iteri (fun i m -> printfn "%d. %s" (i+1) m.Name)
            promptMealDetails filtered
        mainMenu meals

    | "8" ->
        printfn "%s" (green "Have fun cooking!")
        ()

    | _ ->
        printfn "%s" (red "Invalid option. Please try again.")
        mainMenu meals
