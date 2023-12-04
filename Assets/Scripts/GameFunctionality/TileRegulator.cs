using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileRegulator : MonoBehaviour
{
    public static TileRegulator instance;
    private void Awake()
    {
        if (instance == null){
            instance = this;
        }
        else{
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public void ChangeTile(Tile tile, int state)
    {
        tile.ChangeState(state);
    }
}
