using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Copy : MonoBehaviour
{
	public CharacterMovement parent;

	void OnMouseDown()
	{
		parent.Select();
	}
	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}
}
