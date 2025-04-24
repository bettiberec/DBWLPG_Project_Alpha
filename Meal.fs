module MealMaster.Meal

type Ingredient = string

type Meal = {
    Name: string
    Ingredients: Ingredient list
    MealType: string
    PrepTime: int
}

type Meals = Meal list


