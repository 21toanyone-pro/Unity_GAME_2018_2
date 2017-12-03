using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Loding : MonoBehaviour {

    public Image profressBar;

    // Use this for initialization
    void Start () {
        StartCoroutine(LoadScene());
    }
	
	// Update is called once per frame


    IEnumerator LoadScene()
    {
        yield return null;

        AsyncOperation oper = SceneManager.LoadSceneAsync("Stage01");

        oper.allowSceneActivation = false;

        float timer = 0.0f;

        while (!oper.isDone)
        {
            yield return null;

            timer += Time.deltaTime;

            if (oper.progress >= 0.9f)
            {
                profressBar.fillAmount = Mathf.Lerp(profressBar.fillAmount, 1f, timer);

                if (profressBar.fillAmount == 1.0f)
                    oper.allowSceneActivation = true;
            }

            else
            {
                profressBar.fillAmount = Mathf.Lerp(profressBar.fillAmount, oper.progress, timer);
                if (profressBar.fillAmount >= oper.progress)
                {
                    timer = 0f;
                }
            }
        }
    }
}
