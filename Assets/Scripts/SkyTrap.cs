using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyTrap : MonoBehaviour {
    private Transform m_Transform;
    private Transform son_Transform;

    private Vector3 pos;
    private Vector3 targetpos;

	void Start () {
        m_Transform = gameObject.GetComponent<Transform>();
        son_Transform = m_Transform.Find("smashing_spikes_b").GetComponent<Transform>();
        pos = m_Transform.position;
        targetpos = m_Transform.position + new Vector3(0, 0.6f, 0);

        StartCoroutine("UpAndDown");
	}

    private IEnumerator Up()
    {
        while (true)
        {
            son_Transform.position = Vector3.Lerp(son_Transform.position, targetpos, Time.deltaTime * 10);
            yield return null;
        }
    }

    private IEnumerator Down()
    {
        while (true)
        {
            son_Transform.position = Vector3.Lerp(son_Transform.position, pos, Time.deltaTime * 10);
            yield return null;
        }
    }

    private IEnumerator UpAndDown()
    {
        while (true)
        {
            StopCoroutine("Down");
            StartCoroutine("Up");
            yield return new WaitForSeconds(2.0f);
            StopCoroutine("Up");
            StartCoroutine("Down");
            yield return new WaitForSeconds(2.0f);
        }
    }
}
