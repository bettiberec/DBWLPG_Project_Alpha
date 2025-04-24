module MealMaster.Loader

open System.IO
open System.Text.Json
open MealMaster.Meal

let loadMealsFromJson (filePath: string) : Meals =
    let json = File.ReadAllText(filePath)
    JsonSerializer.Deserialize<Meals>(json)
    |> List.map (fun meal ->
        { meal with Ingredients = meal.Ingredients |> List.map (fun ing -> ing.ToLower()) })


