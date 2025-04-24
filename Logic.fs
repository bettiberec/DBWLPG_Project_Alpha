module MealMaster.Logic

open MealMaster.Meal

let canMakeMeal (available: Set<string>) (meal: Meal) =
    meal.Ingredients |> List.forall (fun ing -> Set.contains ing available)

let getNearMatches (available: Set<string>) (meal: Meal) =
    let missing = meal.Ingredients |> List.filter (fun i -> not (available.Contains i))
    (meal, missing)

let getMealByIndices (indices: int list) (meals: Meal list) =
    indices |> List.choose (fun i -> if i > 0 && i <= meals.Length then Some meals.[i-1] else None)

let generateGroceryList (selected: Meal list) =
    selected
    |> List.collect (fun m -> m.Ingredients)
    |> List.groupBy id
    |> List.map (fun (ing, items) -> (ing, List.length items))


