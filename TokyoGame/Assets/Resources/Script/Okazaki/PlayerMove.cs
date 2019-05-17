using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    // 自機の状態
    public enum PlayerState
    {
        Default,
        Light,
        Start,
    }

    private List<GameObject> m_hitObjects = new List<GameObject>();
    [HideInInspector] public PlayerState playerState = PlayerState.Default;
    GameObject launchHit;
    LaunchHit launchHitSc;

    [SerializeField] GameObject Model;          // 自機のモデル
    [SerializeField] GameObject LightModel;     // 自機のモデル（光状態）
    [SerializeField] GameObject StartModel;     // 自機のモデル（開始時）
    [SerializeField] GameObject particle;
    [SerializeField] GameObject Point;          // 自機の現在位置（サブカメラ用レーダー）
    [SerializeField] AudioClip WalkSe;          // 効果音（通常移動）
    [SerializeField] AudioClip RunSe;           // 効果音（ダッシュ）
    [SerializeField] AudioClip JumpSe;          // 効果音（ジャンプ）
    [SerializeField] AudioClip LandingSe;       // 効果音（着地）
    [SerializeField] AudioClip LightInSe;       // 効果音（変身）
    [SerializeField] AudioClip LightMoveSe;     // 効果音（光状態時の移動）
    float speed;                                // 移動速度
    Vector2 power = Vector2.zero;               // 自機にかかっている力
    float moveSpeed;                            // 空中にいるときの移動速度
    Vector2 moveBlockVelocity = Vector2.zero;
    float hitRadius;
    float moveHigh;                             // 空中にいるときの速度の上限（右方向）
    float moveLow;                              // 空中にいるときの速度の上限（左方向）
    float jumpPower;                            // ジャンプ力
    float angle;                                // 向き
    float startCount;
    [HideInInspector]
    public float changeCount;
    [HideInInspector]
    public float airTime;
    private const float WALK_SPEED = 5f;        // 歩行速度
    private const float RUN_SPEED = 10f;        // 走行速度
    private const float LIGHT_SPEED = 25f;      // 光状態の時の速度
    private const float JUMP_HEIGHT = 4f;       // ジャンプの頂点
    private const float GRAVITY_SIZE = 2f;      // 重力の強さ
    [HideInInspector]
    public bool move = false;                   // 能動的に動いているかどうか
    [HideInInspector]
    public bool runFlag = false;                // 走るかどうか
    [HideInInspector]
    public bool isGround = false;               // 接地しているかどうか
    bool lightFlag = false;                     // 光と重なっているかどうか
    [HideInInspector]
    public bool jumpFlag = false;               // ジャンプするかどうか
    bool lightJump = false;                     // 光の中でジャンプしたかどうか
    bool landing = false;
    [HideInInspector]
    public bool lineMove = false;               // 直線光以外の光に当たっていないかどうか
    [HideInInspector]
    public bool launchControl = false;          // 光源を操作するかどうか
    [HideInInspector]
    public bool stopPlayer = false;
    [HideInInspector]
    public float deathHeight;                   // 落下死になる高さ

    [HideInInspector]
    public Vector3 startPosition = Vector2.zero;
    [HideInInspector]
    public Vector2 velocity = Vector2.zero;
    Vector3 position = Vector3.zero;

    new Rigidbody2D rigidbody;
    BoxCollider2D boxCollider2D;
    BoxCollider2D launchHitCollider;
    CircleCollider2D circleCollider2D;
    XBoxController controller;
    AudioSource aSource;
    GameTask gameTask;

    void Awake()
    {
        playerState = PlayerState.Start;
        startPosition = transform.position;
        transform.position = new Vector3(startPosition.x, startPosition.y + 10f, startPosition.z);
        StartModel.SetActive(true);
        Model.SetActive(false);
        LightModel.SetActive(false);
        Point.SetActive(false);
    }

    void Start()
    {
        aSource = GetComponent<AudioSource>();
        launchHit = GameObject.FindGameObjectWithTag("LaunchHit");
        launchHitSc = launchHit.GetComponent<LaunchHit>();
        rigidbody = GetComponent<Rigidbody2D>();
        jumpPower = Mathf.Pow(JUMP_HEIGHT * 2 * 9.81f * GRAVITY_SIZE, 0.5f); // ジャンプ力の計算
        boxCollider2D = gameObject.GetComponent<BoxCollider2D>();
        launchHitCollider = launchHit.GetComponent<BoxCollider2D>();
        circleCollider2D = gameObject.GetComponent<CircleCollider2D>();
        controller = Utility.GetTaskObject().GetComponent<XBoxController>();
        gameTask = Utility.GetTaskObject().GetComponent<GameTask>();
        deathHeight = -7f;
        launchHit.transform.position = new Vector3(transform.position.x + launchHitCollider.size.x / 2, transform.position.y, transform.position.z);
        angle = 90f;
    }

    void Update()
    {
        //Collider2D[] playerHit = Physics2D.OverlapBoxAll(transform.position, boxCollider2D.size, 0f);
        Collider2D[] playerHit = Physics2D.OverlapCircleAll
            (new Vector2(transform.position.x, transform.position.y + (circleCollider2D.offset.y * transform.localScale.y)), hitRadius, LayerMask.GetMask("Col")); // 自機の当たり判定の取得
        Collider2D[] lineHit = Physics2D.OverlapCircleAll
            (new Vector2(transform.position.x, transform.position.y + (circleCollider2D.offset.y * transform.localScale.y)), hitRadius, LayerMask.GetMask("Col"));
        Vector2 velocity = rigidbody.velocity;

        lightFlag = false;

        // 侵入判定
        for (int i = 0; i < playerHit.Length; i++)
        {
            if (playerHit[i].tag == GetTag.Col || playerHit[i].tag == GetTag.Col2)
            {
                lightFlag = true;
                lineMove = true;
            }
        }
        for (int i = 0; i < lineHit.Length; i++)
        {
            if (lineHit[i].tag == GetTag.Col)
                lineMove = false;
        }

        if ((velocity.x > 0f && controller.MoveButton() > 0f) || (velocity.x < 0f && controller.MoveButton() < 0f))
            move = true;

        else
            move = false;

        /*速度の変更------------------------------------------------------------------*/
        if (runFlag)
            speed = RUN_SPEED;

        else if(playerState == PlayerState.Light)
            speed = LIGHT_SPEED;

        else
            speed = WALK_SPEED;
        /*----------------------------------------------------------------------------*/

        /*走るかどうか-----------------------------------------------------------------*/
        if (controller.RunButton() && playerState == PlayerState.Default && isGround)
            runFlag = true;

        else
            runFlag = false;
        /*----------------------------------------------------------------------------*/

        if (lightJump && (!controller.ChangeButton() || !lightFlag || velocity.y < 0f))
            lightJump = false;

        if (!stopPlayer)
        {
            if (playerState == PlayerState.Light)
                power = controller.LightMoveButton();

            else
                power = new Vector2(controller.MoveButton(), 0f);
        }
        else
            power = Vector2.zero;

        // ジャンプする
        if (controller.JumpButton() && !stopPlayer && playerState != PlayerState.Start)
        {
            if (playerState == PlayerState.Light)
            {
                aSource.PlayOneShot(LightInSe);
                lightJump = true;
                Particle();
                playerState = PlayerState.Default;

                if (LightModel.activeSelf)
                    LightModel.SetActive(false);

                changeCount = 0f;
                changeCount += Time.fixedDeltaTime;
            }
            else
            {
                if (!isGround || launchControl)
                    return;

                aSource.PlayOneShot(JumpSe);
            }
            moveHigh = speed;
            jumpFlag = true;
        }
    }

    void FixedUpdate()
    {
        velocity = rigidbody.velocity;

        // 光の中を出入りする
        if(!launchControl)
        {
            if (lightFlag && controller.ChangeButton() && playerState == PlayerState.Default && !lightJump)
            {
                if (Model.activeSelf)
                    Model.SetActive(false);

                aSource.PlayOneShot(LightInSe);
                playerState = PlayerState.Light;
                velocity = new Vector2(0f, 0f);     // 速度をリセット
                Particle();
                changeCount = 0f;
                changeCount += Time.fixedDeltaTime;
            }
            else if ((!lightFlag || !controller.ChangeButton()) && playerState == PlayerState.Light && !stopPlayer)
            {
                if (LightModel.activeSelf)
                    LightModel.SetActive(false);

                aSource.PlayOneShot(LightInSe);
                Particle();
                moveHigh = speed;
                playerState = PlayerState.Default;
                changeCount = 0f;
                changeCount += Time.fixedDeltaTime;
            }
        }

        if(!isGround)
            airTime += Time.fixedDeltaTime;

        else
            airTime = 0f;

        // 落下死判定
        if (transform.position.y < deathHeight)
        {
            stopPlayer = true;
            gameTask.image.color = new Vector4(0f, 0f, 0f, gameTask.alpha);
            gameTask.alpha += Time.deltaTime / 2;

            if (gameTask.alpha >= 1f)
                Utility.GetTaskObject().GetComponent<GameTask>().mode = GameTask.Mode.gameOver;
        }

        if (changeCount > 0f)
            changeCount += Time.fixedDeltaTime;

        moveLow = -moveHigh;

        /*自機の移動----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

        // 自機の向きを変える
        if (isGround || playerState == PlayerState.Light)
        {
            if ((!launchControl && launchHit.transform.position != new Vector3(transform.position.x + launchHitCollider.size.x / 2, transform.position.y, transform.position.z) && controller.MoveButton() > 0f) ||
                (launchControl && transform.position.x < launchHitSc.target.transform.parent.transform.parent.transform.position.x))
            {
                launchHit.transform.position = new Vector3(transform.position.x + launchHitCollider.size.x / 2, transform.position.y, transform.position.z);
                angle = 90f;
            }
            if ((!launchControl && launchHit.transform.position != new Vector3(transform.position.x - launchHitCollider.size.x / 2, transform.position.y, transform.position.z) && controller.MoveButton() < 0f) ||
                (launchControl && transform.position.x > launchHitSc.target.transform.parent.transform.parent.transform.position.x))
            {
                launchHit.transform.position = new Vector3(transform.position.x - launchHitCollider.size.x / 2, transform.position.y, transform.position.z);
                angle = -90f;
            }
        }

        Model.transform.eulerAngles = new Vector3(0f, angle, 0f);

        if (playerState == PlayerState.Start)   // 開始時の演出
        {
            position = transform.position;

            if (transform.position.y != startPosition.y)
                transform.position = Vector3.Lerp(position, startPosition, 0.05f);

            if (transform.position.y - startPosition.y <= 0.01f)
                transform.position = startPosition;

            if (transform.position == startPosition && startCount == 0f)
            {
                Particle();
                StartModel.SetActive(false);
                Point.SetActive(true);
                startCount += Time.fixedDeltaTime;
                changeCount = Time.fixedDeltaTime;
            }

            if (startCount > 0f)
                startCount += Time.fixedDeltaTime;

            if (changeCount >= 0.2f)
            {
                Model.SetActive(true);
                changeCount = 0f;
            }

            if (startCount > 1f)
                playerState = PlayerState.Default;

            rigidbody.gravityScale = 0f;
            boxCollider2D.enabled = false;
        }
        else if (playerState == PlayerState.Light) // 光の中に入っている時の移動
        {
            Vector2 vect2 = power * speed;
            velocity = transform.right * vect2.x + transform.up * vect2.y;

            if (changeCount >= 0.2f)
            {
                if (!LightModel.activeSelf)
                    LightModel.SetActive(true);

                changeCount = 0f;
            }

            if (lineMove)
                hitRadius = circleCollider2D.radius;

            else
                hitRadius = 0f;

            rigidbody.velocity = velocity;
            rigidbody.gravityScale = 0f;            // 重力を消す
            boxCollider2D.enabled = false;
            circleCollider2D.isTrigger = false;
        }
        else // それ以外の時の移動
        {
            if (launchControl)  // 光源操作中は動かない
            {
                velocity.x = 0f;
            }
            else if (!isGround) // 空中にいるとき
            {
                /*速度上限の再設定--------------------------------------*/
                if (moveHigh > speed)
                {
                    if (moveSpeed > 0f && moveSpeed < moveHigh)
                        moveHigh = moveSpeed;

                    else if (moveSpeed < 0f && moveSpeed > moveLow)
                        moveHigh = -moveSpeed;
                }

                if (moveSpeed <= speed && moveSpeed >= -speed)
                    moveHigh = speed;
                /*-----------------------------------------------------*/

                if (moveSpeed > moveHigh)
                    moveSpeed = moveHigh;

                else if (moveSpeed < moveLow)
                    moveSpeed = moveLow;

                // 自機の制御
                if (moveSpeed <= moveHigh && moveSpeed >= moveLow)
                    moveSpeed += power.x / 3;

                velocity.x = moveSpeed;
            }
            else // 接地しているとき
                velocity.x = power.x * speed;

            /*重力関係---------------------------------------------------*/

            if (jumpFlag)
                velocity.y = jumpPower;

            else if (velocity.y < -15f)
                velocity.y = -15f;

            /*----------------------------------------------------------*/

            if (jumpFlag && velocity.y == jumpPower)
            {
                jumpFlag = false;
            }

            // 乗っている動く床のvelocityを取得
            if (m_hitObjects.Count > 0)
                moveBlockVelocity = m_hitObjects[0].gameObject.GetComponent<Rigidbody2D>().velocity;

            else
                moveBlockVelocity = Vector2.zero;

            if (changeCount >= 0.2f)
            {
                if (!Model.activeSelf)
                    Model.SetActive(true);

                changeCount = 0f;
            }

            rigidbody.velocity = new Vector2(velocity.x + moveBlockVelocity.x, velocity.y);
            rigidbody.gravityScale = GRAVITY_SIZE;  // 重力を適用
            boxCollider2D.enabled = true;
            circleCollider2D.isTrigger = true;
            hitRadius = 0f;
        }
        moveSpeed = velocity.x;
        /*-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/
        
        RayGround();
    }

    // 接地判定
    void RayGround()
    {
        isGround = false;
        Ray2D ray = new Ray2D(new Vector2(transform.position.x, transform.position.y + (boxCollider2D.offset.y * transform.localScale.y)), Vector2.down);

        RaycastHit2D[] hits = Physics2D.BoxCastAll(ray.origin, new Vector2(boxCollider2D.size.x * transform.localScale.x, boxCollider2D.size.y * transform.localScale.y), 0f, ray.direction, 0.1f);

        foreach (RaycastHit2D hit in hits)
        {
            if(hit.normal.y > 0.5f && (!hit.collider.isTrigger))
                isGround = true;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        for (int i = 0; i < collision.contacts.Length; i++)
        {
            if (collision.contacts[i].normal.y > 0.5f && playerState == PlayerState.Default && (collision.collider.tag == "Block" || collision.collider.tag == "Glass"))
                aSource.PlayOneShot(LandingSe);
        }

        if (collision.collider.tag != "MoveBlock")
            return;

        m_hitObjects.Add(collision.gameObject);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        for (int i = 0; i < collision.contacts.Length; i++)
        {
            if ((collision.collider.tag == "Block" || collision.collider.tag == "Glass") &&
                ((collision.contacts[i].normal.x < -0.5f && moveSpeed > 0f) || (collision.contacts[i].normal.x > 0.5f && moveSpeed < 0f)))
                moveSpeed = 0f;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag != "MoveBlock")
            return;

        m_hitObjects.Remove(collision.gameObject);
    }

    // 変身時のエフェクト
    void Particle()
    {
        GameObject go = Instantiate(particle);
        go.transform.parent = transform;
        go.transform.position = transform.position + new Vector3(0f,1f,0f);
        Destroy(go, 1f);
    }
}
