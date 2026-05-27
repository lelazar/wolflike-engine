// This loader gives us validation, readable error messages, and entity spawn support

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WolfLike.src.World;

public class LevelLoader
{
    public LevelData LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Level file was not found: {filePath}");

        string[] rawLines = File.ReadAllLines(filePath);

        List<string> mapLines = rawLines
            .Select(line => line.Trim())
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Where(line => !line.StartsWith("#"))
            .ToList();

        if (mapLines.Count == 0)
            throw new InvalidOperationException($"Level file contains no map data: {filePath}");

        int width = mapLines[0].Length;
        int height = mapLines.Count;

        ValidateMapWidth(mapLines, width, filePath);

        LevelData levelData = new LevelData
        {
            Tiles = new int[height, width],
            PlayerStartPosition = new Vector2(1.5f, 1.5f)
        };

        bool hasPlayerStart = false;

        for (int y = 0; y < height; y++)
        {
            string line = mapLines[y];

            for (int x = 0; x < width; x++)
            {
                char symbol = line[x];

                ParseSymbol(symbol, x, y, levelData, ref hasPlayerStart);
            }
        }

        if (!hasPlayerStart)
            throw new InvalidOperationException($"Level file does not contain a player start symbol 'P': {filePath}");

        return levelData;
    }

    private void ValidateMapWidth(List<string> mapLines, int expectedWidth, string filePath)
    {
        for (int y = 0; y < mapLines.Count; y++)
            if (mapLines[y].Length != expectedWidth)
                throw new InvalidOperationException(
                        $"Invalid map width in {filePath} at row {y}. " +
                        $"Expected {expectedWidth}, found {mapLines[y].Length}");
    }

    private void ParseSymbol(char symbol, int x, int y, LevelData levelData, ref bool hasPlayerStart)
    {
        switch (symbol)
        {
            case '0':
                levelData.Tiles[y, x] = 0;
                break;

            case '1':
                levelData.Tiles[y, x] = 1;
                break;

            case '2':
                levelData.Tiles[y, x] = 2;
                break;

            case '3':
                levelData.Tiles[y, x] = 3;
                break;

            case 'P':
                levelData.Tiles[y, x] = 0;
                levelData.PlayerStartPosition = ToCenteredWorldPosition(x, y);
                hasPlayerStart = true;
                break;

            case 'E':
                levelData.Tiles[y, x] = 0;
                levelData.EntitySpawns.Add(
                        new LevelEntitySpawn(ToCenteredWorldPosition(x, y), LevelEntityType.StandardEnemy)
                    );
                break;

            case 'T':
                levelData.Tiles[y, x] = 0;
                levelData.EntitySpawns.Add(
                    new LevelEntitySpawn(
                        ToCenteredWorldPosition(x, y),
                        LevelEntityType.ToughEnemy
                    )
                );
                break;

            case 'F':
                levelData.Tiles[y, x] = 0;
                levelData.EntitySpawns.Add(
                    new LevelEntitySpawn(
                        ToCenteredWorldPosition(x, y),
                        LevelEntityType.FastEnemy
                    )
                );
                break;

            case 'H':
                levelData.Tiles[y, x] = 0;
                levelData.EntitySpawns.Add(
                    new LevelEntitySpawn(
                        ToCenteredWorldPosition(x, y),
                        LevelEntityType.PickupPlaceholder
                    )
                );
                break;

            default:
                throw new InvalidOperationException(
                        $"Unsupported level symbol '{symbol}' at X={x}, Y={y}."
                    );
        }
    }

    private Vector2 ToCenteredWorldPosition(int x, int y) => new Vector2(x + 0.5f, y + 0.5f);
}
