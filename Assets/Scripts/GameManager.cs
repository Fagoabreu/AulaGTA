using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    #region HP
    [SerializeField] Image hpImage;
    [SerializeField] float curHealth;
    [SerializeField] float maxHeath;

    #endregion


    [SerializeField] GameObject dieShowText;
    // Use this for initialization
    void Start () {
        curHealth = maxHeath;
	}
	
	// Update is called once per frame
	void Update () {
        HealthChage();

    }
    void HealthChage()
    {
        float amount = curHealth / maxHeath;
        hpImage.fillAmount = amount;

        if(curHealth <= 0)
        {
            dieShowText.SetActive(true);
        }
        else
        {
            dieShowText.SetActive(false);
        }
    }
}
