using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowMovement : MonoBehaviour
{
    public Vector2 Move;
    public float Speed = 1;
    private float LifeTime = 10;

    public void Update()
    {
        float rotation = Mathf.Atan2(Move.y, Move.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotation);
        transform.position += new Vector3((Move.x * Time.deltaTime * Speed), (Move.y * Time.deltaTime * Speed));
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag == "EnemyMob")
        {
            collision.transform.GetComponent<Boss>().HitPoints -= Player.Instance.ArrowDamage;
            transform.gameObject.SetActive(false);
        }
        if(collision.transform.tag == "Blocks")
        {
            transform.gameObject.SetActive(false);
        }
    }
    IEnumerator Living()
    {
        while(true)
        {
            yield return new WaitForSeconds(LifeTime);
            transform.gameObject.SetActive(false);
        }
    }
}
