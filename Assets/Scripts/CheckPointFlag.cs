using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointFlag : MonoBehaviour {

    private Animator anim;
    private AudioManager am;


    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
        am = FindObjectOfType<AudioManager>();

        anim.SetBool("Rise", false);
	}

    public void Rise()
    {
        am.Play("Flag");
        anim.SetBool("Rise", true);
        


        GetComponent<Collider>().enabled = false;
    }
}
