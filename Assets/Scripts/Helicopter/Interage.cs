using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CharacterController.ThirdPerson;

public class Interage : MonoBehaviour {

    [SerializeField] HeliControl helicopter;

    private ThirdPersonCharacter personCharacter;
    private ThirdPersonUserController personController;

    private GameObject camPlayer;
    private GameObject player;
    private Rigidbody playerRigidbody;
    private CapsuleCollider playerCap;

    public GameObject camHelicpter;

	// Use this for initialization
	void Start () {
        camPlayer = GameObject.FindGameObjectWithTag("camPlayer");
        player = GameObject.FindGameObjectWithTag("Player");
        personCharacter = (ThirdPersonCharacter)FindObjectOfType(typeof(ThirdPersonCharacter));
        personController = (ThirdPersonUserController)FindObjectOfType(typeof(ThirdPersonUserController));
        playerRigidbody = player.GetComponent<Rigidbody>();
        playerCap = player.GetComponent<CapsuleCollider>();
    }
	
	// Update is called once per frame
	void Update () {
        if (helicopter.onUse)
        {
            camPlayer.SetActive(false);
            camHelicpter.SetActive(true);
            player.transform.position = transform.position;
            personCharacter.enabled = false;
            personController.enabled = false;

            playerRigidbody.isKinematic = true;
            playerCap.isTrigger = true;
        }
        else
        {
            camPlayer.SetActive(true);
            camHelicpter.SetActive(false);

            personCharacter.enabled = true;
            personController.enabled = true;

            playerRigidbody.isKinematic = false;
            playerCap.isTrigger = false;
        }
		
	}

    private void OnTriggerStay(Collider other)
    {
        if(Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), StaticInput.Interact))){
            helicopter.onUse = !helicopter.onUse;
        }
    }
}
