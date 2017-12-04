using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Camera_zoom : MonoBehaviour {

    Player player;
    Boss boss;
    public Camera CameraZ;
    Transform Camera;
    public GameObject Boss;
    float MaxSize = 8.5f;
    public Image Name;
    float MinSize = 3f;
    bool Checked;

    // Use this for initialization
    void Awake () {
        player = GameObject.Find("Player").GetComponent<Player>();
        boss = GameObject.Find("Boss").GetComponent<Boss>();
        Camera = Boss.transform;
	}
	
	// Update is called once per frame
	void Update () {

        if (player.CheckPoint )
        {
            transform.position = Vector3.Lerp(transform.position, Camera.position, 2f * Time.deltaTime);
            transform.position = new Vector3(12.84f, 0f, -10f); // 체크시 카메라 좌표 이동
            StartCoroutine(Zoomin()); //줌인 코루틴으로
        }
    }

    IEnumerator Zoomin()
    {
        CameraZ.orthographicSize = Mathf.Lerp(CameraZ.orthographicSize, MinSize, Time.deltaTime / 2.5f); //줌인
        
        yield return new WaitForSeconds(0.3f); //3초뒤에
        yield return new WaitForSeconds(2f);
        CameraZ.orthographicSize = Mathf.Lerp(CameraZ.orthographicSize, MaxSize, Time.deltaTime * 9f); //줌아웃
        yield return new WaitForSeconds(0.6f);
        boss.SleepOn = true;
        player.CheckPoint = false;
    }
}
