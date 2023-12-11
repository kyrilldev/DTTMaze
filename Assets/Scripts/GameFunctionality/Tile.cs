using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeState
{
    Available,
    Current,
    Completed
}

public class Tile : MonoBehaviour
{
    public int visited = 1;//0 yes, 1 no
    public Vector2Int pos;
    [SerializeField] GameObject[] walls;//right, left, up, down
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void RemoveWall(int wallToRemove)
    {
        if (walls[wallToRemove].gameObject)
        {
            walls[wallToRemove].SetActive(false);
        }
    }

    public void SetWallsDefault()
    {
        for (int i = 0; i < walls.Length; i++)
        {
            walls[i].SetActive(true);
        }
    }

    public void SetState(NodeState state)
    {
        switch (state)
        {
            case NodeState.Available:
                meshRenderer.material.color = Color.white;
                break;
            case NodeState.Current:
                meshRenderer.material.color = Color.yellow;
                break;
            case NodeState.Completed:
                meshRenderer.material.color = Color.blue;
                break;
        }
    }
}
