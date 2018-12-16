using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    [SerializeField] private GameObject fruitPrefab;
    [SerializeField] private GameObject sliderPrefab;

	void Start ()
	{
	    HitObjectManager.instance.fruitPrefab = fruitPrefab;
	    HitObjectManager.instance.sliderPrefab = sliderPrefab;
	}
	
}
