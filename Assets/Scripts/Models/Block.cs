using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum BlockType {Empty,Stone,Coal,Iron,Gold,Bedrock};
public class Block
{
    public BlockType _Type = BlockType.Empty;
    public int X { get; protected set; }
    public int Y { get; protected set; }
    private int HP = 10;
    public int HitPoints { get =>HP; set{ int OldHP = HP; HP = value;HPCheck(); } }
    public int GoldValue = 1;
    private int maxhp;
    public GameObject MyGameObject;

    public Block()
    {

    }
    public Block(int x, int y,GameObject blockGameObject, BlockType blockType)
    {
        this.X = x;
        this.Y = y;
        this.MyGameObject = blockGameObject;
        this._Type = blockType;
    }
    public Block(int x, int y, BlockType blockType)
    {
        this.X = x;
        this.Y = y;
        this._Type = blockType;
    }
    public void HPCheck()
    {
        if(MyGameObject.TryGetComponent<Animator>(out Animator anim) == false)
        {
            MyGameObject.AddComponent<Animator>();
            if(_Type == BlockType.Stone)
            {
                MyGameObject.GetComponent<Animator>().runtimeAnimatorController = gameController.Instance.OresAnim[0];
            }
            else if (_Type == BlockType.Coal)
            {
                MyGameObject.GetComponent<Animator>().runtimeAnimatorController = gameController.Instance.OresAnim[1];
            }
            else if (_Type == BlockType.Iron)
            {
                MyGameObject.GetComponent<Animator>().runtimeAnimatorController = gameController.Instance.OresAnim[2];
            }
            else if (_Type == BlockType.Gold)
            {
                MyGameObject.GetComponent<Animator>().runtimeAnimatorController = gameController.Instance.OresAnim[3];
            }
        }
        if (MyGameObject.GetComponent<Animator>().runtimeAnimatorController != null)
        {
                MyGameObject.GetComponent<Animator>().SetFloat("Stage", 1 - ((float)HP / (float)maxhp));
        }
        if (HitPoints < 1&&_Type != BlockType.Empty )
        {
            MyGameObject.GetComponent<BoxCollider2D>().enabled = false;
            MyGameObject.GetComponent<Animator>().enabled = false;
            ChangeType(BlockType.Empty);
            MyGameObject.GetComponent<SpriteRenderer>().sprite = gameController.Instance.GrassSprites[Random.Range(0,3)];
        }

    }
    public void ChangeType(BlockType ChangedType)
    {
        _Type = ChangedType;
        if(ChangedType == BlockType.Empty)
        {
            HP = 0;
            maxhp = 0;
        }
        else if(ChangedType == BlockType.Stone)
        {
            HP = 100;
            GoldValue = 10;
            maxhp = 100;

        }
        else if(ChangedType == BlockType.Coal)
        {
            HP = 150;
            GoldValue = 25;
            maxhp = 150;

        }
        else if(ChangedType == BlockType.Iron)
        {
            HP = 200;
            GoldValue = 50;
            maxhp = 200;

        }
        else if(ChangedType == BlockType.Gold)
        {
            HP = 300;
            GoldValue = 150;
            maxhp = 300;
        }

        if (_Type != BlockType.Bedrock)
        {
            gameController Controller = gameController.Instance;
            if (Controller.BlockList[X + 1, Y]._Type != BlockType.Empty && Controller.BlockList[X + 1, Y].X != Controller.Width - 1)
                Controller.CheckForNeighbourBlocks(Controller.BlockList[X + 1, Y]);
            if (Controller.BlockList[X, Y + 1]._Type != BlockType.Empty && Controller.BlockList[X, Y + 1].Y != Controller.Height - 1)
                Controller.CheckForNeighbourBlocks(Controller.BlockList[X, Y + 1]);
            if (Controller.BlockList[X - 1, Y]._Type != BlockType.Empty && Controller.BlockList[X - 1, Y].X < 1)
                Controller.CheckForNeighbourBlocks(Controller.BlockList[X - 1, Y]);
            if (Controller.BlockList[X, Y - 1]._Type != BlockType.Empty && Controller.BlockList[X, Y - 1].Y < 1)
                Controller.CheckForNeighbourBlocks(Controller.BlockList[X, Y - 1]);
        }
    }

}
