using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int width;
    public int height;

    [SerializeField] private List<GameObject> Tiles;

    [SerializeField] private Tile tilePrefab;
    [SerializeField] private float timeSpeed;

    [SerializeField] private TMPro.TMP_InputField width2;
    [SerializeField] private TMPro.TMP_InputField height2;

    [SerializeField] List<Tile> nodes;

    public bool animated;
    [SerializeField] private Toggle Toggle;

    [SerializeField] private TextMeshProUGUI clamp;

    private Coroutine c;
    private Coroutine c2;

    private void Start()
    {
        width = int.Parse(width2.text);
        height = int.Parse(height2.text);
        StartCoroutine(GenerateMazeCoroutine(new Vector2Int(width, height)));
    }

    private void Update()
    {
        width = int.Parse(width2.text);
        height = int.Parse(height2.text);

        if (height > 100 || width > 100) 
        {
            c2 ??= StartCoroutine(ShowMsg());
        }

        height = Mathf.Clamp(height,0, 100);
        width = Mathf.Clamp(width, 0, 100);
        width2.text = width.ToString();
        height2.text = height.ToString();


        if (Toggle.isOn)
        {
            animated = true;
        }
        else
        {
            animated= false;
        }
    }

    private void CreateNodes(Vector2Int size)
    {
        // Create nodes
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3 nodePos = new Vector3(x - (size.x / 2f), 0, y - (size.y / 2f));
                Tile newNode = Instantiate(tilePrefab, nodePos, Quaternion.identity, transform);
                nodes.Add(newNode);
            }
        }
    }

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

            int currentNodeIndex = nodes.IndexOf(currentPath[currentPath.Count - 1]);
            //get the x & y of the current node
            int currentNodeX = currentNodeIndex / size.y;
            int currentNodeY = currentNodeIndex % size.y;

            //Check neighbour node on the right
            if (currentNodeX < size.x - 1)
            {
                //Check if the current node isn't already in the complete node list or currentpath list
                if (!completedNodes.Contains(nodes[currentNodeIndex + size.y]) && !currentPath.Contains(nodes[currentNodeIndex + size.y]))
                {
                    possibleDirections.Add(1);
                    possibleNextNodes.Add(currentNodeIndex + size.y);
                }
            }
            if (currentNodeX > 0)
            {
                // Check node to the left of the current node
                if (!completedNodes.Contains(nodes[currentNodeIndex - size.y]) && !currentPath.Contains(nodes[currentNodeIndex - size.y]))
                {
                    possibleDirections.Add(2);
                    possibleNextNodes.Add(currentNodeIndex - size.y);
                }
            }
            if (currentNodeY < size.y - 1)
            {
                // Check node above the current node
                if (!completedNodes.Contains(nodes[currentNodeIndex + 1]) && !currentPath.Contains(nodes[currentNodeIndex + 1]))
                {
                    possibleDirections.Add(3);
                    possibleNextNodes.Add(currentNodeIndex + 1);
                }
            }
            if (currentNodeY > 0)
            {
                // Check node below the current node
                if (!completedNodes.Contains(nodes[currentNodeIndex - 1]) && !currentPath.Contains(nodes[currentNodeIndex - 1]))
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
                Tile newNode = Instantiate(tilePrefab, nodePos, Quaternion.identity, transform);
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

    private IEnumerator ShowMsg()
    {
        //show msg
        clamp.gameObject.SetActive(true);
        Debug.Log("gets here");
        
        yield return new WaitForSeconds(2);

        for (int i = 0; i < 255; i++)
        {
            clamp.alpha -= 0.004f;
            yield return new WaitForSeconds(0.01f);
        }

        clamp.gameObject.SetActive(false);
        clamp.alpha = 1;
        c2 = null;
    }
}
