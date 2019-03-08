using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldChoiceTask : MonoBehaviour
{
    private World[] worlds;                 //ワールドデータ
    private ChoiceClass choiceClass;        //選択用class
    private Camera mCamera;                 //カメラ
    private const float CAMERA_TIME = 0.40f;   //カメラの移動時間
    private IEnumerator moveCamera;
    // Start is called before the first frame update
    void Start()
    {
        worlds = GetComponent<WorldData>().worlds;
        choiceClass = new ChoiceClass(worlds.Length);
        mCamera = Camera.main;
        mCamera.transform.position = worlds[choiceClass.nowChoice].cameraPos;
        mCamera.transform.eulerAngles = worlds[choiceClass.nowChoice].cameraAngle;
    }

    // Update is called once per frame
    void Update()
    {
        CameraChoice();
    }

    private void CameraChoice()
    {
        if (moveCamera != null)
            return;

        if (Input.GetKeyDown(KeyCode.D))
        {
            choiceClass.ChoiceChange(true);
            moveCamera = MoveCamera(worlds[choiceClass.nowChoice].cameraPos, worlds[choiceClass.nowChoice].cameraAngle);
            StartCoroutine(moveCamera);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            choiceClass.ChoiceChange(false);
            moveCamera = MoveCamera(worlds[choiceClass.nowChoice].cameraPos, worlds[choiceClass.nowChoice].cameraAngle);
            StartCoroutine(moveCamera);
        }
    }

    private IEnumerator MoveCamera(Vector3 cameraPos, Vector3 cameraAngle)
    {
        float moveRange = (cameraPos - mCamera.transform.position).magnitude / CAMERA_TIME;
        float timer = 0;
        Vector3 pos = mCamera.transform.position;
        Vector3 angle = mCamera.transform.eulerAngles;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > CAMERA_TIME)
            {
                mCamera.transform.position = cameraPos;
                mCamera.transform.eulerAngles = cameraAngle;
                moveCamera = null;
                yield break;
            }
            float t = timer / CAMERA_TIME;
            mCamera.transform.position = Vector3.Lerp(pos, cameraPos,t);
            mCamera.transform.eulerAngles = MoveRangeAngle(angle, cameraAngle, t);
            yield return null;
        }
    }

    private Vector3 MoveRangeAngle(Vector3 now, Vector3 next, float t)
    {
       return  new Vector3(Mathf.LerpAngle(now.x, next.x, t), Mathf.LerpAngle(now.y, next.y, t), Mathf.LerpAngle(now.z, next.z, t));
    }
}

