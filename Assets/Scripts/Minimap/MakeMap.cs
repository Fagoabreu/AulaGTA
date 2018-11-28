using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MakeMap : MonoBehaviour {

    public Image image;

	// Use this for initialization
	void Start () {
        MiniMapController.RegisterMapObjects(this.gameObject, image);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDestroy()
    {
        MiniMapController.RemoveMapObjects(this.gameObject);
    }
}
