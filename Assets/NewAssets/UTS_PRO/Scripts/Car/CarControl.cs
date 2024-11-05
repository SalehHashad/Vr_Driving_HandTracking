using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CarControl : MonoBehaviour 
{
	private CarMove m_Car;

	private void Awake()
	{
		m_Car = GetComponent<CarMove>();
	}


	private void FixedUpdate()
	{

	}
}
