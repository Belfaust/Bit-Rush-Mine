using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            if (Player.Instance.Indestrucitble == false)
                Player.Instance.Health -= 1;
            transform.gameObject.SetActive(false);
        }
        if (collision.transform.tag == "Blocks")
        {
            if (gameController.Instance.GetBlockAt((int)collision.transform.position.x, (int)collision.transform.position.y)._Type != BlockType.Bedrock)
                gameController.Instance.GetBlockAt((int)collision.transform.position.x, (int)collision.transform.position.y).HitPoints -= 100;
            transform.gameObject.SetActive(false);
        }
    }

}
