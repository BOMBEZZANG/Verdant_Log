using UnityEngine;
using UnityEditor;
using VerdantLog.Data;

namespace VerdantLog.Editor
{
    public static class CreateTestAssets
    {
        [MenuItem("Verdant Log/Create Test Assets/Create All Test Assets")]
        public static void CreateAllTestAssets()
        {
            CreateTestItems();
            CreateTestPlants();
            CreateTestRecipes();
            CreateProgressionData();
            CreateTestZones();
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("Test assets created successfully!");
        }
        
        [MenuItem("Verdant Log/Create Test Assets/Items")]
        public static void CreateTestItems()
        {
            // Create Seeds
            CreateItem("seed_basic", "Basic Seed", "A common seed found in the forest", ItemType.Seed);
            CreateItem("seed_mystic", "Mystic Seed", "A mysterious seed that glows faintly", ItemType.Seed);
            CreateItem("seed_crystal", "Crystal Seed", "A seed encased in crystal", ItemType.Seed);
            
            // Create Materials
            CreateItem("mat_crystal_dust", "Crystal Dust", "Powdered crystals used in cultivation", ItemType.Material);
            CreateItem("mat_moonwater", "Moonwater", "Water collected under moonlight", ItemType.Material);
            CreateItem("mat_fertile_soil", "Fertile Soil", "Rich soil perfect for growing", ItemType.Material);
            
            // Create Plant Items (harvest results)
            CreateItem("plant_greenleaf", "Greenleaf", "A common medicinal herb", ItemType.Plant);
            CreateItem("plant_moonflower", "Moonflower", "A flower that blooms at night", ItemType.Plant);
            CreateItem("plant_crystalbloom", "Crystal Bloom", "A rare crystalline flower", ItemType.Plant);
        }
        
        [MenuItem("Verdant Log/Create Test Assets/Plants")]
        public static void CreateTestPlants()
        {
            CreatePlant("plant_greenleaf", "Greenleaf", "A common medicinal herb", 10, "plant_greenleaf", 1, "seed_basic");
            CreatePlant("plant_moonflower", "Moonflower", "A flower that blooms only at night", 25, "plant_moonflower", 1, "seed_mystic");
            CreatePlant("plant_crystalbloom", "Crystal Bloom", "A rare crystalline flower", 50, "plant_crystalbloom", 1, "seed_crystal");
        }
        
        [MenuItem("Verdant Log/Create Test Assets/Recipes")]
        public static void CreateTestRecipes()
        {
            // Basic Seed -> Greenleaf (no conditions)
            var recipe1 = CreateRecipe("recipe_greenleaf", "Greenleaf Recipe", "Plant in any conditions", "seed_basic", "plant_greenleaf");
            AssetDatabase.CreateAsset(recipe1, "Assets/Data/Recipes/recipe_greenleaf.asset");
            
            // Mystic Seed -> Moonflower (requires night time)
            var recipe2 = CreateRecipe("recipe_moonflower", "Moonflower Recipe", "Plant at night for best results", "seed_mystic", "plant_moonflower");
            recipe2.RequiredConditions.Add(new CultivationCondition(ConditionType.TimeOfDay, "Night"));
            AssetDatabase.CreateAsset(recipe2, "Assets/Data/Recipes/recipe_moonflower.asset");
            
            // Crystal Seed -> Crystal Bloom (requires crystal dust)
            var recipe3 = CreateRecipe("recipe_crystalbloom", "Crystal Bloom Recipe", "Use crystal dust when planting", "seed_crystal", "plant_crystalbloom");
            recipe3.RequiredConditions.Add(new CultivationCondition(ConditionType.ItemUsed, "mat_crystal_dust"));
            AssetDatabase.CreateAsset(recipe3, "Assets/Data/Recipes/recipe_crystalbloom.asset");
        }
        
        [MenuItem("Verdant Log/Create Test Assets/Progression Data")]
        public static void CreateProgressionData()
        {
            var progressionData = ScriptableObject.CreateInstance<ProgressionData>();
            AssetDatabase.CreateAsset(progressionData, "Assets/Resources/ProgressionData.asset");
        }
        
        [MenuItem("Verdant Log/Create Test Assets/Zones")]
        public static void CreateTestZones()
        {
            // Hub Zone (always unlocked)
            CreateZone("zone_hub", "Greenhouse Hub", "Your personal greenhouse and laboratory", "Hub_Scene", UnlockType.Always, 0);
            
            // Forest Zone (unlocked at start)
            CreateZone("zone_forest", "Verdant Forest", "A lush forest full of common plants", "Forest_Scene", UnlockType.Always, 0);
            
            // Cave Zone (requires level 3)
            CreateZone("zone_cave", "Crystal Cave", "A mysterious cave with crystalline formations", "Cave_Scene", UnlockType.Level, 3);
            
            // Swamp Zone (requires level 5)
            CreateZone("zone_swamp", "Mystic Swamp", "A foggy swamp with rare nocturnal plants", "Swamp_Scene", UnlockType.Level, 5);
        }
        
        private static ItemData CreateItem(string id, string name, string description, ItemType type)
        {
            var item = ScriptableObject.CreateInstance<ItemData>();
            var serializedObject = new SerializedObject(item);
            
            serializedObject.FindProperty("itemID").stringValue = id;
            serializedObject.FindProperty("itemName").stringValue = name;
            serializedObject.FindProperty("description").stringValue = description;
            serializedObject.FindProperty("itemType").enumValueIndex = (int)type;
            serializedObject.FindProperty("isStackable").boolValue = true;
            serializedObject.FindProperty("maxStackSize").intValue = 99;
            
            serializedObject.ApplyModifiedProperties();
            
            string path = $"Assets/Data/Items/{id}.asset";
            AssetDatabase.CreateAsset(item, path);
            
            return item;
        }
        
        private static PlantData CreatePlant(string id, string name, string description, int expValue, string harvestItemID, int harvestAmount, string seedItemID)
        {
            var plant = ScriptableObject.CreateInstance<PlantData>();
            var serializedObject = new SerializedObject(plant);
            
            serializedObject.FindProperty("plantID").stringValue = id;
            serializedObject.FindProperty("plantName").stringValue = name;
            serializedObject.FindProperty("description").stringValue = description;
            serializedObject.FindProperty("expValue").intValue = expValue;
            serializedObject.FindProperty("harvestYieldAmount").intValue = harvestAmount;
            serializedObject.FindProperty("growthTime").floatValue = 30f;
            serializedObject.FindProperty("growthStages").intValue = 3;
            
            serializedObject.ApplyModifiedProperties();
            
            // Load referenced items
            var harvestItem = AssetDatabase.LoadAssetAtPath<ItemData>($"Assets/Data/Items/{harvestItemID}.asset");
            var seedItem = AssetDatabase.LoadAssetAtPath<ItemData>($"Assets/Data/Items/{seedItemID}.asset");
            
            serializedObject.Update();
            serializedObject.FindProperty("harvestYield").objectReferenceValue = harvestItem;
            serializedObject.FindProperty("seedItem").objectReferenceValue = seedItem;
            serializedObject.ApplyModifiedProperties();
            
            string path = $"Assets/Data/Plants/{id}.asset";
            AssetDatabase.CreateAsset(plant, path);
            
            return plant;
        }
        
        private static CultivationRecipe CreateRecipe(string id, string name, string hint, string seedItemID, string plantID)
        {
            var recipe = ScriptableObject.CreateInstance<CultivationRecipe>();
            var serializedObject = new SerializedObject(recipe);
            
            serializedObject.FindProperty("recipeID").stringValue = id;
            serializedObject.FindProperty("recipeName").stringValue = name;
            serializedObject.FindProperty("recipeHint").stringValue = hint;
            
            serializedObject.ApplyModifiedProperties();
            
            // Load referenced items
            var seedItem = AssetDatabase.LoadAssetAtPath<ItemData>($"Assets/Data/Items/{seedItemID}.asset");
            var plant = AssetDatabase.LoadAssetAtPath<PlantData>($"Assets/Data/Plants/{plantID}.asset");
            
            serializedObject.Update();
            serializedObject.FindProperty("inputSeed").objectReferenceValue = seedItem;
            serializedObject.FindProperty("outputPlant").objectReferenceValue = plant;
            serializedObject.ApplyModifiedProperties();
            
            return recipe;
        }
        
        private static ZoneData CreateZone(string id, string name, string description, string sceneName, UnlockType unlockType, int requiredLevel)
        {
            var zone = ScriptableObject.CreateInstance<ZoneData>();
            var serializedObject = new SerializedObject(zone);
            
            serializedObject.FindProperty("zoneID").stringValue = id;
            serializedObject.FindProperty("zoneName").stringValue = name;
            serializedObject.FindProperty("zoneDescription").stringValue = description;
            serializedObject.FindProperty("sceneName").stringValue = sceneName;
            
            var unlockReq = serializedObject.FindProperty("unlockRequirement");
            unlockReq.FindPropertyRelative("unlockType").enumValueIndex = (int)unlockType;
            unlockReq.FindPropertyRelative("requiredLevel").intValue = requiredLevel;
            
            serializedObject.ApplyModifiedProperties();
            
            string path = $"Assets/Data/Zones/{id}.asset";
            AssetDatabase.CreateAsset(zone, path);
            
            return zone;
        }
    }
}