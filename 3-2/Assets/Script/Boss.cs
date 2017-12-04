using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

[RequireComponent(typeof(AudioSource))]
public class Boss : MonoBehaviour {

    
    public GameObject PlayerPos;
    public GameObject groundStinger; //땅에서 나오는 가시
    public GameObject gorundStingerR; 
    public GameObject World_Stinger;
    public GameObject Stones;
    public GameObject Baby; // 새끼들
    public GameObject LastBaby;
    public GameObject[] StoneDrop = new GameObject[6];
    public GameObject[] StoneDrop02 = new GameObject[5];
    public GameObject[] BabyPos = new GameObject[5];
    public GameObject[] BloodPos = new GameObject[2];
    public GameObject Blood_effect;
    public GameObject Uprising_Effect; // 올라오는 이팩트
    public GameObject Crack;


    public SpriteRenderer Shadow_Effect; // 그림자

    /// <summary>
    /// 오디오
    /// </summary>
    public AudioSource D_Source;
    public AudioSource H_Source;
    public AudioClip Shout; // 1페이즈 가시패턴
    public AudioClip Shout02; // 2페이즈 가시패턴
    public AudioClip UprisingSound; //올라올때
    public AudioClip Burrow; //들어갈때
    public AudioClip Rush; //돌진
    public AudioClip HitSound; //맞을때
    public AudioClip Intro; //게임 시작할때
    public AudioClip Intro2; // 페이즈2 넘어갈때
    public AudioClip Intro3; // 페이즈3갈때
    public bool LastScene;

    SkeletonAnimator skeleton;
    Camera_Shake CShacke;
    Animator ani;
    Player player;
    Rigidbody2D rigid;
    Camera_zoom Zoom;
    public Collider2D coll;
    public Collider2D RushColl;
    

    public float Boss_HP = 1500f;
    int RandPattern;
    int RandPattern02;
    int StingerNum;
    public int BabyNum;

    public bool SleepOn; // 이때부터 행동

    bool HozCheck;
    public bool RushCheck;
    bool WallCheck;
    bool Hit_effect;
    bool RushHitCheck; //맞았는지 안맞았는지
    bool Page02Check;
    public int paseCheck; // 1, 2, 3 = 페이즈 1,2,3

    public enum BOSSSTATE { SLEEP, IDLE, UPRISING, RUSH, SHOUT, SHOUT2, WAIT, MOVE, DEATH, PAGE03, PAGE02,BABY }
    BOSSSTATE bossstate = BOSSSTATE.SLEEP;

    // Use this for initialization
    void Awake () {
        StartCoroutine(FSM()); //Pattern2()
        StartCoroutine(Pattern());
        StartCoroutine(Pattern2());
        player = GameObject.Find("Player").GetComponent<Player>();
        Zoom = GameObject.Find("Main Camera").GetComponent<Camera_zoom>();
        CShacke = GameObject.Find("Main Camera").GetComponent<Camera_Shake>();
        ani = GetComponent<Animator>();
        skeleton = GetComponent<SkeletonAnimator>();
        rigid = GetComponent<Rigidbody2D>();
        CShacke.enabled = false;
        Page02Check = false;


    }

    IEnumerator FSM()
    {
        while (true)
        {
            switch (bossstate)
            {
                case BOSSSTATE.SLEEP:

                    if (SleepOn)
                    {
                        bossstate = BOSSSTATE.IDLE;
                    }

                    break;

                case BOSSSTATE.IDLE:
                    StartCoroutine(BossMove());
                    RushColl.enabled = false;
                    if (paseCheck == 3)
                    {
                        D_Source.clip = Intro3;
                        D_Source.Play();
                        bossstate = BOSSSTATE.PAGE03;
                        coll.enabled = false;
                    }
                    break;

                case BOSSSTATE.MOVE:
                    break;

                case BOSSSTATE.UPRISING: // 솟아오르기 (땅속으로 사라진 뒤 1초 후 플레이어가 있던 위치에 솟아오름)
                    if (paseCheck == 1)
                    {
                        StartCoroutine(Uprising(PlayerPos.transform.position));
                        D_Source.Stop();
                        D_Source.clip = Burrow;
                        D_Source.Play();
                    }
                    else if(paseCheck ==2)
                    {
                        StartCoroutine(Uprising(PlayerPos.transform.position));
                        D_Source.Stop();
                        D_Source.clip = Burrow;
                        D_Source.Play();
                    }
                    bossstate = BOSSSTATE.WAIT;
                    break;

                case BOSSSTATE.RUSH: // 덮치기 (플레이어가 있는 방향으로 빠르게 이동)
                    if(paseCheck == 1)
                    {
                        D_Source.clip = Rush;
                        D_Source.Play();
                        StartCoroutine(Rush_P());
                    }
                    
                    else if(paseCheck ==2)
                    {
                        D_Source.clip = Rush;
                        D_Source.Play();
                        StartCoroutine(Rush_Pase2());
                    }
                    bossstate = BOSSSTATE.WAIT;
                    break;

                case BOSSSTATE.SHOUT: // 포효1 (플레이어가 서있는 위치에 0.8초마다 가시가 솟아오름)
                    ani.SetBool("Shoting", true);
                    if (paseCheck == 1)
                    {
                        D_Source.clip = Shout;
                        D_Source.Play();
                    }
                    else if (paseCheck == 2)
                    {
                        D_Source.clip = Shout02;
                        D_Source.Play();
                    }
                    StingerNum = 0;
                    bossstate = BOSSSTATE.WAIT;
                    break;

                case BOSSSTATE.DEATH:
                    break;
                case BOSSSTATE.WAIT:
                    break;
                case BOSSSTATE.PAGE03:
                    
                    StartCoroutine(Pasge03_boss());
                    ani.SetTrigger("PageChange03");
                    bossstate = BOSSSTATE.WAIT;
                    break;

                case BOSSSTATE.PAGE02: //페이즈2
                    D_Source.clip = Intro2;
                    D_Source.Play();
                    ani.SetTrigger("PageChange02");
                    Invoke("Page2end", 1.5f);
                    bossstate = BOSSSTATE.WAIT;
                    break;

                case BOSSSTATE.BABY:
                    StartCoroutine(Baby_boss());
                    transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                    bossstate = BOSSSTATE.WAIT;
                    break;
            }
            yield return null;
        }
    }

    IEnumerator Pattern()
    {
        if (bossstate == BOSSSTATE.IDLE && paseCheck ==1)
        {
            RandPattern = Random.Range(0, 10);
            if (RandPattern == 0 || RandPattern == 1 || RandPattern == 2 || RandPattern == 3) // 덮치기 
            {
                bossstate = BOSSSTATE.RUSH;
            }

            else if (RandPattern == 4 || RandPattern == 5) //솟아오르기 
            {
                bossstate = BOSSSTATE.UPRISING;
            }

            else if (RandPattern == 6 || RandPattern == 7 || RandPattern == 8 || RandPattern == 9) //포효 1 
            {
                bossstate = BOSSSTATE.SHOUT;
            }
        }
        yield return new WaitForSeconds(4f);

        StartCoroutine(Pattern());
    }

    IEnumerator Pattern2()
    {
        if (bossstate == BOSSSTATE.IDLE && paseCheck ==2 && Page02Check)
        {
            RandPattern02 = Random.Range(0, 10);
            if (RandPattern02 == 0 || RandPattern02 == 1 || RandPattern02 == 2 ) // 덮치기 
            {
                bossstate = BOSSSTATE.RUSH;
            }

            else if (RandPattern02 == 3 || RandPattern02 == 4) //솟아오르기 
            {
                bossstate = BOSSSTATE.UPRISING;
            }

            else if (RandPattern02 == 5 || RandPattern02 == 6 || RandPattern02 == 7 || RandPattern02 == 8 || RandPattern02 == 9) //포효 1 
            {
                bossstate = BOSSSTATE.SHOUT;
            }
            
        }
        yield return new WaitForSeconds(4f);
        StartCoroutine(Pattern2());
    }

    public void Page2end()
    {
        paseCheck = 2;
        bossstate = BOSSSTATE.IDLE;
        Page02Check = true;
    }


    void BossSpikeing()
    {
        ani.SetBool("Shoting", false);
        ani.SetBool("Spikeing", true);
        Vector3 StingerPos = new Vector3(PlayerPos.transform.position.x, -5.777819f, PlayerPos.transform.position.z);
        if(paseCheck ==1)
        {
            StartCoroutine(Shout_Stinger(StingerPos));
        }
        else if(paseCheck ==2)
        {
            StartCoroutine(WorldStinger());
        }
    }

    IEnumerator Shout_World() 
    {
        Vector3 World_pos = new Vector3(12.0f, -5.777819f, 0f);
        CShacke.enabled = true;
        CShacke.shake = 1f;
        Invoke("StopShake", 0.9f);
        yield return new WaitForSeconds(1f);
        Instantiate(World_Stinger, World_pos, Quaternion.identity);
        yield return new WaitForSeconds(1f);
        ani.SetBool("Spikeing", false);
        bossstate = BOSSSTATE.IDLE;
    }

    IEnumerator WorldStinger() //2페이즈 전체 공격 
    {
        Vector3 StingerPosR = new Vector3(transform.position.x+3, transform.position.y, transform.position.z);
        Vector3 StingerPosL = new Vector3(transform.position.x-3, transform.position.y, transform.position.z);

        yield return new WaitForSeconds(1f);
       
            for (int i = 0; i <7; i++)
            {
                StingerPosR = new Vector3(StingerPosR.x + 3, StingerPosR.y, StingerPosR.z);
                StingerPosL = new Vector3(StingerPosL.x + -3, StingerPosL.y, StingerPosL.z);
                Instantiate(gorundStingerR, StingerPosR, Quaternion.identity);
                Instantiate(groundStinger, StingerPosL, Quaternion.identity);
                CShacke.enabled = true;
                CShacke.shake = 0.3f;
                Invoke("StopShake", 0.3f);
                yield return new WaitForSeconds(0.3f);
        }
       
        yield return new WaitForSeconds(1f);
        ani.SetBool("Spikeing", false);
        bossstate = BOSSSTATE.IDLE;
    }


    IEnumerator Shout_Stinger(Vector3 pos)
    {
        StingerNum += 1;
        yield return new WaitForSeconds(0.8f);
        if(PlayerPos.transform.position.x > transform.position.x)
        {
            Instantiate(gorundStingerR, pos, Quaternion.identity);
        }

        else if (PlayerPos.transform.position.x < transform.position.x)
        {
            Instantiate(groundStinger, pos, Quaternion.identity);
        }
        
        if(StingerNum < 5)
        {
            StartCoroutine(Shout_Stinger(new Vector3(PlayerPos.transform.position.x, -5.777819f, PlayerPos.transform.position.z)));
        }

        else if(StingerNum == 5)
        {
            ani.SetBool("Spikeing", false);
            yield return new WaitForSeconds(0.8f);
            bossstate = BOSSSTATE.IDLE;
        }
    }

    IEnumerator Baby_boss() // 작은 보스 생성
    {
        do
        {
            BabyNum = Random.Range(0, 5);
            if (Boss_HP <= 15)
            {
                LastScene = true;
                if (transform.position.x > PlayerPos.transform.position.x)
                { Instantiate(LastBaby, BabyPos[3].transform.position, Quaternion.identity); }
                else if (transform.position.x < PlayerPos.transform.position.x)
                { Instantiate(LastBaby, BabyPos[0].transform.position, Quaternion.identity); }
                break;
            }
            else
            {
                Instantiate(Baby, BabyPos[BabyNum].transform.position, Quaternion.identity);
            }
            Boss_HP -= 15;
            yield return new WaitForSeconds(1f);
        } while (true);

        do
        {
            if (Boss_HP <= 15) break;
            StartCoroutine(Drop_Stone());
            yield return new WaitForSeconds(2.5f);
        } while (true);
    }

    IEnumerator Drop_Stone() //돌 떨어짐
    {
        int[] randArray = new int[6];
        bool isSame;
        int randPos =0;
        randPos = Random.Range(0, 2);
        if (randPos == 0)
        {
            for (int i = 0; i < 6; i++)
            {
                while (true)
                {
                    randArray[i] = Random.Range(0, 6);
                    isSame = false;
                    for (int j = 0; j < i; ++j)
                    {
                        if (randArray[j] == randArray[i])
                        {
                            isSame = true;
                            break;
                        }
                    }
                    if (!isSame) break;
                }
                Instantiate(Stones, StoneDrop[randArray[i]].transform.position, Quaternion.identity);
                yield return new WaitForSeconds(0.1f);
            }
        }

        else if(randPos == 1)
        {
            for (int i = 0; i < 5; i++)
            {
                while (true)
                {
                    randArray[i] = Random.Range(0, 5);
                    isSame = false;
                    for (int j = 0; j < i; ++j)
                    {
                        if (randArray[j] == randArray[i])
                        {
                            isSame = true;
                            break;
                        }
                    }
                    if (!isSame) break;
                }
                Instantiate(Stones, StoneDrop02[randArray[i]].transform.position, Quaternion.identity);
                yield return new WaitForSeconds(0.1f);
            }
        }

    }

    public int[] getRandomInt(int length, int min, int max)
    {
        int[] randArray = new int[length];
        bool isSame;

        for (int i = 0; i < length; ++i)
        {
            while (true)
            {
                randArray[i] = Random.Range(min, max);
                isSame = false;

                for (int j = 0; j < i; ++j)
                {
                    if (randArray[j] == randArray[i])
                    {
                        isSame = true;
                        break;
                    }
                }
                if (!isSame) break;
            }
        }
        return randArray;
    }

    IEnumerator Pasge03_boss()
    {
        Vector3 MoveVec = Vector3.zero;
        Vector3 StopPos = new Vector3(0f, transform.position.y, transform.position.z);
        if(transform.position.x > 0f)
        {
            transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        }

        else if(transform.position.x < 0f)
        {
            transform.localScale = new Vector3(-1.2f, 1.2f, 1.2f);
        }
        do
        {
            transform.position = Vector3.MoveTowards(transform.position, StopPos, Time.deltaTime*2f);
            if(transform.position == StopPos)
            {
                StartCoroutine(Crying_Boss());
                ani.SetTrigger("Idle03");
                break;
            }
            yield return null;
        } while (true);
       
    }

    IEnumerator Crying_Boss()
    {
        yield return new WaitForSeconds(1f);
        bossstate = BOSSSTATE.BABY;
        yield return null;
    }
   

    IEnumerator BossMove() // 보스 움직임
    {
        Vector3 MoveVec = Vector3.zero;
        string move = "";
        Vector3 playerPos = PlayerPos.transform.position;

        if (playerPos.x < transform.position.x - 0.5f)
        {
            move = "Left";
        }

        else if (playerPos.x > transform.position.x + 0.5f)
        {
            move = "Right";
        }

        if (move == "Left")
        {
            MoveVec = Vector3.left;
            transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            HozCheck = true;
        }

        else if (move == "Right")
        {
            MoveVec = Vector3.right;
            transform.localScale = new Vector3(-1.2f, 1.2f, 1.2f);
            HozCheck = false;
        }

        transform.position += MoveVec * 1 * Time.deltaTime;
        yield return null;

    }
    IEnumerator Crack_Spwan(Vector3 Spwan)
    {
        if (RushCheck)
        { 
            Instantiate(Crack, new Vector3(Spwan.x, -5.99f, Spwan.z), Quaternion.identity);
            yield return new WaitForSeconds(0.3f);
            StartCoroutine(Crack_Spwan(new Vector3(transform.position.x, -5.99f, transform.position.z)));
        }
        yield return null;
    }
    IEnumerator Rush_P() //덮치기
    {
        Vector3 RushVec = Vector3.zero;
        Vector3 playerPos = player.transform.position;
        ani.SetBool("Rushing", true);
        yield return new WaitForSeconds(1.5f);
        RushColl.enabled = true;
        RushCheck = true;
        StartCoroutine(Crack_Spwan(new Vector3(transform.position.x, -5.99f, transform.position.z)));
        var t = 0f;
        do
        {
            if (HozCheck)
            {
                RushVec = Vector3.left;
                transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            }
            else if (!HozCheck)
            {
                RushVec = Vector3.right;
                transform.localScale = new Vector3(-1.2f, 1.2f, 1.2f);
            }
            t += Time.deltaTime * 2f;
            transform.position += RushVec * 15 * Time.deltaTime;
            if(WallCheck) break; // 벽에 닿으면 멈춤
            if (RushHitCheck) // 플레이어에 닿으면 멈춤;
            {
                RushHitCheck = false;
                RushCheck = false;
                yield return new WaitForSeconds(0.5f);
                ani.SetBool("Rushing", false);
                bossstate = BOSSSTATE.IDLE;
                break;
            }
            yield return null;
        } while (t < 2);

        if (t >= 2)
        {
            RushCheck = false;
            yield return new WaitForSeconds(0.5f);
            ani.SetBool("Rushing", false);
            bossstate = BOSSSTATE.IDLE;
        }
    }

    IEnumerator Rush_Pase2() // 2페이즈 돌진
    {
        Vector3 RushVec = Vector3.zero;
        Vector3 playerPos = player.transform.position;
        ani.SetBool("Rushing", true);
        yield return new WaitForSeconds(1.5f);
        RushColl.enabled = true;
        RushCheck = true;
        float Spead = 10;

        if (HozCheck)
        {
            do
            {
                RushVec = Vector3.left;
                transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                transform.position += RushVec * Spead * Time.deltaTime;
                Spead += 1f;
                yield return null;
            } while (!WallCheck);
        }

        else if (!HozCheck)
        {
            do
            {
                RushVec = Vector3.right;
                transform.localScale = new Vector3(-1.2f, 1.2f, 1.2f);
                transform.position += RushVec * Spead * Time.deltaTime;
                Spead += 1f;
                yield return null;
            } while (!WallCheck);
        }
        if (WallCheck)
        {
            RushCheck = false;
            ani.SetBool("Rushing", false);
            StartCoroutine(Drop_Stone());
            bossstate = BOSSSTATE.UPRISING;
            WallCheck = false;
        }
    }

    IEnumerator Uprising(Vector3 End) // 들어갔다 나오기
    {
        ani.SetBool("Digging", true);
        Shadow_Effect.enabled = false;
        yield return new WaitForSeconds(2f);
        Instantiate(Uprising_Effect, new Vector3(End.x, -6.03335f, End.z), Quaternion.identity);
        yield return new WaitForSeconds(0.2f);
        transform.position = new Vector3(End.x, transform.position.y + 20, transform.position.z);
        D_Source.clip = UprisingSound;
        D_Source.Play();
        ani.SetBool("Digging", false);
        yield return new WaitForSeconds(0.5f);
        Shadow_Effect.enabled = true;
        bossstate = BOSSSTATE.IDLE;
    }

    IEnumerator Hit_Image() // 맞는 이미지
    {
        Hit_effect = true;
        int Hited = 0;
        while ( Hited < 3)
        {
            if (Hited % 2 == 0)
            {
                skeleton.GetComponent<SkeletonAnimator>().skeleton.a = 0.8f;
            }
            else
                skeleton.GetComponent<SkeletonAnimator>().skeleton.a = 1f;
            yield return new WaitForSeconds(0.1f);
            Hited++;
        }
        skeleton.GetComponent<SkeletonAnimator>().skeleton.a = 1f;
        yield return new WaitForSeconds(0.2f);
        Hit_effect = false;
    }

    public void Boss_Dig() //들어가는 이미지가 끝난후 아래로 내려준다.
    {
        transform.position = new Vector3(transform.position.x, transform.position.y - 20, transform.position.z);
        coll.enabled = false;
    }

    public void Boss_Uprising()
    {
        coll.enabled = true;
    }

    void FixedUpdate()
    {
        float Cast = Mathf.Abs(transform.position.x - PlayerPos.transform.position.x);

        if (Boss_HP >= 750)
        {
            paseCheck = 1;
        }

        else if ((Boss_HP < 750 && Boss_HP > 150) && bossstate == BOSSSTATE.IDLE)
        {
            if (paseCheck == 1)
            {
                bossstate = BOSSSTATE.PAGE02;
            }
        }

        else if (Boss_HP <= 150 && bossstate == BOSSSTATE.IDLE)
        {
            paseCheck = 3;
        }

  
        // 여긴 못지나가게 체크
        if ((player.RollingCheck && Cast < 4.8f))
        {
            coll.enabled = false;
        }

        else if (!player.RollingCheck && Cast > 4.8f && paseCheck !=3)
        {
            coll.enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Wall" && paseCheck ==2 && RushCheck)
        {
            WallCheck = true;
            CShacke.enabled = true;
            CShacke.shake = 0.5f;
            Invoke("StopShake", 0.5f);
        }

        if(other.gameObject.tag == "Player" && RushCheck && !player.RollingCheck)
        {
            RushHitCheck = true;
        }

        if (other.gameObject.tag == "Attackcoll" && paseCheck !=3)
        {
            Boss_HP -= player.AttackDamage;
            StartCoroutine(Hit_Image());
            H_Source.Play();
        }
    }


 

    void StopShake()
    {
        CShacke.enabled = false;
    }
}
