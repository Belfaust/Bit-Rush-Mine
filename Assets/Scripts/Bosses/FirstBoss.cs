using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstBoss : Boss
{
    private Transform PlayerPos;
    private void Awake()
    {
         PlayerPos = Player.Instance.transform;
        Dest = new Vector3(Random.Range(5, gameController.Instance.Width - 5), Random.Range(5, gameController.Instance.Height - 5), 0);
    }
    void Update()
    {
        if (Vector2.Distance(transform.position, PlayerPos.position) > 15)
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
        Move(PlayerPos.position);
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
    }



}
