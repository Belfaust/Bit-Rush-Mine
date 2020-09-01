using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; protected set; }
    void Start()
    {
        if (Instance != null)
        {
            Debug.Log("Err there are 2 instances of gameControllers");
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    public void LoadNextLevel()
    {
        SceneManager.LoadScene("LVL" + (gameController.Instance.LevelIndex+1), LoadSceneMode.Single);
        gameController.Instance.GenerateLevel(60,90);
    }
    public void LoadShop()
    {
        SceneManager.LoadScene("Shop", LoadSceneMode.Single);
    }

}
