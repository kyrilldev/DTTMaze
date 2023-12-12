using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Width and Heights")]
    public int width;
    public int height;

    [SerializeField] private TMPro.TMP_InputField _widthInputF;
    [SerializeField] private TMPro.TMP_InputField _heightInputF;

    [Header("Booleans")]
    public bool animated;
    [SerializeField] private Toggle _toggle;

    [Header("Prefabs & Lists")]
    [SerializeField] private Tile _tilePrefab;

    [SerializeField] private List<Tile> _nodes;

    [SerializeField] private TextMeshProUGUI _clamp;

    [Header("Coroutines")]

    private Coroutine _c;
    private Coroutine _c2;

    private void Start()
    {
        width = int.Parse(_widthInputF.text);
        height = int.Parse(_heightInputF.text);
        StartCoroutine(GenerateMazeCoroutine(new Vector2Int(width, height)));
    }

    private void Update()
    {
        width = int.Parse(_widthInputF.text);
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
        _widthInputF.text = width.ToString();
        _heightInputF.text = height.ToString();
    }

    /// <summary>
    /// Regulates which function is used to generate the maze
    /// </summary>
    private void AnimationToggle()
    {
        if (_toggle.isOn)
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
            _c2 ??= StartCoroutine(ShowMsg());
        }
    }

    /// <summary>
    /// Spawns the Nodes into the scene so that the algorithm can get to work
    /// </summary>
    /// <param name="size"></param>
    private void CreateNodes(Vector2Int size)
    {
        // Create _nodes
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                //calculates the node spawn pos by calculating from the center (0,0,0)
                Vector3 nodePos = new
                    (x - (size.x / 2f)//1 - (10/2) = -4
                    , 0
                    , y - (size.y / 2f));//1 - (10/2) = -4
                //NodePos = (-4,0,-4)
                //instantiate each node on the position calculated above
                Tile newNode = Instantiate(_tilePrefab, nodePos, Quaternion.identity, null);
                //add every node to list
                _nodes.Add(newNode);
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
    /// Instant version of the Algortithm implemented first. Picks node at random to start and walks till no more _nodes left. Backtracks when caught in a loop.
    /// </summary>
    /// <param name="size"></param>
    private void GenerateMaze(Vector2Int size)
    {
        _nodes = new();

        // Create _nodes
        CreateNodes(size);

        // Make lists for keeping track of node states
        List<Tile> currentPath = new();
        List<Tile> completedNodes = new();

        // Choose random starting node
        currentPath.Add(_nodes[Random.Range(0, _nodes.Count)]);
        currentPath[0].SetState(NodeState.Current);

        //till there are no more unvisited _nodes left
        while (completedNodes.Count < _nodes.Count)
        {
            // Check _nodes next to the current node
            List<int> possibleNextNodes = new();
            List<int> possibleDirections = new();

            //gets the index of the current node from the _nodes list
            int currentNodeIndex = _nodes.IndexOf(currentPath[^1]);//^1 == currentPath.Count - 1
            //get the x & y of the current node
            int currentNodeX = CalculateXPos(currentNodeIndex, size);
            int currentNodeY = CalculateYPos(currentNodeIndex ,size);

            //Check neighbour node on the right
            if (currentNodeX < size.x - 1) {
                //Check if the current node isn't already in the complete node list or currentpath list
                if (!completedNodes.Contains(_nodes[currentNodeIndex + size.y]) && !currentPath.Contains(_nodes[currentNodeIndex + size.y]))
                {
                    possibleDirections.Add(1);
                    possibleNextNodes.Add(currentNodeIndex + size.y);
                }
            }
            //Check neighbour node on the left
            if (currentNodeX > 0) {
                if (!completedNodes.Contains(_nodes[currentNodeIndex - size.y]) && !currentPath.Contains(_nodes[currentNodeIndex - size.y]))
                {
                    possibleDirections.Add(2);
                    possibleNextNodes.Add(currentNodeIndex - size.y);
                }
            }
            //Check neighbour node above current node
            if (currentNodeY < size.y - 1) {
                if (!completedNodes.Contains(_nodes[currentNodeIndex + 1]) && !currentPath.Contains(_nodes[currentNodeIndex + 1]))
                {
                    possibleDirections.Add(3);
                    possibleNextNodes.Add(currentNodeIndex + 1);
                }
            }
            //Check neighbour node under current node
            if (currentNodeY > 0) {
                if (!completedNodes.Contains(_nodes[currentNodeIndex - 1]) && !currentPath.Contains(_nodes[currentNodeIndex - 1]))
                {
                    possibleDirections.Add(4);
                    possibleNextNodes.Add(currentNodeIndex - 1);
                }
            }

            //If the algorithm has any unvisited neigbours
            if (possibleDirections.Count > 0)
            {
                int chosenDirection = Random.Range(0, possibleDirections.Count);
                Tile chosenNode = _nodes[possibleNextNodes[chosenDirection]];

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
        _nodes = new();

        // Create _nodes
        CreateNodes(size);

        // Make lists for keeping track of node states
        List<Tile> currentPath = new();
        List<Tile> completedNodes = new();

        // Choose random starting node
        currentPath.Add(_nodes[Random.Range(0, _nodes.Count)]);
        currentPath[0].SetState(NodeState.Current);

        //till there are no more unvisited _nodes left
        while (completedNodes.Count < _nodes.Count)
        {
            // Check _nodes next to the current node
            List<int> possibleNextNodes = new();
            List<int> possibleDirections = new();

            //gets the index of the current node from the _nodes list
            int currentNodeIndex = _nodes.IndexOf(currentPath[^1]);//^1 == currentPath.Count - 1
            //get the x & y of the current node
            int currentNodeX = CalculateXPos(currentNodeIndex, size);
            int currentNodeY = CalculateYPos(currentNodeIndex, size);

            //Check neighbour node on the right
            if (currentNodeX < size.x - 1 ){
                //Check if the current node isn't already in the complete node list or currentpath list
                if (!completedNodes.Contains(_nodes[currentNodeIndex + size.y]) && !currentPath.Contains(_nodes[currentNodeIndex + size.y]))
                {
                    possibleDirections.Add(1);
                    possibleNextNodes.Add(currentNodeIndex + size.y);
                }
            }
            //Check neighbour node on the left
            if (currentNodeX > 0) {
                if (!completedNodes.Contains(_nodes[currentNodeIndex - size.y]) && !currentPath.Contains(_nodes[currentNodeIndex - size.y]))
                {
                    possibleDirections.Add(2);
                    possibleNextNodes.Add(currentNodeIndex - size.y);
                }
            }
            //Check neighbour node above current node
            if (currentNodeY < size.y - 1) {
                if (!completedNodes.Contains(_nodes[currentNodeIndex + 1]) && !currentPath.Contains(_nodes[currentNodeIndex + 1]))
                {
                    possibleDirections.Add(3);
                    possibleNextNodes.Add(currentNodeIndex + 1);
                }
            }
            //Check neighbour node under current node
            if (currentNodeY > 0) {
                if (!completedNodes.Contains(_nodes[currentNodeIndex - 1]) && !currentPath.Contains(_nodes[currentNodeIndex - 1]))
                {
                    possibleDirections.Add(4);
                    possibleNextNodes.Add(currentNodeIndex - 1);
                }
            }

            //If the algorithm has any unvisited neigbours
            if (possibleDirections.Count > 0)
            {
                int chosenDirection = Random.Range(0, possibleDirections.Count);
                Tile chosenNode = _nodes[possibleNextNodes[chosenDirection]];

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

            yield return null;
        }
    }

    /// <summary>
    /// Stops the coroutine if the maze is still generating, destroy current grid, clears the list
    /// </summary>
    private void ClearMaze()
    {
        if (_c != null)
        {
            StopCoroutine(_c);
        }

        foreach (Tile node in _nodes)
        {
            Destroy(node.gameObject);
        }

        _nodes.Clear();
    }

    /// <summary>
    /// Clears everything left behind from previous maze, starts either coroutine or normal implementation
    /// </summary>
    public void Regenerate()
    {
        ClearMaze();
        if (animated)
        {
            _c = StartCoroutine(GenerateMazeCoroutine(new Vector2Int(width, height)));
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
        _clamp.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(2);

        for (int i = 0; i < 255; i++)
        {
            //lower alpha
            _clamp.alpha -= 0.004f;
            yield return new WaitForSeconds(0.01f);
        }

        _clamp.gameObject.SetActive(false);
        //set back to default
        _clamp.alpha = 1;
        _c2 = null;
    }
}
