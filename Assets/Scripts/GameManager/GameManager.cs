using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    [SerializeField] Animator animator;
    #region HP
    [SerializeField] Image hpImage;
    [SerializeField] float curHealth;
    [SerializeField] float maxHeath;

    private float curTimeDie;
    [SerializeField] float maxTimeDie;

    [SerializeField] GameObject dieShowText;
    [SerializeField] GameObject soundDie;
    [SerializeField] bool onDieNoRepeat;

    #endregion



    // Use this for initialization
    void Start () {
        curHealth = maxHeath;
        dieShowText.SetActive(false);
        curTimeDie = maxTimeDie;
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
            if (!onDieNoRepeat)
            {
                animator.SetBool(StaticAnimation.Die,true);
                soundDie.SetActive(true);
                curTimeDie -= Time.deltaTime;
                if (curTimeDie <= 0)
                {
                    dieShowText.SetActive(true);
                    curTimeDie = maxTimeDie;
                    onDieNoRepeat = true;
                }
            }
        }
        else
        {
            dieShowText.SetActive(false);
            onDieNoRepeat = false;
            animator.SetBool(StaticAnimation.Die, false);
        }
    }
}
