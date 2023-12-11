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
}
