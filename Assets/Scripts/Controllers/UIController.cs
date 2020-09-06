using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; protected set; }
    public GameObject HealthContainers,HeartPrefab;
    private List<GameObject> Hearts = new List<GameObject>();
    public AudioClip[] Music;
    public GameObject WeaponSlots;
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
        Player.Instance.Controller = gameController.Instance;
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
                    Hearts.Add(Instantiate(HeartPrefab,new Vector3((i*8)+3,142,0),Quaternion.identity, HealthContainers.transform));
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
        gameController.Instance.HighScore += goldAmount;
        GoldText.text = gameController.Instance.Gold.ToString();
    }
    public void UpdateGold()
    {
        GoldText.text = gameController.Instance.Gold.ToString();
    }
    public void UpdateTime(int TimeValue)
    {
        Time.text = TimeValue.ToString();
    }
    public void LoadNextLevel()
    {
        if (Player.Instance.GetComponent<AudioSource>().clip != Music[1])
        {
            Player.Instance.GetComponent<AudioSource>().clip = Music[1];
            Player.Instance.GetComponent<AudioSource>().Play();
        }
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
        Player.Instance.transform.position = new Vector3(14, 13, 0);
        SceneManager.LoadScene("Shop");
        if (Player.Instance.GetComponent<AudioSource>().clip != Music[0])
        {
            Player.Instance.GetComponent<AudioSource>().clip = Music[0];
            Player.Instance.GetComponent<AudioSource>().Play();
        }
    }
    public void LoadStartingLocation()
    {
        SceneManager.sceneLoaded -= GenerateLevel;
        SceneManager.LoadScene("StartLocation");
        if (Player.Instance.GetComponent<AudioSource>().clip != Music[0])
        {
            Player.Instance.GetComponent<AudioSource>().clip = Music[0];
            Player.Instance.GetComponent<AudioSource>().Play();
        }
        Player.Instance.transform.position = new Vector3(14, 12, 0);
        Time.gameObject.SetActive(false);
        CameraController.Instance.IndicatorOn = false;
    }
    void GenerateLevel(Scene Scene, LoadSceneMode mode)
    {
        gameController.Instance.GenerateLevel(60, 90);
        Player.Instance.DestPoint.gameObject.transform.position = Player.Instance.transform.position;
        CameraController.Instance.transform.position = Player.Instance.transform.position - new Vector3(0,0,10);
        CameraController.Instance.IndicatorOn = true;
        Time.gameObject.SetActive(true);
    }
    void LoadPlayerDest(Scene Scene,LoadSceneMode mode)
    {
        GameObject DestPoint = new GameObject("DestPoint");
        DestPoint.transform.position = Player.Instance.transform.position;
        Player.Instance.DestPoint = DestPoint.transform;
    }

}
