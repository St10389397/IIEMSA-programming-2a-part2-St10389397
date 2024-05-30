using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

class Program
{
    static List<Recipe> recipes = new List<Recipe>();
    const string recipeFilePath = "recipes.xml";
    static Recipe[] recipeArray; // Array to store recipes

    static void Main(string[] args)
    {
        LoadRecipes();

        Console.WriteLine("\u001b[32mWelcome to Recipe Manager!\u001b[0m");

        while (true)
        {
            Console.WriteLine("\nMenu:");
            Console.WriteLine("\u001b[33m1. Add a new recipe\u001b[0m");
            Console.WriteLine("\u001b[36m2. View recorded recipes\u001b[0m");
            Console.WriteLine("\u001b[35m3. Scale a recipe\u001b[0m");
            Console.WriteLine("\u001b[34m4. Reset recipe scale\u001b[0m");
            Console.WriteLine("\u001b[31m5. Exit\u001b[0m");
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
            currentRecipe.AddIngredient(name, quantity, unit);
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

        // Add the new recipe to the list and array
        recipes.Add(currentRecipe);
        AddRecipeToArray(currentRecipe); // Adding to the array
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Recipe added successfully!");
        Console.ResetColor();
    }

    static void AddRecipeToArray(Recipe recipe)
    {
        if (recipeArray == null)
            recipeArray = new Recipe[] { recipe };
        else
        {
            Array.Resize(ref recipeArray, recipeArray.Length + 1);
            recipeArray[recipeArray.Length - 1] = recipe;
        }
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

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\nRecorded Recipes:");
        Console.ResetColor();
        for (int i = 0; i < recipes.Count; i++)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Recipe {i + 1}:");
            Console.ResetColor();
            Console.WriteLine(recipes[i]);
        }
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
            Console.WriteLine($"{i + 1}. {recipes[i]}");
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
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Recipe scaled successfully!");
        Console.ResetColor();
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
            Console.WriteLine($"{i + 1}. {recipes[i]}");
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
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Recipe reset successfully!");
        Console.ResetColor();
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
}

[DataContract] // Attribute to allow serialization
class Ingredient
{
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public double Quantity { get; set; }
    [DataMember]
    public string Unit { get; set; }

    // Adding a property to store the original quantity
    public double OriginalQuantity { get; set; }

    public Ingredient(string name, double quantity, string unit)
    {
        Name = name;
        Quantity = quantity;
        Unit = unit;
        OriginalQuantity = quantity; // Store the original quantity
    }

    public override string ToString()
    {
        return $"{Quantity} {Unit} of {Name}";
    }
}

[DataContract] // Attribute to allow serialization
class Recipe
{
    [DataMember]
    public List<Ingredient> Ingredients { get; set; }
    [DataMember]
    public List<string> Steps { get; set; }

    public Recipe()
    {
        Ingredients = new List<Ingredient>();
        Steps = new List<string>();
    }

    public void AddIngredient(string name, double quantity, string unit)
    {
        Ingredients.Add(new Ingredient(name, quantity, unit));
    }

    public void AddStep(string step)
    {
        Steps.Add(step);
    }

    public void ScaleRecipe(double factor)
    {
        foreach (var ingredient in Ingredients)
        {
            // Scale the quantity
            ingredient.Quantity = ingredient.OriginalQuantity * factor;
        }
    }

    public void ResetQuantities()
    {
        // Reset quantities to their original values
        foreach (var ingredient in Ingredients)
        {
            ingredient.Quantity = ingredient.OriginalQuantity;
        }
    }

    public void ClearRecipe()
    {
        Ingredients.Clear();
        Steps.Clear();
    }

    public override string ToString()
    {
        string recipeDetails = "Ingredients:\n";
        foreach (var ingredient in Ingredients)
        {
            recipeDetails += $"{ingredient}\n";
        }
        recipeDetails += "\nSteps:\n";
        for (int i = 0; i < Steps.Count; i++)
        {
            recipeDetails += $"{i + 1}. {Steps[i]}\n";
        }
        return recipeDetails;
    }
}
