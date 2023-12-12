using UnityEngine;

public enum NodeState
{
    Available,//not visited yet
    Current,//where the algorithm currently is
    Completed//cells which aren't going to change
}

public class Tile : MonoBehaviour
{
    [SerializeField] private GameObject[] _walls;//right, left, up, down
    private MeshRenderer _meshRenderer;//for colors

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    /// <summary>
    /// Function called when forming path to delete _walls that interfere with the path direction
    /// </summary>
    /// <param name="wallToRemove"></param>
    public void RemoveWall(int wallToRemove)
    {
        if (_walls[wallToRemove])
        {
            _walls[wallToRemove].SetActive(false);
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
                _meshRenderer.material.color = Color.white;
                break;
            case NodeState.Current:
                _meshRenderer.material.color = Color.yellow;
                break;
            case NodeState.Completed:
                _meshRenderer.material.color = Color.blue;
                break;
        }
    }
}
