using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum BlockType {Empty,Stone,Iron,Gold,Bedrock};
public class Block
{
    public BlockType _Type = BlockType.Empty;
    public int X { get; protected set; }
    public int Y { get; protected set; }
    private int HP = 10;
    public int HitPoints { get =>HP; set{ int OldHP = HP; HP = value;HPCheck(); } }
    public int GoldValue = 1;
    public GameObject MyGameObject;

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
        if(HitPoints < 1&&_Type != BlockType.Empty )
        {
            gameController Controller = gameController.Instance;
            MyGameObject.GetComponent<BoxCollider2D>().enabled = false;
            MyGameObject.GetComponent<SpriteRenderer>().sprite = gameController.Instance.GrassSprites[Random.Range(0,3)];
            _Type = BlockType.Empty;
            if (Controller.BlockList[X + 1, Y]._Type != BlockType.Empty && Controller.BlockList[X + 1, Y].X != Controller.Width - 1)
                Controller.CheckForNeighbourBlocks(Controller.BlockList[X + 1, Y]);
            if (Controller.BlockList[X, Y + 1]._Type != BlockType.Empty && Controller.BlockList[X, Y + 1].Y != Controller.Height - 1)
                Controller.CheckForNeighbourBlocks(Controller.BlockList[X, Y + 1]);
            if (Controller.BlockList[X - 1, Y]._Type != BlockType.Empty && Controller.BlockList[X - 1, Y].X != 0)
                Controller.CheckForNeighbourBlocks(Controller.BlockList[X - 1, Y]);
            if (Controller.BlockList[X, Y - 1]._Type != BlockType.Empty && Controller.BlockList[X, Y - 1].Y != 0)
                Controller.CheckForNeighbourBlocks(Controller.BlockList[X, Y - 1]);
        }

    }
    public void ChangeType(BlockType ChangedType)
    {
        _Type = ChangedType;
        if(ChangedType == BlockType.Empty)
        {
            HitPoints = 0;
        }
        else if(ChangedType == BlockType.Stone)
        {
            HitPoints = 100;
            GoldValue = 10;
        }
        else if(ChangedType == BlockType.Iron)
        {
            HitPoints = 250;
            GoldValue = 50;
        }
        else if(ChangedType == BlockType.Gold)
        {
            HitPoints = 500;
            GoldValue = 150;
        }

    }

}
