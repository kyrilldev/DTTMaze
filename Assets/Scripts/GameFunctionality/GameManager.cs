using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int width;
    public int height;

    //1 == unvisited && 0 == visited
    private int[,] maze;

    [SerializeField] private List<GameObject> Tiles;

    [SerializeField] private GameObject Tile;

    private void Start()
    {
        GenerateMaze();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            var tile = height + width;
            TileRegulator.instance.ChangeTile(Tiles[tile].GetComponent<Tile>(), 0);
        }
    }

    private void GenerateMaze()
    {
        //Tiles = new List<GameObject>();

        //for (int x = 0; x < width; x++)
        //{
        //    for (int y = 0; y < height; y++)
        //    {
        //        GameObject temp = Instantiate(Tile, new Vector3(x, 0, y), Quaternion.identity);
        //        Tiles.Add(temp);
        //        temp.name += Tiles.Count;
        //    }
        //}

        maze = new int[width, height];
        List<Vector2Int> unvisitedCells = new();

        // Initialize maze and unvisitedCells
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                maze[x, y] = 1; // 1 represents unvisited cells
                unvisitedCells.Add(new Vector2Int(x, y));
            }
        }

        // Randomly select a starting point
        Vector2Int startCell = unvisitedCells[Random.Range(0, unvisitedCells.Count)];
        maze[startCell.x, startCell.y] = 0; // 0 represents visited cells
        unvisitedCells.Remove(startCell);

        while (unvisitedCells.Count > 0)
        {
            // Randomly select a cell from unvisitedCells
            Vector2Int currentCell = unvisitedCells[Random.Range(0, unvisitedCells.Count)];

            List<Vector2Int> randomPath = new List<Vector2Int>
            {
                currentCell
            };

            // Continue random walk until we reach a visited cell
            while (maze[currentCell.x, currentCell.y] == 1)
            {
                List<Vector2Int> neighbors = GetNeighbors(currentCell);
                currentCell = neighbors[Random.Range(0, neighbors.Count)];
                randomPath.Add(currentCell);
            }

            // Carve the path in the maze
            for (int i = 0; i < randomPath.Count - 1; i++)
            {
                Vector2Int from = randomPath[i];
                Vector2Int to = randomPath[i + 1];
                maze[(from.x + to.x) / 2, (from.y + to.y) / 2] = 0; // Carve a path between two adjacent cells
            }

            // Mark currentCell as visited
            maze[currentCell.x, currentCell.y] = 0;
            unvisitedCells.Remove(currentCell);
        }

        // Now, 'maze' contains the generated maze (0 for paths, 1 for walls)
        // You can visualize or use this maze data as needed.
    }

    List<Vector2Int> GetNeighbors(Vector2Int cell)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        // Add neighboring cells that are within the bounds of the maze
        if (cell.x > 1) neighbors.Add(new Vector2Int(cell.x - 2, cell.y));
        if (cell.x < width - 2) neighbors.Add(new Vector2Int(cell.x + 2, cell.y));
        if (cell.y > 1) neighbors.Add(new Vector2Int(cell.x, cell.y - 2));
        if (cell.y < height - 2) neighbors.Add(new Vector2Int(cell.x, cell.y + 2));

        return neighbors;
    }
}
