using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player Instance { get; protected set; }
    public float Speed = 5f;
    public int PickAxeSpeed= 5;
    Vector2 Movement, HitMarker;
    public Rigidbody2D rb;
    public GameObject Hitmarker;
    public Animator animator;

    public gameController Controller;


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
    void Update()
    {
        Mining();
        PickingUpthings();
    }
    private void FixedUpdate()
    {
        Move();
    }
    void Move()
    {
        Movement.x = Input.GetAxisRaw("Horizontal");
        Movement.y = Input.GetAxisRaw("Vertical");

        animator.SetFloat("Horizontal", Movement.x);
        animator.SetFloat("Vertical", Movement.y);
        animator.SetFloat("Speed", Movement.sqrMagnitude);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Movement, .55f, gameController.Instance.OreDetector);
        if (hit.collider == null)
        {
            if (Movement.x != 0 && Movement.y != 0)
            {
                rb.MovePosition(rb.position + new Vector2(Movement.x / 1.5f, Movement.y / 1.5f) * Speed * Time.fixedDeltaTime);
            }
            else
            {
                rb.MovePosition(rb.position + Movement * Speed * Time.fixedDeltaTime);
            }
        }
    }
    void Mining()
    {
        if (Movement.x != 0 || Movement.y != 0)
        {
            HitMarker = new Vector2(transform.position.x + Movement.x, transform.position.y + Movement.y);
            Hitmarker.transform.position = HitMarker;
        }
        if (Input.GetKey(KeyCode.Z))
        {
            RaycastHit2D Blockhit = Physics2D.Raycast(transform.position, new Vector3(HitMarker.x - transform.position.x, HitMarker.y - transform.position.y), 1f, Controller.OreDetector);
            if (Blockhit.collider != null)
            {
                Block currentBlock = Controller.GetBlockAt((int)Blockhit.transform.position.x, (int)Blockhit.transform.position.y);
                if (currentBlock.HitPoints > 0 && currentBlock._Type != BlockType.Bedrock)
                {
                    currentBlock.HitPoints -= PickAxeSpeed;
                    if (currentBlock.HitPoints <= 0)
                    {
                        Controller.UpdateGold(currentBlock.GoldValue);
                    }
                }
            }
            RaycastHit2D MobHit = Physics2D.Raycast(transform.position, new Vector3(HitMarker.x - transform.position.x, HitMarker.y - transform.position.y), 1f, Controller.MobDetector);
            if (MobHit.collider != null)
            {
                Boss HittedBoss = MobHit.collider.GetComponent<Boss>();
                HittedBoss.HitPoints -= 5;
            }
        }
    }
    void PickingUpthings()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D ButtonHit = Physics2D.Raycast(transform.position, new Vector3(HitMarker.x - transform.position.x, HitMarker.y - transform.position.y), 1f,Controller.ActionBlocks);
            if (ButtonHit.collider != null)
            {
                if (ButtonHit.transform.name == "StartBlock")
                {
                    UIController.Instance.LoadShop();
                    transform.GetChild(0).gameObject.SetActive(true);
                    UIController.Instance.transform.GetChild(0).gameObject.SetActive(true);
                }
                else if (ButtonHit.transform.name == "ExitBlock")
                {
                    Application.Quit();
                }
            }




        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            if(collision.tag == "NextLevel")
            {
                UIController.Instance.LoadNextLevel();
            }
            if (collision.tag == "Shop")
            {
                UIController.Instance.LoadShop();
            }
        }
    }
}
