using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffObj : MonoBehaviour {

    private float curTime;
    [SerializeField] private float MaxTime;
    

	// Use this for initialization
	void Start () {
        curTime = MaxTime;	
	}
	
	// Update is called once per frame
	void Update () {
        curTime -= Time.deltaTime;
        if(curTime <= 0)
        {
            gameObject.SetActive(false);
            curTime = MaxTime;
        }
	}
}
