using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
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
        SceneManager.LoadScene("LVL" + (gameController.Instance.LevelIndex+1));
        gameController.Instance.GenerateLevel(60,90);
        GameObject DestPoint = new GameObject();
        DestPoint.transform.position = Player.Instance.transform.position;
        Player.Instance.DestPoint = DestPoint.transform;
    }
    public void LoadShop()
    {
        SceneManager.sceneLoaded += LoadPlayerDest;
        SceneManager.LoadScene("Shop");

    }
    void LoadPlayerDest(Scene Scene,LoadSceneMode mode)
    {
        GameObject DestPoint = new GameObject("DestPoint");
        Debug.Log(DestPoint.transform.position);
        DestPoint.transform.position = Player.Instance.transform.position;
        Player.Instance.DestPoint = DestPoint.transform;
    }

}
