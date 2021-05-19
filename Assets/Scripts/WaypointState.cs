using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointState : MonoBehaviour
{
	// public section
    public List<GameObject> neighbors;
	[SerializeField] float rColor;
	[SerializeField] float gColor;
	[SerializeField] float bColor;

	public GameObject previous
    {
		get;
		set;
    }

	public float distance
    {
		get;
		set;
    }

	// private section

	void Start()
    {

    }

	void OnDrawGizmos()
	{
		if (neighbors == null)
			return;
		Gizmos.color = new Color(rColor, gColor, bColor);
		foreach (var neighbor in neighbors)
		{
			if (neighbor != null)
				Gizmos.DrawLine(transform.position, neighbor.transform.position);
		}
	}
}
