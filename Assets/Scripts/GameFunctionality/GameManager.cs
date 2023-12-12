using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Drawing;

public class GameManager : MonoBehaviour
{
    public int width;
    public int height;

    [SerializeField] private Tile _tilePrefab;

    [SerializeField] private TMPro.TMP_InputField _width2InputF;
    [SerializeField] private TMPro.TMP_InputField _heightInputF;

    [SerializeField] private List<Tile> nodes;

    public bool animated;
    [SerializeField] private Toggle Toggle;

    [SerializeField] private TextMeshProUGUI clamp;

    private Coroutine c;
    private Coroutine c2;

    private void Start()
    {
        width = int.Parse(_width2InputF.text);
        height = int.Parse(_heightInputF.text);
        StartCoroutine(GenerateMazeCoroutine(new Vector2Int(width, height)));
    }

    private void Update()
    {
        width = int.Parse(_width2InputF.text);
        height = int.Parse(_heightInputF.text);

        DisplayWarning();
        SetStrings();
        AnimationToggle();
    }

    /// <summary>
    /// Clamps height and width values, set inputfields to width and height
    /// </summary>
    private void SetStrings()
    {
        height = Mathf.Clamp(height, 0, 100);
        width = Mathf.Clamp(width, 0, 100);
        _width2InputF.text = width.ToString();
        _heightInputF.text = height.ToString();
    }

    /// <summary>
    /// Regulates which function is used to generate the maze
    /// </summary>
    private void AnimationToggle()
    {
        if (Toggle.isOn)
            animated = true;
        else
            animated = false;
    }

    /// <summary>
    /// Displays a warning to the user when going over 100 in size on any axis. Because of performance issues.
    /// </summary>
    private void DisplayWarning()
    {
        if (height > 100 || width > 100)
        {
            c2 ??= StartCoroutine(ShowMsg());
        }
    }

    /// <summary>
    /// Spawns the Nodes into the scene so that the algorithm can get to work
    /// </summary>
    /// <param name="size"></param>
    private void CreateNodes(Vector2Int size)
    {
        // Create nodes
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3 nodePos = new(x - (size.x / 2f), 0, y - (size.y / 2f));
                Debug.Log(nodePos);
                //instantiate each node on the position calculated above
                Tile newNode = Instantiate(_tilePrefab, nodePos, Quaternion.identity, null);
                //add every node to list
                nodes.Add(newNode);
            }
        }
    }

    /// <summary>
    /// Gives back X position of a cell in a grid given grid size and index of cell
    /// </summary>
    /// <param name="index"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    private int CalculateXPos(int index, Vector2Int size)
    {
        return index / size.y;
    }

    /// <summary>
    /// Gives back Y position of a cell in a grid given grid size and index of cell
    /// </summary>
    /// <param name="index"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    private int CalculateYPos(int index, Vector2Int size)
    {
        return index % size.y;
    }

    /// <summary>
    /// Instant version of the Algortithm implemented first. Picks node at random to start and walks till no more nodes left. Backtracks when caught in a loop.
    /// </summary>
    /// <param name="size"></param>
    private void GenerateMaze(Vector2Int size)
    {
        nodes = new();

        // Create nodes
        CreateNodes(size);

        // Make lists for keeping track of node states
        List<Tile> currentPath = new();
        List<Tile> completedNodes = new();

        // Choose random starting node
        currentPath.Add(nodes[Random.Range(0, nodes.Count)]);
        currentPath[0].SetState(NodeState.Current);

        //till there are no more unvisited nodes left
        while (completedNodes.Count < nodes.Count)
        {
            // Check nodes next to the current node
            List<int> possibleNextNodes = new();
            List<int> possibleDirections = new();

            //gets the index of the current node from the nodes list
            int currentNodeIndex = nodes.IndexOf(currentPath[^1]);//^1 == currentPath.Count - 1
            //get the x & y of the current node
            int currentNodeX = CalculateXPos(currentNodeIndex, size);
            int currentNodeY = CalculateYPos(currentNodeIndex ,size);

            //Check neighbour node on the right
            if (currentNodeX < size.x - 1) {
                //Check if the current node isn't already in the complete node list or currentpath list
                if (!completedNodes.Contains(nodes[currentNodeIndex + size.y]) && !currentPath.Contains(nodes[currentNodeIndex + size.y]))
                {
                    possibleDirections.Add(1);
                    possibleNextNodes.Add(currentNodeIndex + size.y);
                }
            }
            //Check neighbour node on the left
            if (currentNodeX > 0) {
                if (!completedNodes.Contains(nodes[currentNodeIndex - size.y]) && !currentPath.Contains(nodes[currentNodeIndex - size.y]))
                {
                    possibleDirections.Add(2);
                    possibleNextNodes.Add(currentNodeIndex - size.y);
                }
            }
            //Check neighbour node above current node
            if (currentNodeY < size.y - 1) {
                if (!completedNodes.Contains(nodes[currentNodeIndex + 1]) && !currentPath.Contains(nodes[currentNodeIndex + 1]))
                {
                    possibleDirections.Add(3);
                    possibleNextNodes.Add(currentNodeIndex + 1);
                }
            }
            //Check neighbour node under current node
            if (currentNodeY > 0) {
                if (!completedNodes.Contains(nodes[currentNodeIndex - 1]) && !currentPath.Contains(nodes[currentNodeIndex - 1]))
                {
                    possibleDirections.Add(4);
                    possibleNextNodes.Add(currentNodeIndex - 1);
                }
            }

            //If the algorithm has any unvisited neigbours
            if (possibleDirections.Count > 0)
            {
                int chosenDirection = Random.Range(0, possibleDirections.Count);
                Tile chosenNode = nodes[possibleNextNodes[chosenDirection]];

                //Remove wall on the side the next chosen cell is going to be
                switch (possibleDirections[chosenDirection])
                {
                    case 1://right
                        //remove wall on current node
                        chosenNode.RemoveWall(0);
                        //remove wall on next node
                        currentPath[^1].RemoveWall(1);
                        break;
                    case 2://left
                        chosenNode.RemoveWall(1);
                        currentPath[^1].RemoveWall(0);
                        break;
                    case 3://up
                        chosenNode.RemoveWall(3);
                        currentPath[^1].RemoveWall(2);
                        break;
                    case 4://down
                        chosenNode.RemoveWall(2);
                        currentPath[^1].RemoveWall(3);
                        break;
                }

                //add neighbor node chosen to path list
                currentPath.Add(chosenNode);
                //change the state/color of the chosenNode to yellow
                chosenNode.SetState(NodeState.Current);
            }
            //if there are no more neighbours avaiable that are unvisited
            else
            {
                //add node to completed list
                completedNodes.Add(currentPath[^1]);

                //set state/color to blue to mark cell as completer (will not visited anymore)
                currentPath[^1].SetState(NodeState.Completed);
                //remove the completed node
                currentPath.RemoveAt(currentPath.Count - 1);
            }
        }
    }

    IEnumerator GenerateMazeCoroutine(Vector2Int size)
    {
        nodes = new List<Tile>();

        // Create nodes
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3 nodePos = new Vector3(x - (size.x / 2f), 0, y - (size.y / 2f));
                Tile newNode = Instantiate(_tilePrefab, nodePos, Quaternion.identity, transform);
                nodes.Add(newNode);
            }
        }

        List<Tile> currentPath = new List<Tile>();
        List<Tile> completedNodes = new List<Tile>();

        // Choose starting node
        currentPath.Add(nodes[Random.Range(0, nodes.Count)]);
        currentPath[0].SetState(NodeState.Current);

        while (completedNodes.Count < nodes.Count)
        {
            // Check nodes next to the current node
            List<int> possibleNextNodes = new List<int>();
            List<int> possibleDirections = new List<int>();

            int currentNodeIndex = nodes.IndexOf(currentPath[currentPath.Count - 1]);
            int currentNodeX = currentNodeIndex / size.y;
            int currentNodeY = currentNodeIndex % size.y;

            if (currentNodeX < size.x - 1)
            {
                // Check node to the right of the current node
                if (!completedNodes.Contains(nodes[currentNodeIndex + size.y]) &&
                    !currentPath.Contains(nodes[currentNodeIndex + size.y]))
                {
                    possibleDirections.Add(1);
                    possibleNextNodes.Add(currentNodeIndex + size.y);
                }
            }
            if (currentNodeX > 0)
            {
                // Check node to the left of the current node
                if (!completedNodes.Contains(nodes[currentNodeIndex - size.y]) &&
                    !currentPath.Contains(nodes[currentNodeIndex - size.y]))
                {
                    possibleDirections.Add(2);
                    possibleNextNodes.Add(currentNodeIndex - size.y);
                }
            }
            if (currentNodeY < size.y - 1)
            {
                // Check node above the current node
                if (!completedNodes.Contains(nodes[currentNodeIndex + 1]) &&
                    !currentPath.Contains(nodes[currentNodeIndex + 1]))
                {
                    possibleDirections.Add(3);
                    possibleNextNodes.Add(currentNodeIndex + 1);
                }
            }
            if (currentNodeY > 0)
            {
                // Check node below the current node
                if (!completedNodes.Contains(nodes[currentNodeIndex - 1]) &&
                    !currentPath.Contains(nodes[currentNodeIndex - 1]))
                {
                    possibleDirections.Add(4);
                    possibleNextNodes.Add(currentNodeIndex - 1);
                }
            }

            // Choose next node
            if (possibleDirections.Count > 0)
            {
                int chosenDirection = Random.Range(0, possibleDirections.Count);
                Tile chosenNode = nodes[possibleNextNodes[chosenDirection]];

                switch (possibleDirections[chosenDirection])
                {
                    case 1://right
                        chosenNode.RemoveWall(0);
                        currentPath[currentPath.Count - 1].RemoveWall(1);
                        break;
                    case 2://left
                        chosenNode.RemoveWall(1);
                        currentPath[currentPath.Count - 1].RemoveWall(0);
                        break;
                    case 3://up
                        chosenNode.RemoveWall(3);
                        currentPath[currentPath.Count - 1].RemoveWall(2);
                        break;
                    case 4://down
                        chosenNode.RemoveWall(2);
                        currentPath[currentPath.Count - 1].RemoveWall(3);
                        break;
                }

                currentPath.Add(chosenNode);
                chosenNode.SetState(NodeState.Current);
            }
            else
            {
                completedNodes.Add(currentPath[currentPath.Count - 1]);

                currentPath[currentPath.Count - 1].SetState(NodeState.Completed);
                currentPath.RemoveAt(currentPath.Count - 1);
            }

            yield return null;
        }
    }

    /// <summary>
    /// Stops the coroutine if the maze is still generating, destroy current grid, clears the list
    /// </summary>
    private void ClearMaze()
    {
        if (c != null)
        {
            StopCoroutine(c);
        }

        foreach (Tile node in nodes)
        {
            Destroy(node.gameObject);
        }

        nodes.Clear();
    }

    /// <summary>
    /// Clears everything left behind from previous maze, starts either coroutine or normal implementation
    /// </summary>
    public void Regenerate()
    {
        ClearMaze();
        if (animated)
        {
            c = StartCoroutine(GenerateMazeCoroutine(new Vector2Int(width, height)));
        }
        else
        {
            GenerateMaze(new Vector2Int(width, height));
        }
    }

    /// <summary>
    /// Shows message that notifies the player that they cannot have more than 100 on either axis
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShowMsg()
    {
        clamp.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(2);

        for (int i = 0; i < 255; i++)
        {
            //lower alpha
            clamp.alpha -= 0.004f;
            yield return new WaitForSeconds(0.01f);
        }

        clamp.gameObject.SetActive(false);
        //set back to default
        clamp.alpha = 1;
        c2 = null;
    }
}
