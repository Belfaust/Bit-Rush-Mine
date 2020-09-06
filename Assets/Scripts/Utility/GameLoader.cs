using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameLoader : MonoBehaviour
{
    public void Start()
    {
        SceneManager.LoadScene("StartLocation");
    }
}
