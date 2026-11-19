using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class BuildMap
{
    private const float CELL_SIZE = 0.75f;

    // Grid Dimensions
    // Width: 40 cells (-20 to 19) => 30.0 units
    // Height: 52 cells (-26 to 25) => 39.0 units
    private const int MIN_CX = -20;
    private const int MAX_CX = 19;
    private const int MIN_CY = -26;
    private const int MAX_CY = 25;

    // Ground extension: 2 cells beyond fence
    private const int GROUND_MIN_CX = -22;
    private const int GROUND_MAX_CX = 21;
    private const int GROUND_MIN_CY = -28;
    private const int GROUND_MAX_CY = 27;

    // Clear positions
    private static readonly Vector2 PLAYER_POS = new Vector2(0f, 0f);
    private const float PLAYER_CLEAR_RADIUS = 3.2f;

    private static readonly Vector2[] SPAWNER_POSITIONS = new Vector2[]
    {
        new Vector2(4.81f, -1.26f),   // Collectable Spawner
        new Vector2(0.07f, -6.57f),   // Strong Enemy Spawner (South)
        new Vector2(3.94f, 6.25f),    // EnemySpawner (North-East)
        new Vector2(-10.33f, 0.04f),  // EnemySpawner (2) (West)
        new Vector2(9.96f, -0.19f),   // EnemySpawner (3) (East)
        new Vector2(-3.74f, 6.87f),   // BossSpawner (North-West)
        new Vector2(-4.48f, 5.97f)    // Strong Enemy Spawner (1) (North-West)
    };
    private const float SPAWNER_CLEAR_RADIUS = 2.3f; // > 2.0 required

    private static bool IsCellInClearZone(int cx, int cy)
    {
        // Cell center world pos
        Vector2 worldPos = new Vector2((cx + 0.5f) * CELL_SIZE, (cy + 0.5f) * CELL_SIZE);

        // Player check
        if (Vector2.Distance(worldPos, PLAYER_POS) < PLAYER_CLEAR_RADIUS)
        {
            return true;
        }

        // Spawners check
        foreach (var sp in SPAWNER_POSITIONS)
        {
            if (Vector2.Distance(worldPos, sp) < SPAWNER_CLEAR_RADIUS)
            {
                return true;
            }
        }

        return false;
    }

    public static string Execute()
    {
        // 1. Load Tiles
        var tiles = LoadTiles();

        // 2. Prepare Grid and Tilemaps in Scene
        var gridGo = GameObject.Find("Grid");
        if (gridGo == null)
        {
            gridGo = new GameObject("Grid");
            Undo.RegisterCreatedObjectUndo(gridGo, "Create Grid");
        }
        var grid = gridGo.GetComponent<Grid>();
        if (grid == null) grid = gridGo.AddComponent<Grid>();
        grid.cellSize = new Vector3(CELL_SIZE, CELL_SIZE, 1f);

        // Ensure Ground child
        Transform groundT = gridGo.transform.Find("Ground");
        GameObject groundGo;
        if (groundT == null)
        {
            // Check if old "Tilemap" exists and rename it
            Transform oldTm = gridGo.transform.Find("Tilemap");
            if (oldTm != null)
            {
                groundGo = oldTm.gameObject;
                groundGo.name = "Ground";
            }
            else
            {
                groundGo = new GameObject("Ground");
                groundGo.transform.SetParent(gridGo.transform, false);
            }
        }
        else
        {
            groundGo = groundT.gameObject;
        }

        var groundTm = groundGo.GetComponent<Tilemap>();
        if (groundTm == null) groundTm = groundGo.AddComponent<Tilemap>();
        var groundTr = groundGo.GetComponent<TilemapRenderer>();
        if (groundTr == null) groundTr = groundGo.AddComponent<TilemapRenderer>();
        groundTr.sortingLayerName = "Background";
        groundTr.sortingOrder = 0;
        // Ground has NO colliders
        var groundCol = groundGo.GetComponent<Collider2D>();
        if (groundCol != null) UnityEngine.Object.DestroyImmediate(groundCol);

        // Ensure Walls child
        Transform wallsT = gridGo.transform.Find("Walls");
        GameObject wallsGo;
        if (wallsT == null)
        {
            wallsGo = new GameObject("Walls");
            wallsGo.transform.SetParent(gridGo.transform, false);
        }
        else
        {
            wallsGo = wallsT.gameObject;
        }

        var wallsTm = wallsGo.GetComponent<Tilemap>();
        if (wallsTm == null) wallsTm = wallsGo.AddComponent<Tilemap>();
        var wallsTr = wallsGo.GetComponent<TilemapRenderer>();
        if (wallsTr == null) wallsTr = wallsGo.AddComponent<TilemapRenderer>();
        wallsTr.sortingLayerName = "Background";
        wallsTr.sortingOrder = 5;

        // Physics 2D components on Walls:
        // TilemapCollider2D (usedByComposite = true) + CompositeCollider2D + Rigidbody2D (Static)
        var tilemapCollider = wallsGo.GetComponent<TilemapCollider2D>();
        if (tilemapCollider == null) tilemapCollider = wallsGo.AddComponent<TilemapCollider2D>();
        tilemapCollider.usedByComposite = true;

        var compositeCollider = wallsGo.GetComponent<CompositeCollider2D>();
        if (compositeCollider == null) compositeCollider = wallsGo.AddComponent<CompositeCollider2D>();
        compositeCollider.geometryType = CompositeCollider2D.GeometryType.Outlines;

        var rb2d = wallsGo.GetComponent<Rigidbody2D>();
        if (rb2d != null)
        {
            rb2d.bodyType = RigidbodyType2D.Static;
        }

        // Clear existing tiles
        groundTm.ClearAllTiles();
        wallsTm.ClearAllTiles();

        // 3. Build Wall Layout Matrix
        bool[,] wallGrid = new bool[MAX_CX - MIN_CX + 1, MAX_CY - MIN_CY + 1];
        int width = MAX_CX - MIN_CX + 1;
        int height = MAX_CY - MIN_CY + 1;

        void SetWall(int cx, int cy, bool isWall)
        {
            if (cx < MIN_CX || cx > MAX_CX || cy < MIN_CY || cy > MAX_CY) return;
            if (isWall && IsCellInClearZone(cx, cy)) return; // Strict safety
            wallGrid[cx - MIN_CX, cy - MIN_CY] = isWall;
        }

        // 3a. Perimeter Fence
        for (int cx = MIN_CX; cx <= MAX_CX; cx++)
        {
            SetWall(cx, MIN_CY, true);
            SetWall(cx, MAX_CY, true);
        }
        for (int cy = MIN_CY; cy <= MAX_CY; cy++)
        {
            SetWall(MIN_CX, cy, true);
            SetWall(MAX_CX, cy, true);
        }

        // 3b. Buildings & Obstacles
        // Building 1: North-West Military Depot (cx: -16 to -11, cy: 11 to 16)
        for (int cx = -16; cx <= -11; cx++)
            for (int cy = 11; cy <= 16; cy++)
                SetWall(cx, cy, true);

        // Building 2: North-East Gas Station / Market (cx: 10 to 15, cy: 11 to 16)
        for (int cx = 10; cx <= 15; cx++)
            for (int cy = 11; cy <= 16; cy++)
                SetWall(cx, cy, true);

        // Building 3: South-West Residential Block (cx: -16 to -10, cy: -17 to -11)
        for (int cx = -16; cx <= -10; cx++)
            for (int cy = -17; cy <= -11; cy++)
                SetWall(cx, cy, true);

        // Building 4: South-East Industrial Complex (cx: 10 to 15, cy: -17 to -11)
        for (int cx = 10; cx <= 15; cx++)
            for (int cy = -17; cy <= -11; cy++)
                SetWall(cx, cy, true);

        // Barrier 1: North Barricade / Police Checkpoint (cy: 20..21, cx: -5..-2 and 2..5)
        for (int cx = -6; cx <= -3; cx++)
            for (int cy = 20; cy <= 21; cy++)
                SetWall(cx, cy, true);
        for (int cx = 3; cx <= 6; cx++)
            for (int cy = 20; cy <= 21; cy++)
                SetWall(cx, cy, true);

        // Barrier 2: South Barricade / Checkpoint (cy: -22..-21, cx: -6..-3 and 3..6)
        for (int cx = -6; cx <= -3; cx++)
            for (int cy = -22; cy <= -21; cy++)
                SetWall(cx, cy, true);
        for (int cx = 3; cx <= 6; cx++)
            for (int cy = -22; cy <= -21; cy++)
                SetWall(cx, cy, true);

        // Barrier 3: Mid-West Alley Rubble (cx: -16..-14, cy: -4..-2)
        for (int cx = -16; cx <= -14; cx++)
            for (int cy = -4; cy <= -2; cy++)
                SetWall(cx, cy, true);

        // Barrier 4: Mid-East Alley Rubble (cx: 14..16, cy: -4..-2)
        for (int cx = 14; cx <= 16; cx++)
            for (int cy = -4; cy <= -2; cy++)
                SetWall(cx, cy, true);

        // Small Cover 5: Yard fences / bushes in South-West (cx: -7..-5, cy: -6..-5)
        for (int cx = -7; cx <= -5; cx++)
            for (int cy = -6; cy <= -5; cy++)
                SetWall(cx, cy, true);

        // Small Cover 6: Yard fences / bushes in South-East (cx: 5..7, cy: -8..-7)
        for (int cx = 5; cx <= 7; cx++)
            for (int cy = -8; cy <= -7; cy++)
                SetWall(cx, cy, true);

        // Small Cover 7: North-West perimeter bunker (cx: -10..-8, cy: 19..20)
        for (int cx = -10; cx <= -8; cx++)
            for (int cy = 19; cy <= 20; cy++)
                SetWall(cx, cy, true);

        // Small Cover 8: North-East perimeter bunker (cx: 8..10, cy: 19..20)
        for (int cx = 8; cx <= 10; cx++)
            for (int cy = 19; cy <= 20; cy++)
                SetWall(cx, cy, true);

        // 4. Verify Connectivity via Flood Fill
        bool[,] visited = new bool[width, height];
        int startX = 0 - MIN_CX;
        int startY = 0 - MIN_CY;
        var queue = new Queue<Vector2Int>();
        queue.Enqueue(new Vector2Int(startX, startY));
        visited[startX, startY] = true;
        int reachableCount = 0;

        int[] dx = { 0, 0, 1, -1 };
        int[] dy = { 1, -1, 0, 0 };

        while (queue.Count > 0)
        {
            var cur = queue.Dequeue();
            reachableCount++;

            for (int i = 0; i < 4; i++)
            {
                int nx = cur.x + dx[i];
                int ny = cur.y + dy[i];
                if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                {
                    if (!visited[nx, ny] && !wallGrid[nx, ny])
                    {
                        visited[nx, ny] = true;
                        queue.Enqueue(new Vector2Int(nx, ny));
                    }
                }
            }
        }

        // Check for any unreachable non-wall cell
        int unreachableCount = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!wallGrid[x, y] && !visited[x, y])
                {
                    unreachableCount++;
                    // Remove isolated cell by converting to wall or opening it
                    wallGrid[x, y] = true;
                }
            }
        }

        // 5. Paint Tiles to Tilemaps
        // 5a. Ground Tilemap (Extends from GROUND_MIN to GROUND_MAX)
        for (int cx = GROUND_MIN_CX; cx <= GROUND_MAX_CX; cx++)
        {
            for (int cy = GROUND_MIN_CY; cy <= GROUND_MAX_CY; cy++)
            {
                var pos = new Vector3Int(cx, cy, 0);

                // Is it on a main road?
                // North-South Avenue: cx in [-2..1]
                // East-West Street: cy in [-2..1]
                bool isNS = (cx >= -2 && cx <= 1);
                bool isEW = (cy >= -2 && cy <= 1);

                if (isNS || isEW)
                {
                    // Road
                    if (isNS && (cx == -1 || cx == 0) && !isEW && (cy % 3 == 0))
                    {
                        groundTm.SetTile(pos, tiles["Tile_RoadLine_V"]);
                    }
                    else if (isEW && (cy == -1 || cy == 0) && !isNS && (cx % 3 == 0))
                    {
                        groundTm.SetTile(pos, tiles["Tile_RoadLine_H"]);
                    }
                    else
                    {
                        int r = Mathf.Abs((cx * 7 + cy * 13) % 4);
                        groundTm.SetTile(pos, tiles[$"Tile_Road_0{r + 1}"]);
                    }
                }
                else if (cx >= -4 && cx <= 3 && cy >= -4 && cy <= 3)
                {
                    // Central intersection pavement
                    int r = Mathf.Abs((cx * 11 + cy * 17) % 6);
                    groundTm.SetTile(pos, tiles[$"Tile_Pavement_0{r + 1}"]);
                }
                else
                {
                    // Sidewalk near roads, otherwise wasteland dirt
                    bool nearRoad = (Math.Abs(cx) <= 3 || Math.Abs(cy) <= 3);
                    if (nearRoad)
                    {
                        int r = Mathf.Abs((cx * 5 + cy * 7) % 6);
                        groundTm.SetTile(pos, tiles[$"Tile_Pavement_0{r + 1}"]);
                    }
                    else
                    {
                        int r = Mathf.Abs((cx * 3 + cy * 9) % 6);
                        groundTm.SetTile(pos, tiles[$"Tile_Dirt_0{r + 1}"]);
                    }
                }
            }
        }

        // Add blood decals around spawner locations and crossroads
        foreach (var sp in SPAWNER_POSITIONS)
        {
            int scx = Mathf.FloorToInt(sp.x / CELL_SIZE);
            int scy = Mathf.FloorToInt(sp.y / CELL_SIZE);
            for (int ox = -1; ox <= 1; ox++)
            {
                for (int oy = -1; oy <= 1; oy++)
                {
                    if ((ox + oy) % 2 == 0)
                    {
                        int bloodIdx = (Math.Abs(scx + ox * 3 + scy + oy * 5) % 9) + 1;
                        groundTm.SetTile(new Vector3Int(scx + ox, scy + oy, 0), tiles[$"Tile_Blood_0{bloodIdx}"]);
                    }
                }
            }
        }

        // 5b. Wall Tilemap
        for (int cx = MIN_CX; cx <= MAX_CX; cx++)
        {
            for (int cy = MIN_CY; cy <= MAX_CY; cy++)
            {
                if (wallGrid[cx - MIN_CX, cy - MIN_CY])
                {
                    var pos = new Vector3Int(cx, cy, 0);

                    // Perimeter fence
                    if (cx == MIN_CX || cx == MAX_CX || cy == MIN_CY || cy == MAX_CY)
                    {
                        if (cx == MIN_CX || cx == MAX_CX)
                        {
                            groundTm.SetTile(pos, tiles["Tile_Pavement_01"]); // pavement under fence
                            wallsTm.SetTile(pos, tiles["Tile_Fence_Wire_02"]);
                        }
                        else
                        {
                            groundTm.SetTile(pos, tiles["Tile_Pavement_01"]);
                            wallsTm.SetTile(pos, tiles["Tile_Fence_Wire_01"]);
                        }
                    }
                    else
                    {
                        // Check if it's top row of a building (Roof Trim)
                        bool isBuildingTop = (cy + 1 <= MAX_CY && !wallGrid[cx - MIN_CX, cy + 1 - MIN_CY]);
                        bool isBuildingBottom = (cy - 1 >= MIN_CY && !wallGrid[cx - MIN_CX, cy - 1 - MIN_CY]);

                        // Check obstacle types
                        if (cy == 20 || cy == 21 || cy == -22 || cy == -21)
                        {
                            // Barricade checkpoint
                            int bIdx = (Math.Abs(cx) % 4) + 1;
                            wallsTm.SetTile(pos, tiles[$"Tile_Barricade_0{bIdx}"]);
                        }
                        else if (cx >= -7 && cx <= -5 && cy >= -6 && cy <= -5)
                        {
                            // Bush cluster
                            int bIdx = (Math.Abs(cx + cy) % 3) + 1;
                            wallsTm.SetTile(pos, tiles[$"Tile_Bush_0{bIdx}"]);
                        }
                        else if (cx >= 5 && cx <= 7 && cy >= -8 && cy <= -7)
                        {
                            // Bush cluster
                            int bIdx = (Math.Abs(cx * 2 + cy) % 3) + 1;
                            wallsTm.SetTile(pos, tiles[$"Tile_Bush_0{bIdx}"]);
                        }
                        else if (isBuildingTop)
                        {
                            int r = (Math.Abs(cx) % 4) + 1;
                            wallsTm.SetTile(pos, tiles[$"Tile_Wall_RoofTrim_0{r}"]);
                        }
                        else if (isBuildingBottom)
                        {
                            int r = (Math.Abs(cx) % 4) + 1;
                            wallsTm.SetTile(pos, tiles[$"Tile_Wall_Brick_0{r}"]);
                        }
                        else
                        {
                            int r = (Math.Abs(cx + cy) % 3) + 1;
                            wallsTm.SetTile(pos, tiles[$"Tile_Wall_Window_0{r}"]);
                        }
                    }
                }
            }
        }

        // 6. Deactivate old Background GameObject
        var bg = GameObject.Find("Background");
        if (bg != null)
        {
            bg.SetActive(false);
            EditorUtility.SetDirty(bg);
        }

        // 7. Update ArenaBounds
        var abGo = GameObject.Find("ArenaBounds");
        if (abGo != null)
        {
            var comp = abGo.GetComponent("ArenaBounds");
            if (comp != null)
            {
                var so = new SerializedObject(comp);
                var minProp = so.FindProperty("_min");
                var maxProp = so.FindProperty("_max");
                if (minProp != null && maxProp != null)
                {
                    // Inside the perimeter fence (-15.0..15.0, -19.5..19.5)
                    minProp.vector2Value = new Vector2(-14.0f, -18.5f);
                    maxProp.vector2Value = new Vector2(14.0f, 18.5f);
                    so.ApplyModifiedProperties();
                    EditorUtility.SetDirty(comp);
                }
            }
        }

        // 8. Mark Scene Dirty and Save
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());

        return $"BuildMap Success! Reachable floor cells: {reachableCount}, Unreachable cleaned: {unreachableCount}. Ground tiles: {groundTm.GetUsedTilesCount()}, Wall tiles: {wallsTm.GetUsedTilesCount()}. ArenaBounds: (-14.0, -18.5) to (14.0, 18.5).";
    }

    private static Dictionary<string, Tile> LoadTiles()
    {
        var dict = new Dictionary<string, Tile>();
        var guids = AssetDatabase.FindAssets("t:Tile", new[] { "Assets/Map/Tiles" });
        foreach (var g in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(g);
            var tile = AssetDatabase.LoadAssetAtPath<Tile>(path);
            if (tile != null)
            {
                dict[tile.name] = tile;
            }
        }
        return dict;
    }
}
