using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; protected set; }
    public GameObject HealthContainers,HeartPrefab;
    private List<GameObject> Hearts = new List<GameObject>();
    public Text Time;
    public Text GoldText;

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
        SceneManager.sceneLoaded += LoadPlayerDest;
        UpdateGold(0);
        PlayerHealthCheck();
    }
    public void PlayerHealthCheck()
    {
        int HP = Player.Instance.Health;
        if (HP != Hearts.Count)
        {
            for(int i = 1;i < (HP > Hearts.Count ? (HP+1) : Hearts.Count);i++)
            {
                if(i >= Hearts.Count)
                {
                    Hearts.Add(Instantiate(HeartPrefab,new Vector3((i*15)-15,144,0),Quaternion.identity, HealthContainers.transform));
                }
                else
                {
                    if (i+1 > HP)
                    {
                        Hearts[i].SetActive(false);
                    }
                    else
                    {
                        Hearts[i].SetActive(true);
                    }
                }
            }
        }
    }
    public void UpdateGold(int goldAmount)
    {
        gameController.Instance.Gold += goldAmount;
        GoldText.text = "Gold : " + gameController.Instance.Gold.ToString();
    }
    public void UpdateTime(int TimeValue)
    {
        Time.text = TimeValue.ToString();
    }
    public void LoadNextLevel()
    {
        gameController GameContr = gameController.Instance;
        SceneManager.sceneLoaded -= GenerateLevel;
        SceneManager.sceneLoaded += GenerateLevel;
        if (GameContr.LevelIndex < GameContr.LevelBoss.Length)
        {
            SceneManager.LoadScene("LVL" + (gameController.Instance.LevelIndex + 1));
        }
        else
        {
            LoadEndScreen();
        }
    }
    public void LoadEndScreen()
    {
        SceneManager.sceneLoaded -= GenerateLevel;
        SceneManager.LoadScene("EndScreen");
    }
    public void LoadShop()
    {
        SceneManager.sceneLoaded -= GenerateLevel;
        SceneManager.LoadScene("Shop");
    }
    void GenerateLevel(Scene Scene, LoadSceneMode mode)
    {
        gameController.Instance.GenerateLevel(60, 90);
        Player.Instance.DestPoint.gameObject.transform.position = Player.Instance.transform.position;
        Debug.Log(Player.Instance.DestPoint.transform.position + " " + Player.Instance.transform.position);

    }
    void LoadPlayerDest(Scene Scene,LoadSceneMode mode)
    {
        GameObject DestPoint = new GameObject("DestPoint");
        DestPoint.transform.position = Player.Instance.transform.position;
        Player.Instance.DestPoint = DestPoint.transform;
    }

}
