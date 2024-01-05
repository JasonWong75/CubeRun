using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// camera follow the player
/// </summary>
public class CameraFollow : MonoBehaviour {

    private Transform m_Transform;
    private Transform player_Transform;
    private bool startFollow = false;
    private Vector3 normalPos; //the original camera position
    public AudioSource[] music;

    //attribute
    public bool StartFollow
    {
        get { return startFollow;}
        set { startFollow = value;}
    }

    void Start()
    {
        m_Transform = gameObject.GetComponent<Transform>();
        normalPos = m_Transform.position;
        player_Transform = GameObject.Find("cube_books").GetComponent<Transform>();  //player's position
        music = gameObject.GetComponents<AudioSource>();
    }


    void Update()
    {
        CameraMove();
    }

    void CameraMove()
    {
        if (startFollow == true)
        {
            //camera follow: x: camera's default x value so that camera not out of bound
            //y+offset: player's y value, camera always follow the player when it move forward
            //z: certain distance between camera and player in case they are overlapping
            /*camera绑在cube下时候，采用的是cube的坐标系，其与cube的距离==摄像机的z轴数据==global坐标中摄像机的y轴数据。vector3采用的是global坐标系，故将
            z轴观测到的数据加到y轴上*/
            Vector3 pos = new Vector3(m_Transform.position.x, player_Transform.position.y +1.633f , player_Transform.position.z);
            m_Transform.position = Vector3.Lerp(m_Transform.position, pos, Time.deltaTime);
        }
    }

    public void ResetCamera()
    {
        m_Transform.position = normalPos;
    }
}
