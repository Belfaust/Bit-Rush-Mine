using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; protected set; }
    public int Hp;
    public int Health { get => Hp;set { int oldhp = Hp;Hp = value; HealthCheck();}}
    public float Speed = 5f,AttackRate = 0.75f;
    public int PickAxeStrength= 5,ArrowDamage = 25,TimeLimit = 90;
    public Transform DestPoint;
    public Rigidbody2D rb;
    public GameObject HitMarker,ArrowPrefab;
    public Animator animator;
    public LayerMask MovementCollider;
    public gameController Controller;
    public Sprite Crosshair;
    public bool Indestrucitble = false;
    private bool HoldingBlock = false, AttackCooldown = false, WeaponSwitch = false;
    private Vector2 Movement;



    void Awake()
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
        SwitchingWeapons();
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
        if (Movement.x != 0 || Movement.y != 0)
        {
            HitMarker.transform.localPosition = new Vector2((int)Movement.x, (int)Movement.y);
        }
        if(Input.GetKey(KeyCode.Z)&&AttackCooldown == false)
        {
            if (WeaponSwitch == false)
            {
                RaycastHit2D Blockhit = Physics2D.Raycast(transform.position, new Vector3(HitMarker.transform.position.x - transform.position.x, HitMarker.transform.position.y - transform.position.y), 1f, Controller.OreDetector);
                if (Blockhit.collider != null)
                {
                    Block currentBlock = Controller.GetBlockAt((int)Blockhit.transform.position.x, (int)Blockhit.transform.position.y);
                    if (currentBlock.HitPoints > 0 && currentBlock._Type != BlockType.Bedrock)
                    {
                        currentBlock.HitPoints -= PickAxeStrength;
                        if (currentBlock.HitPoints <= 0)
                        {
                            UIController.Instance.UpdateGold(currentBlock.GoldValue);
                        }
                    }
                }
                RaycastHit2D MobHit = Physics2D.Raycast(transform.position, new Vector3(HitMarker.transform.position.x - transform.position.x, HitMarker.transform.position.y - transform.position.y), 1f, Controller.MobDetector);
                if (MobHit.collider != null)
                {
                    Boss HittedBoss = MobHit.collider.GetComponent<Boss>();
                    HittedBoss.HitPoints -= 5;
                }
                StartCoroutine("AttackCD");

            }
            else
            {
                Vector3 Dir = HitMarker.transform.localPosition;
                GameObject arrow = Instantiate(ArrowPrefab,transform.position + Dir,Quaternion.identity);
                arrow.GetComponent<ArrowMovement>().Move = Dir;
                StartCoroutine("AttackCD");
            }
        }
    }
    private BlockType PickedBlockType = BlockType.Empty;
    void PickingUpthings()
    {
        if(Input.GetKeyDown(KeyCode.X)&&WeaponSwitch == false)
        {
            RaycastHit2D ButtonHit = Physics2D.Raycast(transform.position, new Vector3(HitMarker.transform.position.x - transform.position.x, HitMarker.transform.position.y - transform.position.y), 1f,Controller.ActionBlocks);
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
                else if(ButtonHit.transform.tag == "NextLevel")
                {
                    UIController.Instance.LoadNextLevel();
                }
            }
            RaycastHit2D OreHit = Physics2D.Raycast(transform.position, new Vector3(HitMarker.transform.position.x - transform.position.x, HitMarker.transform.position.y - transform.position.y), 1f, Controller.OreDetector);
            if(OreHit.collider != null && HoldingBlock == false)
            {
                PickedBlockType = Controller.GetBlockAt((int)OreHit.transform.position.x, (int)OreHit.transform.position.y)._Type;
                Block TempBlock = Controller.GetBlockAt((int)OreHit.transform.position.x, (int)OreHit.transform.position.y);
                if (PickedBlockType != BlockType.Bedrock)
                {
                    HitMarker.GetComponent<SpriteRenderer>().sprite = TempBlock.MyGameObject.GetComponent<SpriteRenderer>().sprite;
                    TempBlock.HitPoints = 0;
                    TempBlock.ChangeType(BlockType.Empty);
                    HoldingBlock = true;
                }
            }
            if(OreHit.collider == null && HoldingBlock == true)
            {
                Block PlacedBlock = Controller.GetBlockAt((int)HitMarker.transform.position.x, (int)HitMarker.transform.position.y);
                if(PlacedBlock._Type == BlockType.Empty)
                {
                    if (Vector2.Distance(PlacedBlock.MyGameObject.transform.position, DestPoint.transform.position) > .25f)
                    {
                        PlacedBlock.ChangeType(PickedBlockType);
                        Controller.CheckForNeighbourBlocks(PlacedBlock);
                        PlacedBlock.MyGameObject.GetComponent<BoxCollider2D>().enabled = true;
                        HoldingBlock = false;
                        HitMarker.GetComponent<SpriteRenderer>().sprite = Crosshair;
                    }
                    else
                    {
                        DestPoint.transform.position = new Vector3((int)transform.position.x, (int)transform.position.y);
                        PlacedBlock.ChangeType(PickedBlockType);
                        Controller.CheckForNeighbourBlocks(PlacedBlock);
                        PlacedBlock.MyGameObject.GetComponent<BoxCollider2D>().enabled = true;
                        HoldingBlock = false;
                        HitMarker.GetComponent<SpriteRenderer>().sprite = Crosshair;
                    }
                }
            }
        }
    }
    void SwitchingWeapons()
    {
        if(Input.GetKeyDown(KeyCode.Space)&&HoldingBlock == false)
        {
            if(WeaponSwitch == false)
            {
                WeaponSwitch = true;
            }
            else
            {
                WeaponSwitch = false;
            }
        }
    }
    void HealthCheck()
    {
        if (Hp > 0)
        {
            Indestrucitble = true;
            UIController.Instance.PlayerHealthCheck();
            StartCoroutine("IndectructibilityFrames");
        }
        else
        {
            UIController.Instance.LoadEndScreen();
            StopAllCoroutines();
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision != null)
        {
         //   Debug.Log(collision.transform.name);
        }
    }
    IEnumerator AttackCD()
    {
        while (true)
        {
            AttackCooldown = true;
            yield return new WaitForSeconds(AttackRate);
            AttackCooldown = false;
            break;
        }
    }
    IEnumerator IndectructibilityFrames()
    {
        while (true)
        {
            yield return new WaitForSeconds(3);
            Indestrucitble = false;
            break;
        }
    }
}
