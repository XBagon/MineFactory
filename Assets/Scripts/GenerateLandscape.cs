using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenerateLandscape : MonoBehaviour {
    public List<GameObject> BlockPrefabs = new List<GameObject>();

    public int length;
    public int heightScale;
    public float detailScale;
    public int seed;

    Player mainPlayer;


    public Block[,,] World;

    void Start () {

        mainPlayer = GameObject.Find("Player").GetComponent<Player>();

        World = new Block[length, 20, length];

        if (seed == 0) seed = (int) Network.time * 10;




        for (int z = 0; z < length; z++)
        {
            for (int x = 0; x < length; x++)
            {
                int y = (int) (Mathf.PerlinNoise((x+seed) / detailScale, (z+seed) / detailScale) * heightScale);
                Vector3 blockPos = new Vector3(x, y, z);
                CreateBlock(blockPos, true);
                

                while(y > 0)
                {
                    y--;
                    blockPos.y = y;
                    CreateBlock(blockPos, false);
                }
            }
        }
	}

    void CreateBlock(Vector3 blockPos, bool create)
    {
        int blockType;

        //generation
        if (blockPos.y < 10)
        {
            blockType = 1;
        }
        else
        {
            blockType = 0;
        }



        //ores
        if (blockPos.y < 3 && Random.value > 0.95f)
        {
            blockType = 2;
        }


        



        if (create)
        {
            GameObject b = (GameObject)Instantiate(BlockPrefabs[blockType], blockPos, Quaternion.identity);
            b.transform.SetParent(transform);
        }

        World[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = new Block(blockType, create);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
            if (Physics.Raycast(ray, out hit, mainPlayer.distance))
            {
                Vector3 blockPos = hit.transform.position;

                World[(int)blockPos.x, (int)blockPos.y, (int)blockPos.z] = null;

                Destroy(hit.transform.gameObject);

                for (int x = -1; x <= 1; x++)
                    for (int y = -1; y <= 1; y++)
                        for (int z = -1; z <= 1; z++)
                        {
                            if (!(x == 0 && y == 0 && z == 0))
                            {
                                DrawBlock(new Vector3(blockPos.x + x, blockPos.y + y, blockPos.z + z));
                            }
                        }


            }
        }
    }

    private void DrawBlock(Vector3 blockP)
    {
        Block b = World[(int)blockP.x, (int)blockP.y, (int)blockP.z];

        if (b == null) return;

        if(b.vis == false)
        {
            b.vis = true;

            if (!(b.type > BlockPrefabs.Count))
            {
                Instantiate(BlockPrefabs[b.type], blockP, Quaternion.identity);
            }
            else
            {
                b.vis = false;
            }
        }
    }
}
