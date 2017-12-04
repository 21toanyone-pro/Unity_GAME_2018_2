using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour {

    Boss boss;
    CollDestroy coll;
    public AudioSource BGM;
    public AudioClip Page01; // p1
    public AudioClip Page02; // p2
    public AudioClip Page03; // p3
    public AudioClip Last;
    float Times = 0.5f;
    float Times2 = 0.5f;
    float Times3 = 0.5f;
    float Times4 = 0.5f;
    float Volume02 =0;

    bool Check01;
    bool Check02;
    bool Check03;
    bool Check04;
    // Use this for initialization
    void Start () {
        boss = GameObject.Find("Boss").GetComponent<Boss>();
        coll = GameObject.Find("Check_Point").GetComponent<CollDestroy>();
	}
	
	// Update is called once per frame
	void Update () {

        if(coll.BossRoom && !Check01)
        {
            if (Times3 >= 0)
            {
                Times3 -= 0.01f;
                BGM.volume = Times3;
            }
            if (Times3 <= 0)
            {
                StartCoroutine(BGM_Sound());
                BGM.clip = Page01;
                BGM.Play();
                Check01 = true;
            }
        }

        else if (boss.paseCheck == 2 && !Check02)
        {
            if(Times >=0)
            {
                Times -= 0.01f;
                BGM.volume = Times;
            }
            
            if (Times <= 0)
            {
                StartCoroutine(BGM_Sound());
                BGM.clip = Page02;
                BGM.Play();
                Check02 = true;
            }
        }

        else if (boss.paseCheck == 3 && !Check03)
        {
            if (Times2 >= 0)
            {
                Times2 -= 0.01f;
                BGM.volume = Times2;
            }

            if(Times2 <= 0)
            {
                StartCoroutine(BGM_Sound());
                BGM.clip = Page03;
                BGM.Play();
                Check03 = true;
            }
        }

        else if (boss.LastScene && !Check04)
        {
            if (Times4 >= 0)
            {
                Times4 -= 0.01f;
                BGM.volume = Times4;
            }

            if (Times4 <= 0)
            {
                StartCoroutine(BGM_Sound());
                BGM.clip = Last;
                BGM.Play();
                Check04 = true;
            }

        }
    }

    IEnumerator BGM_Sound()
    {
        do
        {
            BGM.volume = Volume02;
            Volume02 += 0.01f;
            if (Volume02 >= 0.5f)
            {
                Volume02 = 0;
                break;
            }
            yield return new WaitForSeconds(0.1f);
        } while (true);
        
    }
}
