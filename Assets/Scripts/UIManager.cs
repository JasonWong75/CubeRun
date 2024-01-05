using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// UI Manager
/// </summary>
public class UIManager : MonoBehaviour {

    public GameObject m_StartUI;
    public GameObject m_GameUI;
    private UILabel m_UIScore;
    private UILabel m_UIGemCount;
    private UILabel m_GameScore;
    private UILabel m_GameGemCount;
    private GameObject m_PlayButton;
    private PlayerController m_PlayerController;
    private CameraFollow m_CameraFollow;
    private GameObject m_LeftMoveButton;
    private GameObject m_RightMoveButton;

	void Start () {
        m_StartUI = GameObject.Find("StartUI");
        m_GameUI = GameObject.Find("GameUI");
        m_UIScore = GameObject.Find("UIScore").GetComponent<UILabel>();
        m_UIGemCount = GameObject.Find("UIGemCount").GetComponent<UILabel>();
        m_GameScore = GameObject.Find("GameScore").GetComponent<UILabel>();
        m_GameGemCount = GameObject.Find("GameGemCount").GetComponent<UILabel>();
        m_PlayButton = GameObject.Find("startButton");
        m_PlayerController = GameObject.Find("cube_books").GetComponent<PlayerController>();
        m_CameraFollow = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
        m_LeftMoveButton = GameObject.Find("Left");
        m_RightMoveButton = GameObject.Find("Right");


        UIEventListener.Get(m_PlayButton).onClick = StartGameButton;   //固定格式：鼠标点击触发事件
        UIEventListener.Get(m_LeftMoveButton).onClick = StartLeftButton;
        UIEventListener.Get(m_RightMoveButton).onClick = StartRightButton;

        Init();

        m_GameUI.SetActive(false);   //Disable the in-game UI

	}


    //reset all interfaces
    public void Init()
    {
        m_UIScore.text = PlayerPrefs.GetInt("score", 0).ToString();
        m_UIGemCount.text = PlayerPrefs.GetInt("treasure", 0)+"/???";
        m_GameScore.text = "0";
        m_GameGemCount.text = PlayerPrefs.GetInt("treasure", 0)+"/???"; 
    }


    /// <summary>
    /// when click the button, switch UI, and start the game without pressing R
    /// </summary>
    /// <param name="go"></param>
    private void StartGameButton(GameObject go)
    {
        Debug.Log("game start");
        m_StartUI.SetActive(false);
        m_GameUI.SetActive(true);
        m_PlayerController.GameStart();
        m_CameraFollow.music[0].Play();
    }


    private void StartLeftButton(GameObject go)
    {
        m_PlayerController.LeftMove();
    }

    private void StartRightButton(GameObject go)
    {
        m_PlayerController.RightMove();
    }


    /// <summary>
    /// update score and gem in time, connect to AddTreasure() and AddScore() in PlayerController class
    /// </summary>
    /// <param name="score"></param>
    /// <param name="gem"></param>
    public void UpdateData(int score, int gem)
    {
        m_GameScore.text = score.ToString();
        m_GameGemCount.text = gem+"/???";
        m_UIScore.text = score.ToString();
        m_UIGemCount.text = gem + "/???";
    }
    
}
