using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstBoss : Boss
{
    private Transform PlayerPos;
    private bool Attacking = false;
    private Vector2[] Directions = new Vector2[] {new Vector2(1,1), new Vector2(0, 1), new Vector2(-1, 1), new Vector2(-1, 0), new Vector2(-1, -1), new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, 0)};
    private Animator Animat;
    public int ProjectileDamage = 100;
    public GameObject Projectile;
    private void Awake()
    {
         PlayerPos = Player.Instance.transform;
        Dest = new Vector3(Random.Range(5, gameController.Instance.Width - 5), Random.Range(5, gameController.Instance.Height - 5), 0);
        Animat = transform.gameObject.GetComponent<Animator>();
    }
    void Update()
    {
        if (Vector2.Distance(transform.position, PlayerPos.position) > 10)
        {
            TraverseRandomly();
        }
        else
        {
            AttackPlayer();
        }
    }
    void TraverseRandomly()
    {
        if (Vector2.Distance(transform.position, Dest) > 5f)
        {
            Move(Dest);
        }
        else
        {
            Vector3 TempDest = new Vector3(Random.Range(5, gameController.Instance.Width - 5), Random.Range(5, gameController.Instance.Height - 5), 0);
            while (Vector2.Distance(TempDest, Dest) < 20)
            {
                TempDest = new Vector3(Random.Range(5, gameController.Instance.Width - 5), Random.Range(5, gameController.Instance.Height - 5), 0);
            }
            Dest = TempDest;
        }
    }
    void AttackPlayer()
    {
        Speed = 2.5f;
        if (Attacking == false)
        {
            Move(PlayerPos.position);
        }
        if (Vector2.Distance(transform.position, PlayerPos.position) > 5&&Attacking== false)
        {
            Animat.SetTrigger("Attack");
            StartCoroutine(AttackN1());
            Attacking = true;
            int AttackPattern = Random.Range(0,2);

            //    if(AttackPattern == 0)
            //        {
            //            AttackN1();
            //        }
            //    else if(AttackPattern == 1)
            //{
            //    AttackN2();
            //}
        }
    }
    void Move(Vector3 Destination)
    {
        if (Energy > 0)
        {
            Vector3 Dir = new Vector3((Destination.x - transform.position.x), (Destination.y - transform.position.y));
            Dir.Normalize();
            transform.Translate(new Vector3(Dir.x * Speed * Time.deltaTime, Dir.y * Speed * Time.deltaTime));
            Energy -= 1;
        }
        else
        {
            if (Cooldown == false)
            {
                StartCoroutine("EnergyReplenishment");
            }
        }
    }
    IEnumerator EnergyReplenishment()
    {
        while(true)
        {
            yield return new WaitForSeconds(2);
            Energy = MaxEnergy;
            //MaxEnergy = (int)(MaxEnergy *0.1f);
            Cooldown = true;
            yield return new WaitForSeconds(5);
            Cooldown = false;
            break;
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.transform.tag == "Blocks")
        {
            if (gameController.Instance.GetBlockAt((int)collision.transform.position.x, (int)collision.transform.position.y)._Type != BlockType.Bedrock)
            gameController.Instance.GetBlockAt((int)collision.transform.position.x, (int)collision.transform.position.y).HitPoints -= 10;
        }
        if(collision.transform.tag == "Player")
        {
            if(Player.Instance.Indestrucitble == false)
            Player.Instance.Health -= 1;
        }
    }
    IEnumerator AttackN1()
    {
        while (true)
        {
            for (int j = 0; j < 2; j++)
            {
                for (int i = 0; i < Directions.Length; i++)
                {
                    GameObject projectile = Instantiate(Projectile, transform.position, Quaternion.identity);
                    ShootProjectiles(projectile, Directions[i]*100);
                    yield return new WaitForSeconds(.2f);
                }
            }
            yield return new WaitForSeconds(3f);
            Attacking = false;
            break;
        }
    }
    IEnumerator AttackN2()
    {
        while(true)
        {

        }
    }
    void ShootProjectiles(GameObject Shots,Vector2 Force)
    {
        Rigidbody2D rb = Shots.GetComponent<Rigidbody2D>();
        rb.AddForce(Force);
    }

}
