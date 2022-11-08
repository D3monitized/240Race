using UnityEngine;
using System.Collections.Generic; 

public class Spline : MonoBehaviour
{
    private List<Transform> m_nodes; 

    public void CreateNode()
	{
		GameObject go = new GameObject();
		go.name = "Node: " + m_nodes.Count + 1;
		go.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 10));
		m_nodes.Add(go.transform); 
	}
}
