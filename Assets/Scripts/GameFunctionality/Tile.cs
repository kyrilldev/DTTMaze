using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int visited = 1;//0 yes, 1 no
    public Vector2Int pos;
    private MeshRenderer meshRenderer;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        ChangeColor(visited);
    }

    public void ChangeState(int state)
    {
        if (visited != state)
        {
            visited = state;
        }

        ChangeColor(state);
    }

    private void ChangeColor(int state)
    {
        if (state == 1)
        {
            meshRenderer.material.color = Color.black;
        }
        else
        {
            meshRenderer.material.color = Color.white;
        }
    }
}
