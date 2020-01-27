using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenManager : MonoBehaviour {

    bool spacePressed;
    private AudioManager am;

    // Use this for initialization
    void Start () {
        spacePressed = false;
        am = FindObjectOfType<AudioManager>();
    }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Jump") && !spacePressed)
        {
            am.Stop("Title");
            am.Play("Theme");
            SceneManager.LoadScene(0);
        }
	}
}
