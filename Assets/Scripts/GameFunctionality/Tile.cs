using UnityEngine;

public enum NodeState
{
    Available,//not visited yet
    Current,//where the algorithm currently is
    Completed//cells which aren't going to change
}

public class Tile : MonoBehaviour
{
    [SerializeField] GameObject[] walls;//right, left, up, down
    private MeshRenderer meshRenderer;//for colors

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    /// <summary>
    /// Function called when forming path to delete walls that interfere with the path direction
    /// </summary>
    /// <param name="wallToRemove"></param>
    public void RemoveWall(int wallToRemove)
    {
        if (walls[wallToRemove].gameObject)
        {
            walls[wallToRemove].SetActive(false);
        }
    }

    /// <summary>
    /// Function to set states/colors of the cell to indicate where the algorithm is currently working
    /// </summary>
    /// <param name="state"></param>
    public void SetState(NodeState state)
    {
        switch (state) {
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
