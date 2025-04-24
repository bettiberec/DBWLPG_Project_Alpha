open MealMaster.Loader
open MealMaster.Menu

[<EntryPoint>]
let main argv =
    let meals = loadMealsFromJson "foods.json"
    mainMenu meals
    0
