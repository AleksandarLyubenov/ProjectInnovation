using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostSpawner : MonoBehaviour
{
    [SerializeField] private GameObject ghost;

    [SerializeField] private GameObject spawner;

    private Vector2 ghostPos;

    /*private int randomSpawner;*/

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //SpawnGhost();
    }

    /*void SpawnGhost()
    {
        spawner.transform.position = ghostPos;
        if (GameObject.FindGameObjectsWithTag("Ghost").Length == 0) 
        {
            Debug.Log("RAH");
            Instantiate(ghost, spawner.transform.position, Quaternion.identity);
        }
    }*/
}
