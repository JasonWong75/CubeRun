using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// spike trap
/// </summary>
public class Spikes : MonoBehaviour {

    private Transform m_Transform;
    private Transform son_Transform;

    private Vector3 normalPos;
    private Vector3 targetPos;

	void Start () {
		m_Transform = gameObject.GetComponent<Transform>();
        son_Transform = m_Transform.Find("moving_spikes_b").GetComponent<Transform>();

        normalPos = son_Transform.position;
        targetPos = son_Transform.position + new Vector3(0, 0.15f, 0);

        StartCoroutine("UpAndDown");
    }
	

    private IEnumerator SpikeRaise()
    {
        while (true)
        {
            son_Transform.position = Vector3.Lerp(son_Transform.position, targetPos, Time.deltaTime * 50);
            yield return null; //pause for one frame
        }
    }

    private IEnumerator SpikeDown()
    {
        while (true)
        {
            son_Transform.position = Vector3.Lerp(son_Transform.position, normalPos, Time.deltaTime * 10);
            yield return null; //pause for one frame
        }
    }

    private IEnumerator UpAndDown()
    {
        while (true)
        {
            StopCoroutine("SpikeDown");
            StartCoroutine("SpikeRaise");
            yield return new WaitForSeconds(2.0f); // pause 2s for reaching peek
            StopCoroutine("SpikeRaise");
            StartCoroutine("SpikeDown");
            yield return new WaitForSeconds(2.0f); // pause 2s for reaching bottom
        }
    }
}
