using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Con : MonoBehaviour {

    
	// Use this for initialization
	void Start () {
        setupCamera();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void setupCamera()
    {
        //가로 화면 비율
        float targetWidthAspect = 16.0f;

        //세로 화면 비율
        float targetHeightAspect = 9.0f;

        //메인 카메라
        Camera mainCamera = Camera.main;
        Camera BackCamera = GameObject.Find("BackGround Camera").GetComponent<Camera>();

        Camera UICameras = GameObject.Find("UI Camera").GetComponent<Camera>();

        mainCamera.aspect = targetWidthAspect / targetHeightAspect;
        BackCamera.aspect = targetWidthAspect / targetHeightAspect;
        UICameras.aspect = targetWidthAspect / targetHeightAspect;

        float widthRatio = (float)Screen.width / targetWidthAspect;
        float heightRatio = (float)Screen.height / targetHeightAspect;

        float heightadd = ((widthRatio / (heightRatio / 100)) - 100) / 200;
        float widthtadd = ((heightRatio / (widthRatio / 100)) - 100) / 200;

        // 16_10비율보다 가로가 짦다면(4_3 비율)
        // 16_10비율보다 세로가 짧다면(16_9 비율)
        // 시작 지점을 0으로 만들어준다
        if (heightRatio > widthRatio)
            widthtadd = 0.0f;
        else
            heightadd = 0.0f;


        mainCamera.rect = new Rect(
            mainCamera.rect.x + Mathf.Abs(widthtadd),
            mainCamera.rect.y + Mathf.Abs(heightadd),
            mainCamera.rect.width + (widthtadd * 2),
            mainCamera.rect.height + (heightadd * 2));
        BackCamera.rect = new Rect(
           BackCamera.rect.x + Mathf.Abs(widthtadd),
           BackCamera.rect.y + Mathf.Abs(heightadd),
           BackCamera.rect.width + (widthtadd * 2),
           BackCamera.rect.height + (heightadd * 2));
        UICameras.rect = new Rect(
           UICameras.rect.x + Mathf.Abs(widthtadd),
           UICameras.rect.y + Mathf.Abs(heightadd),
           UICameras.rect.width + (widthtadd * 2),
           UICameras.rect.height + (heightadd * 2));

    }
}
