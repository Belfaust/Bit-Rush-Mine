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
    public Transform DestPoint;
    public Rigidbody2D rb;
    public GameObject Hitmarker;
    public Animator animator;
    public Sprite Crosshair;
    bool HoldingBlock = false;
    public LayerMask MovementCollider;
    BlockType PickedBlockType = BlockType.Empty;

    public gameController Controller;


    void Start()
    {
        DestPoint.parent = null;
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
        transform.position = Vector3.MoveTowards(transform.position, DestPoint.position, Speed * Time.deltaTime);

        animator.SetFloat("Horizontal", Movement.x);
        animator.SetFloat("Vertical", Movement.y);
        animator.SetFloat("Speed", Movement.sqrMagnitude);

        if(Vector2.Distance(transform.position, DestPoint.position) <= 0.05f)
            {
                if (Mathf.Abs(Movement.x) == 1)
                {
                    if (!Physics2D.OverlapCircle(DestPoint.position + new Vector3(Movement.x, 0, 0), .2f, MovementCollider))
                    {
                        DestPoint.position += new Vector3(Movement.x, 0, 0);
                    }
                }
                if(Mathf.Abs(Movement.y) == 1)
                {
                    if (!Physics2D.OverlapCircle(DestPoint.position + new Vector3(0, Movement.y, 0), .2f, MovementCollider))
                    {
                        DestPoint.position += new Vector3(0, Movement.y, 0);
                    }
                }
            }
    }
    void Mining()
    {
        Hitmarker.transform.position = new Vector3((int)HitMarker.x, (int)HitMarker.y, 0);
        if (Mathf.Abs(Movement.x) == 1 || Mathf.Abs(Movement.y) == 1)
        {
            HitMarker = new Vector2((int)DestPoint.position.x + (int)Movement.x, (int)DestPoint.position.y + (int)Movement.y);
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
            RaycastHit2D OreHit = Physics2D.Raycast(transform.position, new Vector3(HitMarker.x - transform.position.x, HitMarker.y - transform.position.y), 1f, Controller.OreDetector);
            if(OreHit.collider != null && HoldingBlock == false)
            {
                PickedBlockType = Controller.GetBlockAt((int)OreHit.transform.position.x, (int)OreHit.transform.position.y)._Type;
                Block TempBlock = Controller.GetBlockAt((int)OreHit.transform.position.x, (int)OreHit.transform.position.y);
                if (PickedBlockType != BlockType.Bedrock)
                {
                    Hitmarker.SetActive(true);
                    Hitmarker.GetComponent<SpriteRenderer>().sprite = TempBlock.MyGameObject.GetComponent<SpriteRenderer>().sprite;
                    TempBlock.HitPoints = 0;
                    TempBlock.ChangeType(BlockType.Empty);
                    HoldingBlock = true;
                }
            }
            if(OreHit.collider == null && HoldingBlock == true)
            {
                Block PlacedBlock = Controller.GetBlockAt((int)Hitmarker.transform.position.x, (int)Hitmarker.transform.position.y);
                if(PlacedBlock._Type == BlockType.Empty)
                {
                    PlacedBlock.ChangeType(PickedBlockType);
                    Controller.CheckForNeighbourBlocks(PlacedBlock);
                    PlacedBlock.MyGameObject.GetComponent<BoxCollider2D>().enabled = true;
                    HoldingBlock = false;
                    Hitmarker.GetComponent<SpriteRenderer>().sprite = Crosshair;
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
