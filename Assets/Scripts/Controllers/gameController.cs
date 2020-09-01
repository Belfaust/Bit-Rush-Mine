﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameController : MonoBehaviour
{
    [SerializeField]
    public GameObject Player;
    public GameObject Blocks;
    public GameObject[] LevelBoss;
    public int Width, Height;
    public Vector2Int Offset;
    public Block[,] BlockList;
    public int Gold = 0,LevelIndex = 0;
    public Sprite[] SpriteSlots;
    public Sprite[] GrassSprites;
    public Text GoldText;
    public LayerMask OreDetector,MobDetector,ActionBlocks;
    public static gameController Instance { get; protected set;}

    private void Awake()
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
        UpdateGold(0);
    }
    public void GenerateLevel(int height, int width)
    {
        int seed = Random.Range(-100000, 100000);
        this.Width = width;
        this.Height = height;
        BlockList = new Block[Width, Height];
        float[,] noiseMap = NoiseMap.GenerateNoiseMap(Width, Height, seed, 50, 2, .5f, 3, new Vector2(5, 8));
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                GameObject Block = Instantiate(Blocks, new Vector3(x ,y, 0), Quaternion.identity, transform);
                BlockList[x, y] = new Block(x, y,Block,BlockType.Empty);
                Block.name = "Block No." + x + "|" + y;
                if (y == 0 || y == Height-1 || x == 0 || x == Width-1)
                {
                    BlockList[x, y].ChangeType(BlockType.Bedrock);
                }
                else if ((int)(noiseMap[x, y]*100) > 5&&(Random.Range(1,100)>5))
                {
                    BlockList[x, y].ChangeType(BlockType.Stone);
                }
                else
                {
                   BlockList[x, y].ChangeType(BlockType.Empty);
                }
            }
        }
        Vector2Int BossPos = new Vector2Int();
        BossPos.x = Random.value > 0.5f ? (int)Random.Range(10, 30f) : (int)Random.Range(Width-30, Width-10);
        BossPos.y = Random.value > 0.5f ? (int)Random.Range(10, 20) : (int)Random.Range(Height-20,Height-10);
        Instantiate(LevelBoss[LevelIndex], new Vector3(BossPos.x, BossPos.y,0), Quaternion.identity,transform);
        for(int x = BossPos.x; x < BossPos.x + 10; x++)
        {
            for (int y = BossPos.y; y < BossPos.y + 10; y++)
            {

                BlockList[x-5,y-5].ChangeType(BlockType.Empty);
            }
        }
        Player.transform.position = new Vector2(Random.Range(40,Width-40), Random.Range(25, Height - 25));
        Debug.Log(Player.transform.position);
        for (int x = (int)Player.transform.position.x; x < (int)Player.transform.position.x + 5; x++)
        {
            for (int y = (int)Player.transform.position.y; y < (int)Player.transform.position.y + 5; y++)
            {

                BlockList[x - 2, y - 2].ChangeType(BlockType.Empty);
            }
        }


        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Block tmpBlock = BlockList[x, y];
                 if (tmpBlock._Type != BlockType.Empty&&x!=0&&y!=0&&x!=Width-1&&y!= Height-1)
                {
                    CheckForNeighbourBlocks(BlockList[x, y]);
                }
                 else if(tmpBlock._Type == BlockType.Empty)
                {
                    tmpBlock.MyGameObject.GetComponent<BoxCollider2D>().enabled = false;
                    tmpBlock.MyGameObject.GetComponent<SpriteRenderer>().sprite = GrassSprites[Random.Range(0, 3)];
                }

            }
        }

    }
    public void UpdateGold(int goldAmount)
    {
        Gold += goldAmount;
        GoldText.text = "Gold : " +Gold.ToString();
    }

    public void CheckForNeighbourBlocks(Block myblock)
    {
        List<int> specialvals = new List<int>();
        specialvals.Add(15);
        specialvals.Add(7);
        specialvals.Add(13);
        specialvals.Add(2);
        List<int> specialvalsCalc = new List<int> { 115, 1015, 2015, 3007, 3115, 3013, 3007, 3015, 3113, 3115, 4002, 4115, 5014, 5015, 5108, 5115, 6015, 6100, 6115, 8115, 9015 };
        Vector2 position = new Vector2(myblock.X, myblock.Y);
        if (SpriteSlots.Length < 9) return;
        int index = (GetBlockAt(position + Vector2.up) ? 1 : 0)
                + (GetBlockAt(position + Vector2.right) ? 2 : 0)
                + (GetBlockAt(position + Vector2.down) ? 4 : 0)
                + (GetBlockAt(position + Vector2.left) ? 8 : 0);

        if (specialvals.Contains(index))
        {
            int tmpI = index;
            tmpI += (GetBlockAt(position + Vector2.up + Vector2.left) ? 100 : 0);
            tmpI += (GetBlockAt(position + Vector2.left + Vector2.down) ? 5000 : 0);
            tmpI += (GetBlockAt(position + Vector2.right + Vector2.down) ? 1000 : 0);
            tmpI += (GetBlockAt(position + Vector2.right + Vector2.up) ? 3000 : 0);
            if (specialvalsCalc.Contains(tmpI))
            {
                index = tmpI;
            }
        }

        Sprite slot = SpriteSlots[19];
        switch (index)
        {

            case 1: slot = SpriteSlots[18]; break;
            case 2: slot = SpriteSlots[14]; break;
            case 4: slot = SpriteSlots[17]; break;
            case 5: slot = SpriteSlots[15]; break;
            case 8: slot = SpriteSlots[13]; break;


            case 3: slot = SpriteSlots[10]; break;
            case 6: slot = SpriteSlots[0]; break;
            case 7: slot = SpriteSlots[5]; break;
            case 9: slot = SpriteSlots[12]; break;
            //left and right
            case 10: slot = SpriteSlots[16]; break;
            case 11: slot = SpriteSlots[11]; break;
            case 12: slot = SpriteSlots[2]; break;
            case 13: slot = SpriteSlots[7]; break;
            case 14: slot = SpriteSlots[1]; break;

            case 15: slot = SpriteSlots[6]; break;
            case 115: slot = SpriteSlots[9]; break;
            case 1015: slot = SpriteSlots[3]; break;
            case 1115: slot = SpriteSlots[4]; break;
            case 5015: slot = SpriteSlots[4]; break;
            case 4115: slot = SpriteSlots[4]; break;
            case 5115: slot = SpriteSlots[6]; break;
            case 6015: slot = SpriteSlots[6]; break;
            case 3115: slot = SpriteSlots[6]; break;
            case 3013: slot = SpriteSlots[8]; break;
            case 3113: slot = SpriteSlots[8]; break;
            case 3007: slot = SpriteSlots[9]; break;
            case 3015: slot = SpriteSlots[6]; break;
            case 4002: slot = SpriteSlots[14]; break;
            case 5014: slot = SpriteSlots[6]; break;
            case 5108: slot = SpriteSlots[7]; break;
            case 6100: slot = SpriteSlots[8]; break;
            case 6115: slot = SpriteSlots[8]; break;
            case 8115: slot = SpriteSlots[3]; break;
            case 9015: slot = SpriteSlots[9]; break;
        }
        myblock.MyGameObject.GetComponent<SpriteRenderer>().sprite = slot;
    }

    public Block GetBlockAt(int x , int y)
    {
        return BlockList[x, y];
    }
    public bool GetBlockAt(Vector2 position)
    {
        if (BlockList[(int)position.x, (int)position.y ]._Type != BlockType.Empty)
            return true;
        else
            return false;
    }
}



