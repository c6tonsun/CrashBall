using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInput : MonoBehaviour {

    public Animator animator;
    private bool inputTaken;

    [SerializeField]
    private GameObject[] PlayButton;
    [SerializeField]
    private GameObject[] ExitButton;


    public enum Select
    {
        preStart = 0,
        play = 1,
        exit = 2
    }
    Select selected = Select.preStart;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        GetInput();
        if(selected == Select.preStart)
        {
            PlayButton[0].SetActive(false);
            PlayButton[1].SetActive(false);
            ExitButton[0].SetActive(false);            
            ExitButton[1].SetActive(false);
        }
        else
        {
            PlayButton[0].SetActive(true);
            ExitButton[0].SetActive(true);
        }
        if (selected == Select.play)
        {
            ExitButton[1].SetActive(false);
            PlayButton[1].SetActive(true);
        }
        if(selected == Select.exit)
        {
            ExitButton[1].SetActive(true);
            PlayButton[1].SetActive(false);
        }
        Debug.Log(selected);
    }

    private void GetInput()
    {
        var input = Input.GetAxis("Vertical");
        if (Mathf.Abs(input)>0f)
        {
            if (selected == Select.play && input<0)
            {
                selected = Select.exit;
            }
            else if (selected == Select.exit && input>0)
            {
                selected = Select.play;
            }
            Debug.Log("W press");
        }
        if (Input.GetButtonDown("Submit"))
        {
            if (selected == Select.play)
            {
                Debug.Log("select play");
                SceneManager.LoadScene(1);
            }
            else if (selected == Select.exit)
            {
                //Application.Quit();
                Debug.Log("application quit");
            }
            else if(selected == Select.preStart)
            {
                animator.SetInteger("StartPresses", 1);
                selected = Select.play;
            }
        }
    }
}
