using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffMesh : MonoBehaviour {

    public SkinnedMeshRenderer[] mesh;
    HeliControl interage;

	// Use this for initialization
	void Start () {
        interage = (HeliControl)FindObjectOfType<HeliControl>();

	}
	
	// Update is called once per frame
	void Update () {
        if (interage.onUse)
        {
            for(int i =0; i < mesh.Length; i++)
            {
                mesh[i].enabled = false;
            }
        }
        else
        {
            for (int i = 0; i < mesh.Length; i++)
            {
                mesh[i].enabled = true;
            }
        }

	}
}
