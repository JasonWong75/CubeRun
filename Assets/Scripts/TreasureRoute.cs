using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureRoute : MonoBehaviour {
    private Transform m_SubTransform;
	// Use this for initialization
	void Start () {
        m_SubTransform = gameObject.GetComponent<Transform>().Find("gem 3").GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
		m_SubTransform.Rotate(Vector3.right*0.8f+Vector3.up, Space.World);
	}
}
