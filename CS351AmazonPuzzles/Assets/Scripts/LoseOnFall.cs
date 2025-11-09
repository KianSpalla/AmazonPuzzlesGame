using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseOnFall : MonoBehaviour
{
    public float lowestY = -10f;

    void Update()
    {
        if (transform.position.y < lowestY)
        {
            //Set position to (0,0,0) and reload the scene 
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
}
