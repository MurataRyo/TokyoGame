using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldData : MonoBehaviour
{
    public World[] worlds;
    public const int MAX_WORLDS = 6;
    private void Awake()
    {
        worlds = new World[MAX_WORLDS];
        worlds[0] = new World(-5, 22.5f, -40, 17, 0, 0);
        worlds[1] = new World(71.3f, 22.5f, 14.75f, 17, -74, 0);
        worlds[2] = new World(51.5f, 22.5f, 107.2f, 17, -140.5f, 0);
        worlds[3] = new World(-56, 22.5f, 103, 17, 140, -1);
        worlds[4] = new World(-82, 22.5f, 14.3f, 17, 71.32f, 0);
        worlds[5] = new World(-16, 40, 20, 60, 25, -7);

        //選択できないステージを？？？にする
        for (int i = GameTask.choiceStage; i < worlds.Length; i++)
        {
            worlds[i].choiceFlag = false;
        }
    }
}

public class World
{
    public Vector3 cameraPos;
    public Vector3 cameraAngle;
    public bool choiceFlag;

    public World(Vector3 cameraPos, Vector3 cameraAngle)
    {
        choiceFlag = true;
        this.cameraPos = cameraPos;
        this.cameraAngle = cameraAngle;
    }

    public World(float cameraPosX, float cameraPosY, float cameraPosZ, float cameraAngleX, float cameraAngleY, float cameraAngleZ)
    {
        choiceFlag = true;
        cameraPos = new Vector3(cameraPosX, cameraPosY, cameraPosZ);
        cameraAngle = new Vector3(cameraAngleX, cameraAngleY, cameraAngleZ);
    }
}
