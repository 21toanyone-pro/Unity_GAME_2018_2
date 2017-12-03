using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

[RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour {

    Animator animator;

    public Animator Dust; // 먼지 이펙트

    Rigidbody2D rigid;
    public Collider2D coll;
    public GameObject BossPos;
    public SpriteRenderer Shadow;
    SkeletonAnimator skeleton;
    Boss boss;

    AudioSource PlayerSource;
    public AudioClip SAttack_S;
    public AudioClip WAttack_S01;
    public AudioClip WAttack_S02;
    public AudioClip Rolling_S;
    public AudioClip Defence_S;
    public AudioClip Move_S;
    public AudioClip Jump_S;
    public AudioClip Hit_S;
    public AudioClip Landing_S;


    public float PlayerHP = 100;
    public float PlayerST = 100;

    float CurrentHp;
    float CurrentST;
    float y = 0; //피채워줌 별거없음

    float JumpPower = 10; // 점프 파워
    public float AttackDamage = 0;

    bool JumpCheck; // 이중점프 체크
    bool JumpCharsh; // 점프중 부딫힘
    bool AttackCheck; //공격중 체크
    bool StrongCheck; //강한 공격 체크
    bool NextAttackCheck; // 다음 공격 켜져있나
    bool GuardCheck; // 땅이 닿았는지 체크
    bool HitCheck; // 맞았는지 체크
    bool UnHitCheck; // 안맞는 시간
    bool RushHit; // 상대 돌진중에 맞는거

    public bool CheckPoint;


    public float Stamina_Time = 0f; //스태미나 차는 시간 체크
    public bool RollingCheck; // 구르기 체크

    int LR_Check = 1; // 좌우 체크
    int HitRandNum; // 

    // Use this for initialization
    void Awake () {

        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        StartCoroutine(Recovery_ST());
        boss = GameObject.Find("Boss").GetComponent<Boss>();
        skeleton = GetComponent<SkeletonAnimator>();
        PlayerSource = GetComponent<AudioSource>();
        InvokeRepeating("WalkSound", 0.0f, 0.5f);
    }

    void WalkSound()
    {
        if ((Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)) && !JumpCheck)
        {
            PlayerSource.PlayOneShot(Move_S);
            Dust.enabled = true;
        }

        else
        {
            Dust.enabled = false;
        }
    }

    void FixedUpdate()
    {

        if (!JumpCheck && !AttackCheck && !RollingCheck)
        {
            Stamina_Time += Time.deltaTime;
        }

        if (PlayerST <= 0)
        {
            PlayerST = 0;
        }
    }

    // Update is called once per frame
    void Update () {

        if(!RollingCheck && !HitCheck) //구르는 중이 아니면 움직임 
        {
            if (!AttackCheck && !HitCheck && !UnHitCheck)
            {
                Moving();
            }
            StrongAttack();
        }

        if (!JumpCheck && !AttackCheck && !HitCheck && !UnHitCheck)
        {
            Rolling(); // 구르기
        }

        if (Input.GetKeyDown(KeyCode.S) && PlayerST >= 10 && !HitCheck && !UnHitCheck && !StrongCheck)
        {
            if (!NextAttackCheck) // 만약 어택 다음 공격  체크가 안되있으면
            {
                AttackDamage = 20; //대미지 20
                NextAttackCheck = true; //다음 어택 체크를 트루로
                coll.enabled = true; //출동 박스를 켜줌
                StartCoroutine(Co_WAttack()); // 공격 코루틴 실행
                CurrentST = 10; //스테미너
                StartCoroutine(Slow_ST()); // 스테미너 다는거
                Stamina_Time = 0;
                PlayerSource.clip = WAttack_S01;
                PlayerSource.Play();
            }

            else if(NextAttackCheck && PlayerST >= 15)
            {
                AttackDamage = 22;
                StartCoroutine(Co_WAttack02());
                CurrentST = 15;
                StartCoroutine(Slow_ST());
                Stamina_Time = 0;
                PlayerSource.clip = WAttack_S02;
                PlayerSource.Play();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && !JumpCheck && PlayerST >= 5 && !HitCheck && !UnHitCheck) //점프 버튼 
        {
            PlayerSource.clip = Jump_S;
            PlayerSource.Play();
            animator.SetBool("Jumping", true);
            JumpCheck = true;
            Jumping();
        }

        if(Input.GetKeyDown(KeyCode.A) && !HitCheck && !UnHitCheck) // 가드 하기
        {
            StartCoroutine(Guard());
            PlayerSource.clip = Defence_S;
            PlayerSource.Play();
        }

        else if(Input.GetKeyUp(KeyCode.A)) // 가드 풀기
        {
            StartCoroutine(UnGuard());
        }
    }

    IEnumerator Guard() //가드 하기
    {
        animator.SetBool("Guard", true);
        GuardCheck = true;
        yield return null;
    }

    IEnumerator UnGuard() // 가드 풀기
    {
        animator.SetBool("Guard", false);
        GuardCheck = false;
        yield return null;
    }

    IEnumerator Recovery_ST() //스태미너 회복 
    {
        do
        {
            if (Stamina_Time > 0.3f && PlayerST < 100)
            {
                PlayerST += 1.5f;
            }
            yield return new WaitForSeconds(0.001f);
        } while (true);
    }

    IEnumerator Slow_ST() //천천히 다는 스태미너
    {
        yield return new WaitForSeconds(0.01f);
        if (y < CurrentST)
        {
            PlayerST -= 1;
            y += 1;
            StartCoroutine(Slow_ST());
        }

        else if (y == CurrentST)
            y = 0;
    }

    IEnumerator Slow_HP()
    {
        yield return new WaitForSeconds(0.01f);
        if (y < CurrentHp)
        {
            PlayerHP -= 1;
            y += 1;
            StartCoroutine(Slow_HP());
        }

        else if (y == CurrentHp)
            y = 0;
    }

    void Moving() //캐릭터 좌우 이동
    {
        float movepowers = 0;
       // float Cast = Mathf.Abs(transform.position.x - Boss2.transform.position.x);

        if (Input.GetKey(KeyCode.LeftArrow) && !AttackCheck)
        {
            animator.SetBool("Running", true);
            transform.localScale = new Vector3(-0.8f, 0.8f, 1);
            LR_Check = 0;
            movepowers = -7;
        }

        else if (Input.GetKey(KeyCode.RightArrow) && !AttackCheck)
        {
            animator.SetBool("Running", true);
            movepowers = 7;
            transform.localScale = new Vector3(0.8f, 0.8f, 1);
            LR_Check = 1;
        }
        else
        {
            animator.SetBool("Running", false);
        }

        if (!JumpCharsh && !AttackCheck)
            rigid.velocity = new Vector2(movepowers, rigid.velocity.y);
    }

    void Jumping() //점프
    {
        rigid.velocity = Vector2.zero;
        rigid.AddForce(new Vector2(0, JumpPower), ForceMode2D.Impulse);
        CurrentST = 5;
        StartCoroutine(Slow_ST());
        Stamina_Time = 0;
        Shadow.enabled = false;
    }


    void Rolling() //구르기 
    {
        var Start = transform.position;
        if (Input.GetKeyDown(KeyCode.LeftShift) && !RollingCheck && PlayerST >= 25 && !UnHitCheck && !AttackCheck) // 구르기 체크
        {
            animator.SetBool("Rolling", true);
            StartCoroutine(RollingState());
            RollingCheck = true;
            CurrentST = 25;
            StartCoroutine(Slow_ST());
            Stamina_Time = 0;
            PlayerSource.clip = Rolling_S;
            PlayerSource.Play();
        }
    }

    IEnumerator RollingState()
    {
        //Vector3 RollVec = new Vector3(transform.position.x + 3f, transform.position.y, transform.position.z);
        rigid.velocity = Vector2.zero;
        float Rollinging = 0f;

        if (LR_Check == 0)
        {
            transform.localScale = new Vector3(-0.8f, 0.8f, 1);
            VectorAdd(new Vector2(-25f, 0f));
        }

        else if (LR_Check == 1)
        {
            transform.localScale = new Vector3(0.8f, 0.8f, 1);
            VectorAdd(new Vector2(25f, 0f));
        }
        while (Rollinging < 1f)
        {
            Rollinging += Time.deltaTime / 0.5f;
            //transform.position += RollVec * 10f * Time.deltaTime;
            yield return null;
        }

        if(Rollinging >= 1)
        {
            animator.SetBool("Rolling", false);
            RollingCheck = false;
        }

    }

    public void WAttack_StartColl() // 두번째 공격 콜라이더 다시 켜주기
    {
        coll.enabled = true;
    }

    IEnumerator Co_WAttack() // 첫번째 공격
    {
        rigid.velocity = Vector2.zero;
        AttackCheck = true;
        animator.SetBool("WAttack01", true);
        yield return new WaitForSeconds(0.3f);
        NextAttackCheck = false;
        animator.SetBool("WAttack01", false);
        AttackCheck = false;
        coll.enabled = false;
    }

    IEnumerator Co_WAttack02() // 두번째 공격
    {
        rigid.velocity = Vector2.zero;
        AttackCheck = true;
        animator.SetBool("WAttack02", true);
        yield return new WaitForSeconds(0.3f);
        animator.SetBool("WAttack02", false);
        AttackCheck = false;
        NextAttackCheck = false;
        coll.enabled = false;
    }

    public void SAttackCheck() //강공격 끝날때
    {
        animator.SetBool("SAttacking", false);
        coll.enabled = false;
        AttackCheck = false;
        StrongCheck = false;
    }

    public void StrongAttack() // 강공격
    {
        if(Input.GetKeyDown(KeyCode.D) && PlayerST >= 5 && !AttackCheck && !StrongCheck)
        {
            AttackDamage = 36;
            CurrentST = 20;
            StartCoroutine(Slow_ST());
            Stamina_Time = 0;
            AttackCheck = true;
            StrongCheck = true;
            rigid.velocity = Vector2.zero;
            animator.SetBool("SAttacking", true);
            coll.enabled = true;
            PlayerSource.clip = SAttack_S;
            PlayerSource.Play();
        }
    }

    public void RushCheck()
    {
        if(RushHit)
        {
            animator.SetBool("RushHit", true);
        }
    }

    public void DeathCheck()
    {
        if(PlayerHP >0)
        {
            animator.SetBool("RushHit", false);
            RushHit = false;
        }
    }

    public void StandPlayer()
    {
        UnHitCheck = false;
        animator.SetBool("HitCheck", false);
    }

    void Angel_move(Vector3 velocity) //밀어주기
    {
        GetComponent<Rigidbody2D>().velocity = velocity;
    }

    void VectorAdd(Vector2 Addforce)
    {
        rigid.AddForce(Addforce, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Check")
        {
            CheckPoint = true;
        }

        if (other.gameObject.tag == "Stone" && !UnHitCheck && !RollingCheck && !GuardCheck)
        {
            Vector2 hitvec = Vector2.zero;
            if (!GuardCheck)
            {
                animator.SetBool("HitCheck", true);
                
                CurrentHp = 10;
                StartCoroutine(Slow_HP());
                HitCheck = true;
                Invoke("ReHit", 0.5f);
                StartCoroutine(NoHitTime());
                if(LR_Check == 0) // 왼쪽이면
                {
                    hitvec = new Vector2(4f, 7f);
                }

                else if (LR_Check == 1) // 왼쪽이면
                {
                    hitvec = new Vector2(-4f, 7f);
                }
            }

            else
            {
                CurrentST = 10;
                StartCoroutine(Slow_ST());
                Stamina_Time = 0;
                if (LR_Check == 0) // 왼쪽이면
                {
                    hitvec = new Vector2(2f, 5f);
                }

                else if (LR_Check == 1) // 왼쪽이면
                {
                    hitvec = new Vector2(-2f, 5f);
                }
            }
        }

        if(other.gameObject.tag=="Boss" && boss.RushCheck && !RollingCheck && !UnHitCheck && !GuardCheck)
        {
            rigid.velocity = Vector2.zero;
            SAttackCheck();
            if (transform.position.x > BossPos.transform.position.x)
            {
                transform.localScale = new Vector3(-0.8f, 0.8f, 1);
                animator.SetBool("HitCheck", true);
                RushHit = true;
                VectorAdd(new Vector2(15f,10f));
                HitCheck = true;
                Invoke("ReHit", 0.5f);
                UnHitCheck = true;
                Shadow.enabled = false;
            }

            else
            {
                transform.localScale = new Vector3(0.8f, 0.8f, 1);
                animator.SetBool("HitCheck", true);
                RushHit = true;
                VectorAdd(new Vector2(-15f, 10f));
                HitCheck = true;
                Invoke("ReHit", 0.5f);
                UnHitCheck = true;
                Shadow.enabled = false;
            }
        }

        if(other.gameObject.tag == "Ground") // 착지 체크
        {
            JumpCheck = false;
            JumpCharsh = false;
            animator.SetBool("Jumping", false);
            Shadow.enabled = true;

            if (RushHit)
            {
               Invoke("DeathCheck", 0.8f);
               Shadow.enabled = true;
            }
        }

        if (other.gameObject.tag == "Stinger" && !UnHitCheck && !RollingCheck)
        {
            Vector2 hitvec2 = Vector2.zero;
            if(!GuardCheck)
            {
                rigid.velocity = Vector2.zero;
                animator.SetBool("HitCheck", true);
                CurrentHp = 10;
                StartCoroutine(Slow_HP());
                HitCheck = true;
                Invoke("ReHit", 0.5f);
                StartCoroutine(NoHitTime());
                if (LR_Check == 0) // 왼쪽이면
                {
                    hitvec2 = new Vector2(5f, 5f);
                }

                else if (LR_Check == 1) // 왼쪽이면
                {
                    hitvec2 = new Vector2(-5f, 5f);
                }
            }

            else
            {
                rigid.velocity = Vector2.zero;
                CurrentST = 10;
                StartCoroutine(Slow_ST());
                Stamina_Time = 0;
                if (LR_Check == 0) // 왼쪽이면
                {
                    hitvec2 = new Vector2(3f, 3f);
                }

                else if (LR_Check == 1) // 왼쪽이면
                {
                    hitvec2 = new Vector2(-3f, 3f);
                }

            }
            rigid.AddForce(hitvec2, ForceMode2D.Impulse);
        }

        if(other.gameObject.tag == "MiniBoss" && !UnHitCheck && !RollingCheck) 
        {
            Vector2 hitvec3 = Vector2.zero;

            if (!GuardCheck)
            {
                rigid.velocity = Vector2.zero;
                animator.SetBool("HitCheck", true);
                CurrentHp = 20;
                StartCoroutine(Slow_HP());
                HitCheck = true;
                Invoke("ReHit", 0.5f);
                StartCoroutine(NoHitTime());
                if (LR_Check == 0) // 왼쪽이면
                {
                    hitvec3 = new Vector2(5f, 5f);
                }

                else if (LR_Check == 1) // 왼쪽이면
                {
                    hitvec3 = new Vector2(-5f, 5f);
                }
            }

            else
            {
                CurrentST = 10;
                StartCoroutine(Slow_ST());
                Stamina_Time = 0;
                if (LR_Check == 0) // 왼쪽이면
                {
                    hitvec3 = new Vector2(2f, 3f);
                }

                else if (LR_Check == 1) // 왼쪽이면
                {
                    hitvec3 = new Vector2(-2f, 3f);
                }
            }
            rigid.AddForce(hitvec3, ForceMode2D.Impulse);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Boss") 
        {
            JumpCharsh = true;
        }
    }

    void ReHit()
    {
        HitCheck = false;
    }

    IEnumerator NoHitTime()
    {
        UnHitCheck = true;
        int Hited = 0;
        while (Hited < 3)
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
        yield return null;
        UnHitCheck = false;
        animator.SetBool("HitCheck", false);
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.tag == "Boss") // 착지 체크
        {
            JumpCharsh = true;
            //rigid.velocity = Vector2.zero;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "Boss") // 착지 체크
        {
            JumpCharsh = false;
           // rigid.velocity = Vector2.zero;
        }
    }

}
