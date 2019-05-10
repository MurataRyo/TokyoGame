using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarTask : MonoBehaviour
{
    GameObject player;
    PlayerMove playerMove;
    public EdgeCollider2D thisStarCol;
    private ParticleSystem lightLine;
    GameTask gameTask;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerMove = player.GetComponent<PlayerMove>();
        GameObject prefab = Resources.Load<GameObject>(GetPath.Particle + "/GoalParticle");
        GameObject particleOb = Instantiate(prefab);
        particleOb.transform.parent = gameObject.transform;
        particleOb.transform.position = transform.position;
        lightLine = particleOb.GetComponent<ParticleSystem>();
        gameTask = Utility.GetTaskObject().GetComponent<GameTask>();
    }

    private void Update()
    {
        if (thisStarCol != null)
            ParticlAdd(thisStarCol.points, Vector3.zero);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == GetTag.Player)
        {
            if (playerMove.playerState == PlayerMove.PlayerState.Light && playerMove.changeCount == 0f)
            {
                playerMove.stopPlayer = true;
                gameTask.image.color = new Vector4(1f, 1f, 1f, gameTask.alpha);
                gameTask.alpha += Time.deltaTime / 2;
                //クリア判定
                if(gameTask.alpha >= 1f)
                    gameTask.mode = GameTask.Mode.gameClear;
            }
        }
    }

    void ParticlAdd(Vector2[] vector2s,Vector3 basePos)
    {
        for (int i = 0; i < vector2s.Length -1; i++)
        {
            float range = (vector2s[i] - vector2s[i + 1]).magnitude * 10;
            for (int j = 0; j < range; j++)
            {
                ParticleSystem.EmitParams emit = new ParticleSystem.EmitParams();
                emit.position = lightLine.transform.InverseTransformPoint(Vector2.Lerp(vector2s[i], vector2s[i + 1], j / range)) + basePos;
                lightLine.Emit(emit, 1);
            }
        }
    }
}
