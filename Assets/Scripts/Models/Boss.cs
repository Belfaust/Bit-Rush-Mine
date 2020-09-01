using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    private int HP = 10;
    public int HitPoints { get => HP; set { int OldHP = HP; HP = value; HPCheck(); } }
    public int MaxEnergy = 100;
    public static int Energy = 0;
    public bool Cooldown = false;
    public float Speed;
    public Vector3 Dest;


    public virtual void Ability1()
    {

    }
    public virtual void Ability2()
    {

    }
    public virtual void Ability3()
    {

    }
    public void HPCheck()
    {
        if(HitPoints < 1)
        {
            transform.gameObject.SetActive(false);
            //Death anim
        }
    }


}
