using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public (int x, int y) position;

    // Start is called before the first frame update
    void Start()
    {
        GetComponentInChildren<TextMesh>().text = position.x.ToString() + "," + position.y.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
