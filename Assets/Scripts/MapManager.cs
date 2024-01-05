using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Map Manager
/// </summary>
public class MapManager : MonoBehaviour
{
    #region values
    //prefebs in the Resource folder
    private GameObject m_prefab_tile;
    private GameObject m_prefab_wall;
    private GameObject m_moving_spikes;
    private GameObject m_smashing_spikes;
    private GameObject m_Gem;

    private Transform m_Transform;

    //distance between each floor pieces, 0.254f is the side length of each tile_white squre
    public float bottomLength = 0.254f * Mathf.Sqrt(2);

    //floor color
    private Color colorOne = new Color(124 / 255f, 155 / 255f, 230 / 255f);
    private Color colorTwo = new Color(125 / 255f, 169 / 255f, 233 / 255f);
    //wall color
    private Color colorThree = new Color(87 / 255f, 93 / 255f, 169 / 255f);

    //store mapinfo by using list
    public List<GameObject[]> mapList = new List<GameObject[]>();

    //index for floor destroy, start from the 0st row
    private int index = 0;

    private PlayerController pc;
    private CameraFollow m_CameraFollow;

    //possibility to generate various traps and treasure
    public int holePossibility = 0;
    private int spikesPossibility = 0;
    private int skyPossibility = 0;
    private int gem = 2;
    #endregion


    #region start method
    //Use this for initialization
	void Start () {
        m_prefab_tile = Resources.Load("tile_white") as GameObject;
        m_prefab_wall = Resources.Load("wall2") as GameObject;
        m_moving_spikes = Resources.Load("moving_spikes") as GameObject;
        m_smashing_spikes = Resources.Load("smashing_spikes") as GameObject;
        m_Gem = Resources.Load("Gem 2") as GameObject;
        m_Transform = gameObject.GetComponent<Transform>();
        CreateMapItem(0);

        pc = GameObject.Find("cube_books").GetComponent<PlayerController>(); ;
        m_CameraFollow = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
    }
    #endregion


    #region map generating method
    /// <summary>
    /// create map item
    /// </summary>
    /// <param name="offsetZ"> generate new map with offset Z, otherwise they all overlap together</param>
    public void CreateMapItem(float offsetZ)
    {
        for (int i = 0; i < 10; i++)
        {

            //even rows
            GameObject[] itemOdd = new GameObject[6];

            for (int j = 0; j < 6; j++)
            {
                Vector3 pos = new Vector3(j * bottomLength, 0, offsetZ + i * bottomLength);              
                Vector3 rot = new Vector3(-90, 45, 0);

                //a GameObject type to instantiate prefeb
                GameObject tile = null;

                //Instantiate(prefeb, location, rotation), generate wall, generate floor
                if (j == 0 || j == 5)
                {
                    tile = GameObject.Instantiate(m_prefab_wall, pos, Quaternion.Euler(rot)) as GameObject; //generate wall
                    tile.GetComponent<MeshRenderer>().material.color = colorThree;
                }
                else
                {
                    int possibility = CalTrapPossibility();            //for each floor piece, make decision about what to generate according to the possibility value

                    tile = GenerateFloorOrTrap(possibility, pos, rot, colorOne); //generate a floor piece or a trap or someting else
                    
                }
                
                //*add tile under father object
                tile.GetComponent<Transform>().SetParent(m_Transform);

                itemOdd[j] = tile;
            }
            //*itemOdd store a row of information, mapList store itemOdds
            mapList.Add(itemOdd);



            //odd rows

            GameObject[] itemEven = new GameObject[5];

            for (int j = 0; j < 5; j++)
            {
                Vector3 pos1 = new Vector3(j * bottomLength + bottomLength / 2, 0, offsetZ + i * bottomLength + bottomLength / 2);

                Vector3 rot = new Vector3(-90, 45, 0);

                GameObject tile1 = null;

                int possibility = CalTrapPossibility();            //for each floor piece, make decision about what to generate according to the possibility value

                tile1 = GenerateFloorOrTrap(possibility, pos1, rot, colorTwo); //generate a floor piece or a trap or someting else

                tile1.GetComponent<Transform>().SetParent(m_Transform);

                itemEven[j] = tile1;
            }
            //*itemEven store a row of information, mapList store itemEvens (itemOdd1,itemEven1,itemOdd2,itemEven2 ...)
            mapList.Add(itemEven);
        }
    }
    #endregion


    #region floor break down method: FloorBreakDown()
    public IEnumerator FloorBreakDown()
    {
        yield return new WaitForSeconds(1f);

        while (true)
        {
            yield return new WaitForSeconds(0.165f);
            //Traverse maplist[0] and break down floor piece one by one
            for (int i = 0; i < mapList[index].Length; i++)
            {         
                Rigidbody rb = mapList[index][i].AddComponent<Rigidbody>(); //add physics so that floor piece drop
                rb.angularVelocity = new Vector3(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)) * Random.Range(1, 10); //when it drops, drop to random direction with random force

                GameObject.Destroy(mapList[index][i], 1f); //Destroy dropped gameobject

                //if player is on the corresponding row, excute the player by adding rigid body and stop the game
                if (index-1 == pc.Z)
                {
                    pc.hp = false;
                    StopFloorBreakDown();
                    m_CameraFollow.music[0].Pause();
                    m_CameraFollow.music[3].Play();
                    pc.StartCoroutine("GameOver",false);
                    pc.gameObject.AddComponent<Rigidbody>();
                    break;
                    //Destroy(pc.gameObject, 1.0f);
                }
                
            }
            index++;
        }
    }
    #endregion

    #region start floor breakdown method: StartFloorBreakDown()
    public void StartFloorBreakDown()
    {
        StartCoroutine("FloorBreakDown");
    }
    #endregion

    #region stop floor breakdown method: StopFloorBreakDown()
    public void StopFloorBreakDown()
    {
        StopCoroutine("FloorBreakDown");
    }
    #endregion



    #region hole trap generating method: CalTrapPossibility()
    /// <summary>
    /// Calculate possibility
    /// 0 for floor  piece
    /// 1 for hole trap
    /// 2 for ground trap
    /// 3 for sky trap
    /// </summary>
    /// <returns></returns>
    private int CalTrapPossibility()
    {
        int pr = Random.Range(0, 100);
        if (pr < holePossibility)    //if pr<holepossibility, generate hole trap
        {
            return 1;
        }
        else if (31 < pr && pr < spikesPossibility + 30)
        {
            return 2;
        }
        else if (61 < pr && pr < skyPossibility + 60)
        {
            return 3;
        }

        return 0;
    }
    #endregion

    #region treasure generating method
    /// <summary>
    /// generate treasure if 1, noting if 0
    /// </summary>
    /// <returns></returns>
    private int CalTreasurePossibility()
    {
        int pr = Random.Range(0, 100);
        if (gem > pr)
        {
            return 1;
        }else
        {
            return 0;
        }
    }
    #endregion

    #region add chance of generating traps: AddTrapPossibility()
    public void AddTrapPossibility()
    {
        if(holePossibility<10)
            holePossibility += 2;
        if(spikesPossibility<6)
            spikesPossibility += 1;
        if(skyPossibility<6)
            skyPossibility += 1;
    }
    #endregion

    #region generate floor or trap method: GenerateFloorOrTrap(int evenorodd, int possibility, Vector3 pos, Vector3 rot)

    /// <summary>
    /// for each floor-piece position, generate a floor, or a hole, or a spike, or a sky up to the possibility value
    /// </summary>
    /// <param name="a">possibility</param>
    /// <param name="b">pos</param>
    /// <param name="c">rot</param>
    /// <returns></returns>
    private GameObject GenerateFloorOrTrap(int a, Vector3 b, Vector3 c, Color d)
    {
        GameObject tile = null;
        int possibility = a;
        Vector3 pos = b;
        Vector3 rot = c;
        Color color = d;
       
            // 0 for generating normal floor piece
            if (possibility == 0)
            {
                tile = GameObject.Instantiate(m_prefab_tile, pos, Quaternion.Euler(rot)) as GameObject; //generate normal floor piece
                tile.GetComponent<MeshRenderer>().material.color = color; //add color on it
                tile.GetComponent<Transform>().Find("normal_a2").GetComponent<MeshRenderer>().material.color = color; //seems <Transform>.Find() can get the whold child instead child<Trnaform>

                // after generating floor, judge whether treasure is generated above the floor
                if (CalTreasurePossibility() == 1)
                {
                    GameObject Treasure = GameObject.Instantiate(m_Gem, tile.GetComponent<Transform>().position + new Vector3(0, 0.06f, 0), Quaternion.identity) as GameObject;
                    Treasure.GetComponent<Transform>().SetParent(tile.GetComponent<Transform>()); //set tile as treasure's father object, when tile fall, treasure will fall with tile
                }
            }

            // 1 for generating hole trap (attention: hole is an empty object with same position and rotation as floor piece)
            else if (possibility == 1)
            {
                //way1: the next three lines, it can generate null object without record on the unity left side bar
                tile = new GameObject(); //generate hole trap (empty object)
                tile.GetComponent<Transform>().position = pos;
                tile.GetComponent<Transform>().rotation = Quaternion.Euler(rot);
                //way2: the next one line, it can generate null object with record on the unity left side bar
                //tile = GameObject.Instantiate(new GameObject(), pos, Quaternion.Euler(rot)) as GameObject;
            }

            //2 for generating spike trap
            else if (possibility == 2)
            {
                tile = GameObject.Instantiate(m_moving_spikes, pos, Quaternion.Euler(rot)) as GameObject; //generate spike trap
            }

            //3 for generating sky trap
            else if (possibility == 3)
            {
                tile = GameObject.Instantiate(m_smashing_spikes, pos, Quaternion.Euler(rot)) as GameObject; //generate sky trap
            }
        
        
        return tile;
    }
    #endregion

    #region reset map
    public void ResetMap()
    {
        Transform[] sonOfTransform = m_Transform.GetComponentsInChildren<Transform>(); 
        
        //remove all the current floor and traps
        for (int i = 1; i < sonOfTransform.Length;i++ )
        {
            GameObject.Destroy(sonOfTransform[i].gameObject);
        }

        //reset all the possibility
        holePossibility = 0;
        spikesPossibility = 0;
        skyPossibility = 0;

        //塌陷角标清除
        index = 0;

        mapList.Clear();

        //regenerate map
        CreateMapItem(0);
    }
    #endregion

    #region update method
    // Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            string str = "";
            for (int i = 0; i < mapList.Count; i++)
            {
                for (int j = 0; j < mapList[i].Length; j++)
                {
                    str += mapList[i][j].name;
                    mapList[i][j].name = i + "_" + j;
                }
                str += "\n";
            }
            Debug.Log(str);
        }
    }
    #endregion
}
