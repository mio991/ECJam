﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
	[System.Serializable]
	public struct AngleBounds{
		public float max;
		public float min;
	}

	public static Planet instance;
	
	public float gravity = 9.8f;
	public LayerMask layerMask;

	[HideInInspector]
	public SeedSocketView[] seedSocketViews;
	public Transform seedSocketsHolder;

	public GameObject socketPrefab;

	public Transform house;
	public Transform forest;

	public AngleBounds[] angleBounds;

	private void Awake()
	{
		if (instance != null)
			DestroyImmediate(instance);
		instance = this;
	}

	// Start is called before the first frame update
	void Start()
    {
		seedSocketViews = GetComponentsInChildren<SeedSocketView>();

		if(GameManager.instance.gameData.generateSockets)
		{
			float radius = seedSocketViews[0].transform.localPosition.magnitude;
			Vector3 scale = seedSocketViews[0].transform.localScale;
			foreach (SeedSocketView s in seedSocketViews)
				Destroy(s.gameObject);
			
			for(int i = 0; i < GameManager.instance.gameData.socketDensity;i++)
			{
				float angle = 360f / GameManager.instance.gameData.socketDensity * i - 180;
				bool flag = true;
				foreach(AngleBounds ab in angleBounds){
					if (angle > ab.min && angle < ab.max)
					{
						flag = false;
						break;
					}
				}
				if(flag){
					Quaternion q = Quaternion.AngleAxis(angle, Vector3.back);
					Vector3 v = (q * Vector3.up) * radius + Vector3.back;
					Transform t = Instantiate(socketPrefab, v, q).transform;
					t.SetParent(seedSocketsHolder, false);
					t.localScale = scale;
				}
			}

			seedSocketViews = GetComponentsInChildren<SeedSocketView>();
		}

		GameManager.InitSockets(seedSocketViews.Length);
		for(int i = 0; i< seedSocketViews.Length;i++){
			seedSocketViews[i].SetSocket(GameManager.instance.gameData.sockets[i]);
		}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public static float GetAngle(Vector3 position){
		Vector3 vec = position - instance.transform.position;
		vec.z = 0;
		return Vector3.SignedAngle(Vector3.up, vec, Vector3.back);
	}
}
