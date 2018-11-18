using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour {

    [SerializeField] private float damage = 10f;
    [SerializeField] private float range = 100f;
    [SerializeField] private GameObject soundShot;
    [SerializeField] private GameObject impactEffect;
    [SerializeField] private Transform pivotShot;
    [SerializeField] private ParticleSystem shotEffect;

    public void OnShot()
    {
        RaycastHit hit;
        shotEffect.Play();
        if(Physics.Raycast(pivotShot.position, pivotShot.forward,out hit, range))
        {
            Debug.Log(hit.transform.name);
        }

        GameObject impact = Instantiate(impactEffect, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
        Destroy(impact, 0.5f);
        GameObject sound = Instantiate(soundShot, transform.position, Quaternion.identity);
        Destroy(sound, 2f);
    }

}
