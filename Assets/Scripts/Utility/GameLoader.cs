using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameLoader : MonoBehaviour
{
    public void Start()
    {
        UIController.Instance.LoadStartingLocation();
    }
}
