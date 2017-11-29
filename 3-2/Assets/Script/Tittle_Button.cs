using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tittle_Button : MonoBehaviour {

    public GameObject Select_button;

    int Selectnum;

	// Use this for initialization
	void Awake ()
    {
        Selectnum = 0;
	}
	
	// Update is called once per frame
	void Update ()
    {
        var SelectPos = Select_button.transform.position;

        if(Selectnum == 0)
        {
            Select_button.transform.position = new Vector3(SelectPos.x, -2.72f, SelectPos.z);
        }

        if (Selectnum == 1)
        {
            Select_button.transform.position = new Vector3(SelectPos.x, -6.72f, SelectPos.z);
        }

        if (Input.GetKeyDown(KeyCode.W) && Selectnum == 0 || (Input.GetKeyDown(KeyCode.DownArrow) && Selectnum == 0))
        {
            Selectnum = 1;
        }

        if((Input.GetKeyDown(KeyCode.UpArrow) && Selectnum == 1) || (Input.GetKeyDown(KeyCode.DownArrow) && Selectnum == 1))
        {
            Selectnum = 0;
        }


    }
}
