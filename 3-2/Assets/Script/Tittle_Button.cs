using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tittle_Button : MonoBehaviour {

    public GameObject Select_button;

    int Selectnum;
    public AudioSource Button;

    // Use this for initialization
    void Awake ()
    {
       
        Selectnum = 0;
	}


    public void NextScene()
    {
        Button.Play();
        Invoke("LoadScene", 0.5f);
    }

    void LoadScene()
    {
        SceneManager.LoadScene("Lodinng");
    }

	
	// Update is called once per frame
	void Update ()
    {
        var SelectPos = Select_button.transform.position;

        if(Input.GetKeyDown(KeyCode.Space) && Selectnum == 0)
        {
            Button.Play();
            Invoke("LoadScene", 0.5f);
        }

        if(Selectnum == 0)
        {
            Select_button.transform.position = new Vector3(SelectPos.x, -2.72f, SelectPos.z);
        }

        else if (Selectnum == 1)
        {
            Select_button.transform.position = new Vector3(SelectPos.x, -6.72f, SelectPos.z);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && Selectnum == 0) // 게임스타트에 있고 위 버튼 누를때
        {
            Selectnum = 1;
        }

        else if(Input.GetKeyDown(KeyCode.DownArrow) && Selectnum == 0) // 게임 스타트에 있고 아래 버튼 누를때
        {
            Selectnum = 1;
        }

        else if(Input.GetKeyDown(KeyCode.UpArrow) && Selectnum == 1) // 종료에 있고 위에 버튼 누를때
        {
            Selectnum = 0;
        }

        else if(Input.GetKeyDown(KeyCode.DownArrow) && Selectnum == 1) // 종료에 있을때 아래키 누를때
        {
            Selectnum = 0;
        }


    }
}
