using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// script for player control
/// </summary>
public class PlayerController : MonoBehaviour
{

    #region values
    private int z = 0;
    private int x = 2;
    //way1:  the map generate times
    //private int count = 1;

    private Transform m_TransForm;

    private MapManager m_MapManager;
    private CameraFollow m_CameraFollow;
    private UIManager m_UIManager;
    private SkyTrap m_SkyTrap;
    private Spikes m_Spikes;

    //trail color
    private Color colorOne = new Color(122 / 255f, 85 / 255f, 179 / 255f);
    private Color colorTwo = new Color(126 / 255f, 93 / 255f, 183 / 255f);

    public bool hp = false; // if it is inable, the player can not move

    private int treasureCount = 0; // count of the quantity of treasure gathered
    private int score = 0; // total score
    #endregion

    #region attributes
    public int Z
    {
        get { return z; }
        set { z = value; }
    }

    public int X
    {
        get { return x; }
        set { x = value; }
    }
    #endregion


    void Start () {
        treasureCount = PlayerPrefs.GetInt("treasure", 0); //get Treasure count from previous game by visiting playerprefs database
        m_TransForm = gameObject.GetComponent<Transform>();
        m_MapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        m_CameraFollow = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
        m_UIManager = GameObject.Find("UI Root").GetComponent<UIManager>();
        m_SkyTrap = GameObject.Find("smashing_spikes").GetComponent<SkyTrap>();
        m_Spikes = GameObject.Find("moving_spikes").GetComponent<Spikes>();
    }

	void Update () {
        if (Input.GetKeyDown(KeyCode.R))
        {
            GameStart();
            
        }

        // the boundry limit problem, which is hard to understand

        
            PlayerControl();
        
    }

    public void GameStart()
    {
        hp = true;
        //when hit M, generate cube at {z,x} which registed on the mapList

        Move();

        // camera start to follow the character

        m_CameraFollow.StartFollow = true;

        m_MapManager.StartFloorBreakDown();
    }

    #region player control
    /// <summary>
    /// player move
    /// </summary>
    private void PlayerControl()
    {
        
        // A for leftward
        if (Input.GetKeyDown(KeyCode.A))
        {
            LeftMove();
        }
        // D for rightward

        if (Input.GetKeyDown(KeyCode.D))
        {
            RightMove();
        }
    }
    #endregion

    public void LeftMove()
    {
        if (hp)
        {

            if (x != 0)
            {
                z++;
                AddScore();
                m_CameraFollow.music[1].Play();
            }

            if (z % 2 == 1 && x != 0)
            {
                x--;
            }

            Debug.Log("Left:z: " + z + " x: " + x);
            Move();
            CalPosition();
        }
        
    }

    public void RightMove()
    {
        if (hp)
        {

            if (z % 2 == 0 || x != 4)
            {
                z++;
                AddScore();
                m_CameraFollow.music[1].Play();
            }
            if (z % 2 == 0 && x != 4)
            {
                x++;
            }

            Debug.Log("Right:z: " + z + " x: " + x);
            Move();
            CalPosition();
        }
    }

    #region player move 
    /// <summary>
    /// let player move
    /// </summary>
    private void Move()
    {
        Transform playerPosition = m_MapManager.mapList[z][x].GetComponent<Transform>();
        MeshRenderer playerMesh = null;

        // detect the floor type
        if (playerPosition.tag == "floor")
        {
            playerMesh = playerPosition.Find("normal_a2").GetComponent<MeshRenderer>();
        }
        else if (playerPosition.tag == "skytrap")
        {
            playerMesh = playerPosition.Find("smashing_spikes_a2").GetComponent<MeshRenderer>();
        }
        else if (playerPosition.tag == "spiketrap")
        {
            playerMesh = playerPosition.Find("moving_spikes_a2").GetComponent<MeshRenderer>();
        }

        // add color onto the trail, OR, IF IT IS THE HOLE TRAP, EXECUTE THE PLAYER
        if (playerMesh != null)
        {
            if (z % 2 == 0)
            {
                playerMesh.material.color = colorOne;
            }
            else
            {
                playerMesh.material.color = colorTwo;
            }
        }
        else
        {
            hp = false;
            m_MapManager.StopFloorBreakDown();
            m_CameraFollow.music[0].Pause();
            m_CameraFollow.music[3].Play();
            StartCoroutine("GameOver", false);
            gameObject.AddComponent<Rigidbody>();
        }

        m_TransForm.position = playerPosition.position + new Vector3(0, 0.254f / 2, 0);     // new Vector3 is the vector that lift player upon to the map
        m_TransForm.rotation = playerPosition.rotation;
    }
    #endregion

    #region when player gathered new treasure
    private void AddTreasure()
    {
        treasureCount++;
        m_CameraFollow.music[2].Play();
        Debug.Log("Got a new treasure: "+ treasureCount);

        m_UIManager.UpdateData(score, treasureCount); // method from UIManager, when user get Gem, UIManager will update in time
    }
    #endregion

    #region player's total score
    private void AddScore()
    {
        score++;
        Debug.Log("Total Score: " + score);

        m_UIManager.UpdateData(score, treasureCount); // when user get score, UIManager will update in time
    }
    #endregion

    #region   Save player's Treasure and the highest score into Playerprefs database
    private void SaveData()
    {
        PlayerPrefs.SetInt("treasure", treasureCount);

        if (score > PlayerPrefs.GetInt("score", 0))
        {
            PlayerPrefs.SetInt("score", score);
            Debug.Log("New Highest Score: " + score);
        }

    }
#endregion

    #region generate new map area
    /// <summary>
    /// generate new map area
    /// </summary>
    private void CalPosition()
    {
        if ((m_MapManager.mapList.Count - z) <= 15)
        {
            Debug.Log("Generate new Map"+" hoelpos: "+m_MapManager.holePossibility);

            //way1: generate new map by fixed distance
            //m_MapManager.CreateMapItem(count* 10 * 0.254f * Mathf.Sqrt(2));

            //way2: generate new map follwing the last floor from the last map
            float offsetZ = m_MapManager.mapList[m_MapManager.mapList.Count-1][0].GetComponent<Transform>().position.z + m_MapManager.bottomLength/2;           
            m_MapManager.CreateMapItem(offsetZ);
            m_MapManager.AddTrapPossibility(); //increasing trap-generating possibility each time generating new floor area

            //(way1:) count++;
        }
    }
    #endregion

    # region when player goes into spike trap or sky trap or player gather new Treasure

    /// <summary>
    /// when player goes into spike trap or sky trap
    /// </summary>
    /// <param name="coll"></param>
    private void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "spike" || coll.tag == "sky")
        {
           // gameObject.AddComponent<Rigidbody>();
           
            //Transform trapTransform = coll.gameObject.GetComponent<Transform>().parent;

           // GameObject.Destroy(trapTransform.gameObject);
                        
            hp = false;
            m_MapManager.StopFloorBreakDown();
            m_CameraFollow.music[0].Pause();
            m_CameraFollow.music[4].Play();
            StartCoroutine("GameOver", false);
           // Destroy(gameObject, 0.3f); 
        }
        else if (coll.tag == "gem")
        {
            AddTreasure();
            GameObject.Destroy(coll.gameObject.GetComponent<Transform>().parent.gameObject);
        }
    }
    #endregion

    #region    kill the player
    /// <summary>
    /// when player died, end the game, reset UI and reset player and map, this is a XieCheng program, if b is true,  then excute player after 0.5s, it is used to hole trap for giving player enough time to fall
    /// </summary>
    public IEnumerator GameOver(bool b)
    {
        if (b)
        {
            yield return new WaitForSeconds(0.6f);  /// 0.5f == 0.5 seconds
        }


        SaveData();
        m_CameraFollow.StartFollow = false;
        yield return new WaitForSeconds(0.9f);
        Debug.Log("Game Over");
        
        hp = false;

        m_UIManager.m_GameUI.SetActive(false);
        m_UIManager.m_StartUI.SetActive(true);

        ResetPlayer();  
        m_MapManager.ResetMap();
        m_CameraFollow.ResetCamera();
        m_UIManager.Init();



        //Time.timeScale = 0; //Time stop, easy to trigger unity to crush
    }
    #endregion

    #region reset the player
    private void ResetPlayer()
    {
        GameObject.Destroy(gameObject.GetComponent<Rigidbody>()); // remove rigidbody on player
        
        z = 0;  //relocate player
        x = 2;

        score = 0;
    }
    #endregion

}
