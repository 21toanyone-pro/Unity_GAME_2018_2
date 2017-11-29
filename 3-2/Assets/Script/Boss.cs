using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

public class Boss : MonoBehaviour {

    public GameObject PlayerPos;
    public GameObject groundStinger; //땅에서 나오는 가시
    public GameObject gorundStingerR; 
    public GameObject World_Stinger;
    public GameObject Stones;
    public GameObject Baby; // 새끼들
    public GameObject[] StoneDrop = new GameObject[11];
    public GameObject[] BabyPos = new GameObject[5];
    public GameObject[] BloodPos = new GameObject[2];
    public GameObject Blood_effect;
    SkeletonAnimator skeleton;
    Camera_Shake CShacke;
    Animator ani;
    Player player;
    public Collider2D coll;
    public Collider2D RushColl;

    public float Boss_HP = 2000f;
    int RandPattern;
    int RandPattern02;
    int StingerNum;
    public int BabyNum;

    bool HozCheck;
    public bool RushCheck;
    bool WallCheck;
    bool Hit_effect;
    bool RushHitCheck; //맞았는지 안맞았는지

    int paseCheck; // 1, 2, 3 = 페이즈 1,2,3

    public enum BOSSSTATE { SLEEP, IDLE, UPRISING, RUSH, SHOUT, SHOUT2, WAIT, MOVE, DEATH, PAGE03, PAGE02,BABY }
    BOSSSTATE bossstate = BOSSSTATE.IDLE;

    // Use this for initialization
    void Awake () {
        StartCoroutine(FSM()); //Pattern2()
        StartCoroutine(Pattern());
        StartCoroutine(Pattern2());
        player = GameObject.Find("Player").GetComponent<Player>();
        CShacke = GameObject.Find("Main Camera").GetComponent<Camera_Shake>();
        ani = GetComponent<Animator>();
        skeleton = GetComponent<SkeletonAnimator>();
        CShacke.enabled = false;
    }

    IEnumerator FSM()
    {
        while (true)
        {
            switch (bossstate)
            {
                case BOSSSTATE.SLEEP:
                    break;

                case BOSSSTATE.IDLE:
                    StartCoroutine(BossMove());
                    RushColl.enabled = false;
                    if (paseCheck == 3)
                    {
                        bossstate = BOSSSTATE.PAGE03;
                    }
                    break;

                case BOSSSTATE.MOVE:
                    break;

                case BOSSSTATE.UPRISING: // 솟아오르기 (땅속으로 사라진 뒤 1초 후 플레이어가 있던 위치에 솟아오름)
                    if (paseCheck == 1)
                    {
                        StartCoroutine(Uprising(PlayerPos.transform.position));
                    }
                    else if(paseCheck ==2)
                    {
                        StartCoroutine(Uprising(PlayerPos.transform.position));
                    }
                    bossstate = BOSSSTATE.WAIT;
                    break;

                case BOSSSTATE.RUSH: // 덮치기 (플레이어가 있는 방향으로 빠르게 이동)
                    if(paseCheck == 1)
                    {
                        StartCoroutine(Rush_P());
                        RushCheck = true;
                        RushColl.enabled = true;
                    }
                    
                    else if(paseCheck ==2)
                    {
                        StartCoroutine(Rush_Pase2());
                        RushCheck = true;
                        RushColl.enabled = true;
                    }
                    bossstate = BOSSSTATE.WAIT;
                    break;

                case BOSSSTATE.SHOUT: // 포효1 (플레이어가 서있는 위치에 0.8초마다 가시가 솟아오름)
                    ani.SetBool("Shoting", true);
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

                case BOSSSTATE.PAGE02:
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
        if (bossstate == BOSSSTATE.IDLE && paseCheck ==2)
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

    IEnumerator WorldStinger()
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

        else if (PlayerPos.transform.position.x < transform.position.x) //요기요
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
            Instantiate(Baby, BabyPos[BabyNum].transform.position, Quaternion.identity);
            Boss_HP -= 50;
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(Drop_Stone());
            yield return new WaitForSeconds(1f);
            if (Boss_HP == 0) break;
        } while (true);
        
    }

    IEnumerator Drop_Stone() //돌 떨어짐
    {
        int[] randArray = new int[6];
        bool isSame;
        for(int i =0; i< 6; i++)
        {
            while(true)
            {
                randArray[i] = Random.Range(0, 11);
                isSame = false;
                for(int j =0; j< i; ++j)
                {
                    if(randArray[j] == randArray[i])
                    {
                        isSame = true;
                        break;
                    }
                }
                if (!isSame) break;
            }
            Instantiate(Stones, StoneDrop[randArray[i]].transform.position, Quaternion.Euler(0,0,-36f));
        }
        yield return new WaitForSeconds(0.1f);


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
        Vector3 StopPos = new Vector3(12f, transform.position.y, transform.position.z);
        do
        {
            transform.position = Vector3.MoveTowards(transform.position, StopPos, Time.deltaTime);
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
    IEnumerator Rush_P() //덮치기
    {
        Vector3 RushVec = Vector3.zero;
        Vector3 playerPos = player.transform.position;
        var t = 0f;
        do
        {
            if (HozCheck)
            {
                ani.SetBool("Rushing", true);
                RushVec = Vector3.left;
                transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);

            }
            else if (!HozCheck)
            {
                ani.SetBool("Rushing", true);
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
 
        do
        {
            if (HozCheck)
            {
                
                ani.SetBool("Rushing", true);
                RushVec = Vector3.left;
                transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            }
            else if (!HozCheck)
            {
                ani.SetBool("Rushing", true);
                RushVec = Vector3.right;
                transform.localScale = new Vector3(-1.2f, 1.2f, 1.2f);
            }
            transform.position += RushVec * 10 * Time.deltaTime;
            
            yield return null;
        } while (!WallCheck);

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
        yield return new WaitForSeconds(2f);
        transform.position = new Vector3(End.x, transform.position.y + 20, transform.position.z);
        ani.SetBool("Digging", false);
        yield return new WaitForSeconds(0.5f);
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

        else if (Boss_HP < 750 && Boss_HP > 150)
        {
            if (paseCheck == 1)
            {
                bossstate = BOSSSTATE.PAGE02;
            }
            ani.SetTrigger("PageChange02");
        }

        else if (Boss_HP <= 150)
        {
            paseCheck = 3;
        }

  
        // 여긴 못지나가게 체크
        if (player.RollingCheck && Cast < 4.5f || paseCheck == 3)
        {
            coll.enabled = false;
        }

        else if (!player.RollingCheck && Cast > 4.5f && paseCheck !=3)
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

        if(other.gameObject.tag == "Player" && RushCheck)
        {
            RushHitCheck = true;
        }

        if (other.gameObject.tag == "Attackcoll" && paseCheck !=3)
        {
            StartCoroutine(HitEffect());
            Boss_HP -= player.AttackDamage;
            StartCoroutine(Hit_Image());
        }
    }


    IEnumerator HitEffect()
    {
        float Vec = 0f;
        if (transform.position.x > PlayerPos.transform.position.x)
        {
            Vec = -60f;
        }

        if (transform.position.x < PlayerPos.transform.position.x)
        {
            Vec = 60f;
        }
        
        GameObject blood = Instantiate(Blood_effect, BloodPos[1].transform.position, Quaternion.Euler(0, 0, Vec));
        blood.transform.parent = this.transform;
        yield return null;
    }



    void StopShake()
    {
        CShacke.enabled = false;
    }
}
