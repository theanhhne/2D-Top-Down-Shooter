using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEditor.Tilemaps;

public static class GenerateTilesAndPalette
{
    public static string CreateAssets()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Map"))
        {
            AssetDatabase.CreateFolder("Assets", "Map");
        }
        if (!AssetDatabase.IsValidFolder("Assets/Map/Tiles"))
        {
            AssetDatabase.CreateFolder("Assets/Map", "Tiles");
        }

        string sheetPath = "Assets/Sprites/Zombie Apocalypse Tileset Reference.png";
        var allAssets = AssetDatabase.LoadAllAssetsAtPath(sheetPath);
        var spriteMap = new Dictionary<string, Sprite>();
        foreach (var a in allAssets)
        {
            if (a is Sprite s) spriteMap[s.name] = s;
        }

        Tile CreateTile(string tileName, string spriteName, Tile.ColliderType colliderType)
        {
            string assetPath = $"Assets/Map/Tiles/{tileName}.asset";
            var existing = AssetDatabase.LoadAssetAtPath<Tile>(assetPath);
            if (existing != null)
            {
                if (spriteMap.TryGetValue(spriteName, out var sp))
                {
                    existing.sprite = sp;
                    existing.colliderType = colliderType;
                    EditorUtility.SetDirty(existing);
                }
                return existing;
            }

            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.name = tileName;
            if (spriteMap.TryGetValue(spriteName, out var s))
            {
                tile.sprite = s;
            }
            tile.colliderType = colliderType;
            tile.color = Color.white;
            AssetDatabase.CreateAsset(tile, assetPath);
            return tile;
        }

        var createdTiles = new List<Tile>();

        // Ground Tiles (ColliderType.None)
        // Road
        createdTiles.Add(CreateTile("Tile_Road_01", "Zombie Apocalypse Tileset Reference_6", Tile.ColliderType.None));
        createdTiles.Add(CreateTile("Tile_Road_02", "Zombie Apocalypse Tileset Reference_7", Tile.ColliderType.None));
        createdTiles.Add(CreateTile("Tile_Road_03", "Zombie Apocalypse Tileset Reference_8", Tile.ColliderType.None));
        createdTiles.Add(CreateTile("Tile_Road_04", "Zombie Apocalypse Tileset Reference_9", Tile.ColliderType.None));
        // Road Markings
        createdTiles.Add(CreateTile("Tile_RoadLine_V", "Zombie Apocalypse Tileset Reference_10", Tile.ColliderType.None));
        createdTiles.Add(CreateTile("Tile_RoadLine_Center", "Zombie Apocalypse Tileset Reference_11", Tile.ColliderType.None));
        createdTiles.Add(CreateTile("Tile_RoadLine_H", "Zombie Apocalypse Tileset Reference_12", Tile.ColliderType.None));
        // Pavement / Sidewalk
        createdTiles.Add(CreateTile("Tile_Pavement_01", "Zombie Apocalypse Tileset Reference_39", Tile.ColliderType.None));
        createdTiles.Add(CreateTile("Tile_Pavement_02", "Zombie Apocalypse Tileset Reference_40", Tile.ColliderType.None));
        createdTiles.Add(CreateTile("Tile_Pavement_03", "Zombie Apocalypse Tileset Reference_41", Tile.ColliderType.None));
        createdTiles.Add(CreateTile("Tile_Pavement_04", "Zombie Apocalypse Tileset Reference_42", Tile.ColliderType.None));
        createdTiles.Add(CreateTile("Tile_Pavement_05", "Zombie Apocalypse Tileset Reference_43", Tile.ColliderType.None));
        createdTiles.Add(CreateTile("Tile_Pavement_06", "Zombie Apocalypse Tileset Reference_44", Tile.ColliderType.None));
        // Dirt / Ground (Natural & Smooth)
        createdTiles.Add(CreateTile("Tile_Dirt_01", "Zombie Apocalypse Tileset Reference_272", Tile.ColliderType.None));
        createdTiles.Add(CreateTile("Tile_Dirt_02", "Zombie Apocalypse Tileset Reference_273", Tile.ColliderType.None));
        createdTiles.Add(CreateTile("Tile_Dirt_03", "Zombie Apocalypse Tileset Reference_274", Tile.ColliderType.None));
        createdTiles.Add(CreateTile("Tile_Dirt_04", "Zombie Apocalypse Tileset Reference_87", Tile.ColliderType.None));
        createdTiles.Add(CreateTile("Tile_Dirt_05", "Zombie Apocalypse Tileset Reference_316", Tile.ColliderType.None));
        createdTiles.Add(CreateTile("Tile_Dirt_06", "Zombie Apocalypse Tileset Reference_13", Tile.ColliderType.None));
        // Blood splatters
        createdTiles.Add(CreateTile("Tile_Blood_01", "Zombie Apocalypse Tileset Reference_56", Tile.ColliderType.None));
        createdTiles.Add(CreateTile("Tile_Blood_02", "Zombie Apocalypse Tileset Reference_57", Tile.ColliderType.None));
        createdTiles.Add(CreateTile("Tile_Blood_03", "Zombie Apocalypse Tileset Reference_58", Tile.ColliderType.None));
        createdTiles.Add(CreateTile("Tile_Blood_04", "Zombie Apocalypse Tileset Reference_59", Tile.ColliderType.None));
        createdTiles.Add(CreateTile("Tile_Blood_05", "Zombie Apocalypse Tileset Reference_91", Tile.ColliderType.None));
        createdTiles.Add(CreateTile("Tile_Blood_06", "Zombie Apocalypse Tileset Reference_92", Tile.ColliderType.None));
        createdTiles.Add(CreateTile("Tile_Blood_07", "Zombie Apocalypse Tileset Reference_93", Tile.ColliderType.None));
        createdTiles.Add(CreateTile("Tile_Blood_08", "Zombie Apocalypse Tileset Reference_94", Tile.ColliderType.None));
        createdTiles.Add(CreateTile("Tile_Blood_09", "Zombie Apocalypse Tileset Reference_95", Tile.ColliderType.None));

        // Wall & Obstacle Tiles (ColliderType.Grid)
        createdTiles.Add(CreateTile("Tile_Wall_Brick_01", "Zombie Apocalypse Tileset Reference_434", Tile.ColliderType.Grid));
        createdTiles.Add(CreateTile("Tile_Wall_Brick_02", "Zombie Apocalypse Tileset Reference_435", Tile.ColliderType.Grid));
        createdTiles.Add(CreateTile("Tile_Wall_Brick_03", "Zombie Apocalypse Tileset Reference_436", Tile.ColliderType.Grid));
        createdTiles.Add(CreateTile("Tile_Wall_Brick_04", "Zombie Apocalypse Tileset Reference_437", Tile.ColliderType.Grid));
        createdTiles.Add(CreateTile("Tile_Wall_Window_01", "Zombie Apocalypse Tileset Reference_408", Tile.ColliderType.Grid));
        createdTiles.Add(CreateTile("Tile_Wall_Window_02", "Zombie Apocalypse Tileset Reference_409", Tile.ColliderType.Grid));
        createdTiles.Add(CreateTile("Tile_Wall_Window_03", "Zombie Apocalypse Tileset Reference_410", Tile.ColliderType.Grid));
        createdTiles.Add(CreateTile("Tile_Wall_RoofTrim_01", "Zombie Apocalypse Tileset Reference_369", Tile.ColliderType.Grid));
        createdTiles.Add(CreateTile("Tile_Wall_RoofTrim_02", "Zombie Apocalypse Tileset Reference_370", Tile.ColliderType.Grid));
        createdTiles.Add(CreateTile("Tile_Wall_RoofTrim_03", "Zombie Apocalypse Tileset Reference_371", Tile.ColliderType.Grid));
        createdTiles.Add(CreateTile("Tile_Wall_RoofTrim_04", "Zombie Apocalypse Tileset Reference_372", Tile.ColliderType.Grid));
        createdTiles.Add(CreateTile("Tile_Wall_Roof_01", "Zombie Apocalypse Tileset Reference_336", Tile.ColliderType.Grid));
        createdTiles.Add(CreateTile("Tile_Wall_Roof_02", "Zombie Apocalypse Tileset Reference_337", Tile.ColliderType.Grid));
        createdTiles.Add(CreateTile("Tile_Fence_H_01", "Zombie Apocalypse Tileset Reference_2", Tile.ColliderType.Grid));
        createdTiles.Add(CreateTile("Tile_Fence_H_02", "Zombie Apocalypse Tileset Reference_3", Tile.ColliderType.Grid));
        createdTiles.Add(CreateTile("Tile_Fence_H_03", "Zombie Apocalypse Tileset Reference_4", Tile.ColliderType.Grid));
        createdTiles.Add(CreateTile("Tile_Fence_Wire_01", "Zombie Apocalypse Tileset Reference_147", Tile.ColliderType.Grid));
        createdTiles.Add(CreateTile("Tile_Fence_Wire_02", "Zombie Apocalypse Tileset Reference_148", Tile.ColliderType.Grid));
        createdTiles.Add(CreateTile("Tile_Fence_Wire_03", "Zombie Apocalypse Tileset Reference_149", Tile.ColliderType.Grid));
        createdTiles.Add(CreateTile("Tile_Fence_Post", "Zombie Apocalypse Tileset Reference_150", Tile.ColliderType.Grid));
        createdTiles.Add(CreateTile("Tile_Bush_01", "Zombie Apocalypse Tileset Reference_20", Tile.ColliderType.Grid));
        createdTiles.Add(CreateTile("Tile_Bush_02", "Zombie Apocalypse Tileset Reference_24", Tile.ColliderType.Grid));
        createdTiles.Add(CreateTile("Tile_Bush_03", "Zombie Apocalypse Tileset Reference_26", Tile.ColliderType.Grid));
        createdTiles.Add(CreateTile("Tile_Barricade_01", "Zombie Apocalypse Tileset Reference_223", Tile.ColliderType.Grid));
        createdTiles.Add(CreateTile("Tile_Barricade_02", "Zombie Apocalypse Tileset Reference_224", Tile.ColliderType.Grid));
        createdTiles.Add(CreateTile("Tile_Barricade_03", "Zombie Apocalypse Tileset Reference_231", Tile.ColliderType.Grid));
        createdTiles.Add(CreateTile("Tile_Barricade_04", "Zombie Apocalypse Tileset Reference_274", Tile.ColliderType.Grid));

        AssetDatabase.SaveAssets();

        // Update Palette Prefab
        string palettePath = "Assets/Map/ZombieApocalypsePalette.prefab";
        var paletteObj = AssetDatabase.LoadAssetAtPath<GameObject>(palettePath);
        if (paletteObj != null)
        {
            var root = PrefabUtility.LoadPrefabContents(palettePath);
            var paletteTm = root.GetComponentInChildren<Tilemap>();
            if (paletteTm != null)
            {
                paletteTm.ClearAllTiles();
                int col = 0, row = 0, tilesPerRow = 8;
                foreach (var t in createdTiles)
                {
                    paletteTm.SetTile(new Vector3Int(col, -row, 0), t);
                    col++;
                    if (col >= tilesPerRow) { col = 0; row++; }
                }
                EditorUtility.SetDirty(paletteTm);
            }
            PrefabUtility.SaveAsPrefabAsset(root, palettePath);
            PrefabUtility.UnloadPrefabContents(root);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return $"Updated {createdTiles.Count} tiles in Assets/Map/Tiles";
    }
}
