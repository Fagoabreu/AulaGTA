using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Shot))]
[RequireComponent(typeof(Animator))]
public class MechanicPlayer : MonoBehaviour {
    Shot shot;
    private Animator animator;

    private bool onPistolIdle;
    private bool onFirePistol;

    [SerializeField] private GameObject pistolObj;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        shot = GetComponent<Shot>();
	}
	
	// Update is called once per frame
	void Update () {
        OnPistolIdle();
	}

    void OnPistolIdle()
    {
        //toda vez que apertar o botão, ativa ou desativa a variavel pistolIdle
        //if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), StaticInput.Aim)))
        if(Input.GetButtonDown(StaticInput.Aim))
        {
            onPistolIdle = !onPistolIdle;
        }

        if (onPistolIdle)
        {
            if (!onFirePistol)
            {
                animator.SetBool(StaticAnimation.IdlePistol, true);
            }
            else
            {
                animator.SetBool(StaticAnimation.IdlePistol, true);
            }
            if (Input.GetButton(StaticInput.Fire))
            {
                StartCoroutine(FirePistol());
                
            }
        }
        else
        {
            animator.SetBool(StaticAnimation.IdlePistol, false);
        }
    }

    IEnumerator FirePistol()
    {
        onFirePistol = true;
        animator.SetBool(StaticAnimation.FirePistol, onFirePistol);
        yield return new WaitForSeconds(1f);
        onFirePistol = false;
        animator.SetBool(StaticAnimation.FirePistol, onFirePistol);
    }

    #region Events
    public void OffFirePistol()
    {
        animator.SetBool(StaticAnimation.FirePistol, false);
    }

    public void OnWeapon(int num)
    {
        if (num == 0)
        {
            pistolObj.SetActive(false);
        }else if (num == 1)
        {
            pistolObj.SetActive(true);
        }
    }

    public void FireEffect()
    {
        shot.OnShot();
    }
    #endregion
}
