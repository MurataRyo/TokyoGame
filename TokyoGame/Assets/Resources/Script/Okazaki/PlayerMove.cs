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
    }

    private List<GameObject> m_hitObjects = new List<GameObject>();
    [HideInInspector] public PlayerState playerState = PlayerState.Default;
    GameObject launchHit;

    [SerializeField] GameObject Model;          // 自機のモデル
    [SerializeField] GameObject LightModel;     // 自機のモデル（光状態）
    [SerializeField] GameObject particle;
    float speed;                                // 移動速度
    Vector2 power = Vector2.zero;
    float moveSpeed;
    Vector2 moveBlockVelocity = Vector2.zero;
    float hitRadius;
    float moveHigh;
    float moveLow;
    float jumpPower;                            // ジャンプ力
    float angle;                                // 向き
    float changeCount;
    [HideInInspector]
    public float airTime;
    private const float WALK_SPEED = 5f;        // 歩行速度
    private const float RUN_SPEED = 10f;        // 走行速度
    private const float LIGHT_SPEED = 15f;      // 光状態の時の速度
    private const float LINE_SPEED = 25f;       // 光状態の時の速度（直線）
    private const float JUMP_HEIGHT = 4f;       // ジャンプの頂点
    private const float GRAVITY_SIZE = 20f;     // 重力の強さ
    [HideInInspector]
    public bool move = false;                   // 能動的に動いているかどうか
    [HideInInspector]
    public bool runFlag = false;                // 走るかどうか
    [HideInInspector]
    public bool isGround = false;               // 接地しているかどうか
    bool lightFlag = false;                     // 光と重なっているかどうか
    [HideInInspector]
    public bool jumpFlag = false;               // ジャンプするかどうか
    bool lightJump = false;
    bool search = false;
    [HideInInspector]
    public bool lineMove = false;
    [HideInInspector]
    public bool launchControl = false;          // 光源を操作するかどうか
    [HideInInspector]
    public bool stopPlayer = false;
    [HideInInspector]
    public float deathHeight;                   // 落下死になる高さ

    Vector2 position = Vector2.zero;
    [HideInInspector]
    public Vector2 velocity = Vector2.zero;

    new Rigidbody2D rigidbody;
    BoxCollider2D boxCollider2D;
    BoxCollider2D launchHitCollider;
    CircleCollider2D circleCollider2D;
    XBoxController controller;

    void Start()
    {
        launchHit = GameObject.FindGameObjectWithTag("LaunchHit");
        jumpPower = Mathf.Pow(JUMP_HEIGHT * 2 * GRAVITY_SIZE, 0.5f); // ジャンプ力の計算
        rigidbody = GetComponent<Rigidbody2D>();
        boxCollider2D = gameObject.GetComponent<BoxCollider2D>();
        launchHitCollider = launchHit.GetComponent<BoxCollider2D>();
        circleCollider2D = gameObject.GetComponent<CircleCollider2D>();
        controller = Utility.GetTaskObject().GetComponent<XBoxController>();
        deathHeight = -7f;
        launchHit.transform.position = new Vector3(transform.position.x + launchHitCollider.size.x / 2, transform.position.y, transform.position.z);
        angle = 90f;
    }

    void Update()
    {
        /*Collider2D[] playerHit = Physics2D.OverlapBoxAll(transform.position, boxCollider2D.size, 0f);*/
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
        {
            if (lineMove)
                speed = LINE_SPEED;

            else
                speed = LIGHT_SPEED;
        }
        else
            speed = WALK_SPEED;
        /*----------------------------------------------------------------------------*/

        /*走るかどうか-----------------------------------------------------------------*/
        if (controller.RunButton() && playerState == PlayerState.Default && isGround)
            runFlag = true;

        else
            runFlag = false;
        /*----------------------------------------------------------------------------*/

        if (!controller.ChangeButton() && lightJump)
            lightJump = false;

        if (playerState == PlayerState.Light)
            power = controller.LightMoveButton();

        else
            power = new Vector2(controller.MoveButton(), 0f);

        // ジャンプする
        if (controller.JumpButton())
        {
            if (playerState == PlayerState.Light)
            {
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
            }
            moveHigh = speed;
            jumpFlag = true;
        }
    }

    void FixedUpdate()
    {
        velocity = rigidbody.velocity;
        //Vector2 position = transform.position;
        //float x = circleCollider2D.radius + speed * Time.fixedDeltaTime;
        //Vector2 y = new Vector2(velocity.x, velocity.y).normalized;
        //Vector2 z = x * y + position;
        //Collider2D[] lightSearch = Physics2D.OverlapCircleAll(z, 0f, LayerMask.GetMask("Col"));

        // 光の中を出入りする
        if (lightFlag && controller.ChangeButton() && playerState != PlayerState.Light && !lightJump)
        {
            if(Model.activeSelf)
                Model.SetActive(false);

            playerState = PlayerState.Light;
            velocity = new Vector2(0f, 0f);     // 速度をリセット
            Particle();
            changeCount = 0f;
            changeCount += Time.fixedDeltaTime;
        }
        else if ((!lightFlag || !controller.ChangeButton()) && playerState != PlayerState.Default)
        {
            if(LightModel.activeSelf)
                LightModel.SetActive(false);

            Particle();
            moveHigh = speed;
            playerState = PlayerState.Default;
            changeCount = 0f;
            changeCount += Time.fixedDeltaTime;
        }

        if(!isGround)
            airTime += Time.fixedDeltaTime;

        else
            airTime = 0f;

        // 落下死判定
        if (transform.position.y < deathHeight)
            Utility.GetTaskObject().GetComponent<GameTask>().mode = GameTask.Mode.gameOver;

        if (changeCount > 0f)
            changeCount += Time.fixedDeltaTime;

        moveLow = -moveHigh;

        // 自機の移動
        if (!stopPlayer)
        {
            // 自機の向きを変える
            if((isGround && !launchControl) || playerState == PlayerState.Light)
            {
                if (launchHit.transform.position != new Vector3(transform.position.x + launchHitCollider.size.x / 2, transform.position.y, transform.position.z) && controller.MoveButton() > 0f)
                {
                    launchHit.transform.position = new Vector3(transform.position.x + launchHitCollider.size.x / 2, transform.position.y, transform.position.z);
                    angle = 90f;
                }
                if (launchHit.transform.position != new Vector3(transform.position.x - launchHitCollider.size.x / 2, transform.position.y, transform.position.z) && controller.MoveButton() < 0f)
                {
                    launchHit.transform.position = new Vector3(transform.position.x - launchHitCollider.size.x / 2, transform.position.y, transform.position.z);
                    angle = -90f;
                }
            }

            Model.transform.eulerAngles = new Vector3(0f, angle, 0f);

            if (playerState == PlayerState.Light) // 光の中に入っている時の移動
            {
                Vector2 vect2 = power * speed;
                velocity = transform.right * vect2.x + transform.up * vect2.y;

                if(changeCount >= 0.2f)
                {
                    if (!LightModel.activeSelf)
                        LightModel.SetActive(true);

                    changeCount = 0f;
                }

                rigidbody.velocity = new Vector2(velocity.x, velocity.y);
                boxCollider2D.enabled = false;
                circleCollider2D.isTrigger = false;
                hitRadius = circleCollider2D.radius / 4;
                search = true;
            }
            else // それ以外の時の移動
            {
                if (launchControl)
                {
                    velocity.x = 0f;
                }
                else if (!isGround)
                {
                    if(moveHigh > speed)
                    {
                        if (moveSpeed > 0f && moveSpeed < moveHigh)
                            moveHigh = moveSpeed;

                        else if (moveSpeed < 0f && moveSpeed > moveLow)
                            moveHigh = -moveSpeed;
                    }

                    if (moveSpeed <= speed && moveSpeed >= -speed)
                        moveHigh = speed;

                    if (moveSpeed > moveHigh)
                        moveSpeed = moveHigh;

                    else if (moveSpeed < moveLow)
                        moveSpeed = moveLow;

                    if (moveSpeed <= moveHigh && moveSpeed >= moveLow)
                        moveSpeed += power.x / 3;

                    velocity.x = moveSpeed;
                }
                else
                    velocity.x = power.x * speed;

                /*重力関係---------------------------------------------------*/
                if (((isGround && !jumpFlag) || playerState == PlayerState.Light) && velocity.y <= 0f)
                    velocity.y = 0f;

                else if (jumpFlag)
                    velocity.y = jumpPower;

                else if(velocity.y < -15f)
                    velocity.y = -15f;

                else
                    velocity.y -= GRAVITY_SIZE * Time.fixedDeltaTime;
                /*----------------------------------------------------------*/

                if (jumpFlag && velocity.y == jumpPower)
                    jumpFlag = false;

                if (m_hitObjects.Count > 0)
                    moveBlockVelocity = m_hitObjects[0].gameObject.GetComponent<Rigidbody2D>().velocity;

                else
                    moveBlockVelocity = Vector2.zero;

                if (changeCount >= 0.2f)
                {
                    if(!Model.activeSelf)
                        Model.SetActive(true);

                    changeCount = 0f;
                }

                rigidbody.velocity = velocity + moveBlockVelocity;
                boxCollider2D.enabled = true;
                circleCollider2D.isTrigger = true;
                hitRadius = 0f;
                search = false;
            }
            //for (int i = 0; i < lightSearch.Length; i++)
            //{
            //    if (lightSearch[i].tag == "Col")
            //    {
            //        search = false;
            //    }
            //}
            moveSpeed = velocity.x;
        }
        else
            rigidbody.velocity = new Vector2(0f, 0f);

        RayGround();
        Debug.Log(moveHigh);
    }

    void LightSearch()
    {
        Vector2 position = transform.position;
        velocity = rigidbody.velocity;
        float x = circleCollider2D.radius + LIGHT_SPEED * Time.fixedDeltaTime;
        Vector2 y = new Vector2(velocity.x, velocity.y).normalized;
        Vector2 z = x * y + position;
        Vector2 direction = position - z;

        RaycastHit2D hit = Physics2D.Raycast(z, direction, x, LayerMask.GetMask("Col"));

        if(Physics2D.Raycast(z, direction, x, LayerMask.GetMask("Col")))
        {
            Debug.Log(hit.point);
            Debug.Log(Quaternion.FromToRotation(direction, hit.normal).eulerAngles);
            transform.position = ClosestPoint(hit.collider, z) - (ClosestPoint(hit.collider, z) - position);
        }
        Debug.DrawRay(z, direction, Color.red);
        Debug.Log(ClosestPoint(hit.collider, z) - position);
    }

    Vector2 ClosestPoint(Collider2D collider, Vector2 point)
    {
        GameObject go = new GameObject();
        go.transform.position = point;
        CircleCollider2D circle = go.AddComponent<CircleCollider2D>();
        circle.radius = 0.01f;
        ColliderDistance2D dis = collider.Distance(circle);
        Destroy(go);
        return dis.pointA;
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
            if (collision.contacts[i].normal.x < -0.5f || collision.contacts[i].normal.x > 0.5f)
            {
                if(moveSpeed != 0f)
                    moveSpeed = 0f;
            }
        }
        if (collision.collider.tag == "MoveBlock" && playerState == PlayerState.Default)
        {
            m_hitObjects.Add(collision.transform.root.gameObject);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag != "MoveBlock")
            return;

        m_hitObjects.Remove(collision.transform.root.gameObject);
    }

    // 変身時のエフェクト
    void Particle()
    {
        GameObject go = Instantiate(particle);
        go.transform.parent = transform;
        go.transform.position = transform.position + new Vector3(0f,1f,0f);
        Destroy(go, 0.6f);
    }
}
