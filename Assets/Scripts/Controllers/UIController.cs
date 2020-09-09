using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; protected set; }
    public GameObject HealthContainers,HeartPrefab;
    public List<GameObject> Hearts;
    public AudioClip[] Music;
    public TextMeshProUGUI Weapon;
    public TextMeshProUGUI Time;
    public TextMeshProUGUI GoldText;
    public bool CanShoot = false;

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
        if(SceneManager.GetActiveScene().name == "TransitionScene")
        {
            LoadStartingLocation();
        }
    }
    public void PlayerHealthCheck()
    {
        int HP = Player.Instance.Health;
        if (HP != Hearts.Count)
        {
            for(int i = 1;i < (HP > Hearts.Count ? (HP+1) : Hearts.Count);i++)
            {
                //if(i >= Hearts.Count)
                //{
                //    Debug.Log((i * 8) - 64);
                //   // Hearts.Add(Instantiate(HeartPrefab,new Vector3((i*8)-64,70,0),Quaternion.identity, HealthContainers.transform));
                //   // Hearts.Add(Instantiate(HeartPrefab,new Vector3(0,0,0),Quaternion.identity, HealthContainers.transform));
                //}

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
        Time.text = (TimeValue-1).ToString();
    }
    public void LoadNextLevel()
    {
        StartCoroutine("CameraTransition");
        Player.Instance.animator.SetBool("Shop",false);


        if (Player.Instance.GetComponent<AudioSource>().clip != Music[1])
        {
            Player.Instance.GetComponent<AudioSource>().clip = Music[1];
            Player.Instance.GetComponent<AudioSource>().Play();
        }
        gameController GameContr = gameController.Instance;
        Player.Instance.animator.Play("Idle");
        CanShoot = true;
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
        Destroy(Player.Instance.gameObject);
        Destroy(gameController.Instance.gameObject);
        Destroy(CameraController.Instance.gameObject);
        Destroy(gameObject);

    }
    public void LoadShop()
    {
        Player.Instance.transform.position = new Vector3(14, 13, 0);
        SceneManager.LoadScene("Shop");
        CanShoot = false;
        if (Player.Instance.GetComponent<AudioSource>().clip != Music[0])
        {
            Player.Instance.GetComponent<AudioSource>().clip = Music[0];
            Player.Instance.GetComponent<AudioSource>().Play();
        }
    }
    public void LoadStartingLocation()
    {
        StartCoroutine("CameraTransition");
        Player.Instance.animator.Play("Idle Shop");
        Player.Instance.animator.SetBool("Shop", true);
        CanShoot = false;

        SceneManager.sceneLoaded -= GenerateLevel;
        SceneManager.LoadScene("StartLocation");
        if (Player.Instance.GetComponent<AudioSource>().clip != Music[0])
        {
            Player.Instance.GetComponent<AudioSource>().clip = Music[0];
            Player.Instance.GetComponent<AudioSource>().Play();
        }
        Player.Instance.transform.position = new Vector3(14, 12, 0);
        Time.gameObject.SetActive(false);
        Player.Instance.HitMarker.SetActive(false);
        GoldText.transform.parent.gameObject.SetActive(false);
        CameraController.Instance.IndicatorOn = false;
    }
    public void LoadMenu()
    {
        SceneManager.sceneLoaded -= GenerateLevel;
        SceneManager.LoadScene("Menu");
        Destroy(Player.Instance.gameObject);
        Destroy(gameController.Instance.gameObject);
        Destroy(CameraController.Instance.gameObject);
        Destroy(gameObject);
    }
    private void GenerateLevel(Scene Scene, LoadSceneMode mode)
    {
        gameController.Instance.GenerateLevel(60, 90);
        Player.Instance.DestPoint.gameObject.transform.position = Player.Instance.transform.position;
        CameraController.Instance.transform.position = Player.Instance.transform.position - new Vector3(0,0,10);
        CameraController.Instance.IndicatorOn = true;
        Player.Instance.HitMarker.SetActive(true);
        GoldText.transform.parent.gameObject.SetActive(true);
        Time.gameObject.SetActive(true);
    }
    private void LoadPlayerDest(Scene Scene,LoadSceneMode mode)
    {
        GameObject DestPoint = new GameObject("DestPoint");
        DestPoint.transform.position = Player.Instance.transform.position;
        Player.Instance.DestPoint = DestPoint.transform;
    }
    public IEnumerator CameraTransition()
    {
        while(true)
        {
            CameraController.Instance.gameObject.SetActive(false);
            yield return new WaitForSeconds(.1f);
            CameraController.Instance.gameObject.SetActive(true);
            break;
        }
    }
}
