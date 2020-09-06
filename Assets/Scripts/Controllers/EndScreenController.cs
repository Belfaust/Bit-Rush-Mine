using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class EndScreenController : MonoBehaviour
{
    public Vector2 Dest;
    void Update()
    {
        moveTo();
        if(Input.GetKeyDown(KeyCode.X))
        {
            SceneManager.LoadScene("Menu");
        }
    }
    void moveTo()
    {
        if (Vector2.Distance(transform.position, Dest) > .5f)
        {
            transform.position -= new Vector3(0, 5 * Time.deltaTime, 0);
        }
    }
}
