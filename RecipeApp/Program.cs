using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;

class Program
{
    static List<Recipe> recipes = new List<Recipe>();
    const string recipeFilePath = "recipes.xml";

    // Delegate for notifying when calories exceed 300
    delegate void CalorieNotification(string message);
    static event CalorieNotification NotifyCalorieExcess;

    static void Main(string[] args)
    {
        NotifyCalorieExcess += DisplayCalorieWarning;

        LoadRecipes();

        Console.WriteLine("\u001b[32mWelcome to Recipe Manager!\u001b[0m");

        while (true)
        {
            Console.WriteLine("\nMenu:");
            Console.WriteLine("\u001b[33m1. Add a new recipe\u001b[0m");
            Console.WriteLine("\u001b[36m2. View recorded recipes\u001b[0m");
            Console.WriteLine("\u001b[35m3. Scale a recipe\u001b[0m");
            Console.WriteLine("\u001b[34m4. Reset recipe scale\u001b[0m");
            Console.WriteLine("\u001b[31m5. Reset all recipes\u001b[0m");
            Console.WriteLine("\u001b[31m6. Exit\u001b[0m");
            Console.Write("\u001b[35mEnter your choice: \u001b[0m");
            int choice = Convert.ToInt32(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    AddRecipe();
                    break;
                case 2:
                    ViewRecipes();
                    break;
                case 3:
                    ScaleRecipe();
                    break;
                case 4:
                    ResetRecipeScale();
                    break;
                case 5:
                    ResetAllRecipes();
                    break;
                case 6:
                    SaveRecipes();
                    Console.WriteLine("\u001b[31mExiting...\u001b[0m");
                    return;
                default:
                    Console.WriteLine("\u001b[31mInvalid choice. Please try again.\u001b[0m");
                    break;
            }
        }
    }

    static void AddRecipe()
    {
        Recipe currentRecipe = new Recipe();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("Enter the recipe name: ");
        Console.ResetColor();
        currentRecipe.Name = Console.ReadLine();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("Enter the number of ingredients: ");
        Console.ResetColor();
        int numIngredients = Convert.ToInt32(Console.ReadLine());

        // Prompt the user to enter each ingredient
        for (int i = 0; i < numIngredients; i++)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\nIngredient {i + 1}:");
            Console.ResetColor();
            Console.Write("Name: ");
            string name = Console.ReadLine();
            Console.Write("Quantity: ");
            double quantity = Convert.ToDouble(Console.ReadLine());
            Console.Write("Unit: ");
            string unit = Console.ReadLine();
            Console.Write("Calories: ");
            int calories = Convert.ToInt32(Console.ReadLine());
            Console.Write("Food Group: ");
            string foodGroup = Console.ReadLine();
            currentRecipe.AddIngredient(name, quantity, unit, calories, foodGroup);
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("\nEnter the number of steps: ");
        Console.ResetColor();
        int numSteps = Convert.ToInt32(Console.ReadLine());

        // Prompt the user to enter each step
        for (int i = 0; i < numSteps; i++)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\nStep {i + 1}:");
            Console.ResetColor();
            Console.Write("Description: ");
            string step = Console.ReadLine();
            currentRecipe.AddStep(step);
        }

        // Calculate total calories and check if they exceed 300
        currentRecipe.CalculateTotalCalories();
        if (currentRecipe.TotalCalories > 300)
        {
            NotifyCalorieExcess?.Invoke($"Warning: The total calories for {currentRecipe.Name} exceed 300!");
        }

        // Add the new recipe to the list
        recipes.Add(currentRecipe);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Recipe added successfully!");
        Console.ResetColor();

        // Save recipes immediately after adding
        SaveRecipes();
    }

    static void ViewRecipes()
    {
        if (recipes.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("No recipes recorded yet.");
            Console.ResetColor();
            return;
        }

        // Sort recipes by name alphabetically
        var sortedRecipes = recipes.OrderBy(r => r.Name).ToList();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\nRecorded Recipes:");
        Console.ResetColor();
        for (int i = 0; i < sortedRecipes.Count; i++)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Recipe {i + 1}: {sortedRecipes[i].Name}");
            Console.ResetColor();
        }

        Console.Write("\nEnter the index of the recipe you want to view: ");
        int index = Convert.ToInt32(Console.ReadLine()) - 1;

        if (index < 0 || index >= sortedRecipes.Count)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid recipe index.");
            Console.ResetColor();
            return;
        }

        Console.WriteLine(sortedRecipes[index]);
    }

    static void ScaleRecipe()
    {
        if (recipes.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("No recipes available to scale.");
            Console.ResetColor();
            return;
        }

        Console.WriteLine("Enter the index of the recipe you want to scale:");
        for (int i = 0; i < recipes.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {recipes[i].Name}");
        }

        Console.Write("Enter the index: ");
        int index = Convert.ToInt32(Console.ReadLine()) - 1;

        if (index < 0 || index >= recipes.Count)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid recipe index.");
            Console.ResetColor();
            return;
        }

        Console.Write("Enter the scaling factor (0.5 for half, 2 for double, 3 for triple): ");
        double factor = Convert.ToDouble(Console.ReadLine());
        recipes[index].ScaleRecipe(factor);
        recipes[index].CalculateTotalCalories();
        if (recipes[index].TotalCalories > 300)
        {
            NotifyCalorieExcess?.Invoke($"Warning: The total calories for {recipes[index].Name} exceed 300 after scaling!");
        }
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Recipe scaled successfully!");
        Console.ResetColor();

        // Save recipes immediately after scaling
        SaveRecipes();
    }

    static void ResetRecipeScale()
    {
        if (recipes.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("No recipes available to reset.");
            Console.ResetColor();
            return;
        }

        Console.WriteLine("Enter the index of the recipe you want to reset:");
        for (int i = 0; i < recipes.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {recipes[i].Name}");
        }

        Console.Write("Enter the index: ");
        int index = Convert.ToInt32(Console.ReadLine()) - 1;


        if (index < 0 || index >= recipes.Count)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid recipe index.");
            Console.ResetColor();
            return;
        }

        recipes[index].ResetQuantities();
        recipes[index].CalculateTotalCalories();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Recipe reset successfully!");
        Console.ResetColor();

        // Save recipes immediately after resetting
        SaveRecipes();
    }

    static void ResetAllRecipes()
    {
        if (recipes.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("No recipes recorded yet.");
            Console.ResetColor();
            return;
        }

        Console.Write("Are you sure you want to reset all recipes? (yes/no): ");
        string response = Console.ReadLine().ToLower();
        if (response == "yes" || response == "y")
        {
            foreach (var recipe in recipes)
            {
                recipe.ResetQuantities();
                recipe.CalculateTotalCalories();
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("All recipes have been reset successfully!");
            Console.ResetColor();

            // Save recipes immediately after resetting all
            SaveRecipes();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Reset all recipes operation cancelled.");
            Console.ResetColor();
        }
    }

    static void SaveRecipes()
    {
        try
        {
            using (FileStream fs = new FileStream(recipeFilePath, FileMode.Create))
            {
                var serializer = new DataContractSerializer(typeof(List<Recipe>));
                serializer.WriteObject(fs, recipes);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Recipes saved successfully!");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error saving recipes: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void LoadRecipes()
    {
        if (File.Exists(recipeFilePath))
        {
            try
            {
                using (FileStream fs = new FileStream(recipeFilePath, FileMode.Open))
                {
                    var serializer = new DataContractSerializer(typeof(List<Recipe>));
                    recipes = (List<Recipe>)serializer.ReadObject(fs);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Recipes loaded successfully!");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error loading recipes: {ex.Message}");
                Console.ResetColor();
            }
        }
    }

    static void DisplayCalorieWarning(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}

[DataContract]
class Ingredient
{
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public double Quantity { get; set; }
    [DataMember]
    public string Unit { get; set; }
    [DataMember]
    public double OriginalQuantity { get; set; }
    [DataMember]
    public int Calories { get; set; }
    [DataMember]
    public string FoodGroup { get; set; }

    public Ingredient(string name, double quantity, string unit, int calories, string foodGroup)
    {
        Name = name;
        Quantity = quantity;
        Unit = unit;
        OriginalQuantity = quantity;
        Calories = calories;
        FoodGroup = foodGroup;
    }

    public override string ToString()
    {
        return $"{Quantity} {Unit} of {Name} ({Calories} calories, {FoodGroup})";
    }
}

[DataContract]
class Recipe
{
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public List<Ingredient> Ingredients { get; set; }
    [DataMember]
    public List<string> Steps { get; set; }
    [DataMember]
    public int TotalCalories { get; private set; }

    public Recipe()
    {
        Ingredients = new List<Ingredient>();
        Steps = new List<string>();
    }

    public void AddIngredient(string name, double quantity, string unit, int calories, string foodGroup)
    {
        Ingredients.Add(new Ingredient(name, quantity, unit, calories, foodGroup));
    }

    public void AddStep(string step)
    {
        Steps.Add(step);
    }

    public void ScaleRecipe(double factor)
    {
        foreach (var ingredient in Ingredients)
        {
            ingredient.Quantity = ingredient.OriginalQuantity * factor;
        }
        CalculateTotalCalories();
    }

    public void ResetQuantities()
    {
        foreach (var ingredient in Ingredients)
        {
            ingredient.Quantity = ingredient.OriginalQuantity;
        }
        CalculateTotalCalories();
    }

    public void CalculateTotalCalories()
    {
        TotalCalories = Ingredients.Sum(ingredient => ingredient.Calories);
    }

    public override string ToString()
    {
        string recipeDetails = $"Name: {Name}\nIngredients:\n";
        foreach (var ingredient in Ingredients)
        {
            recipeDetails += $"{ingredient}\n";
        }
        recipeDetails += $"\nTotal Calories: {TotalCalories}\nSteps:\n";
        for (int i = 0; i < Steps.Count; i++)
        {
            recipeDetails += $"{i + 1}. {Steps[i]}\n";
        }
        return recipeDetails;
    }
}
