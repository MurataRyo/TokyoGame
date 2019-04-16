using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldData : MonoBehaviour
{
    public World[] worlds;
    private void Awake()
    {
        worlds = new World[6];
        worlds[0] = new World(-5,8,-40,0,0,0,"保管庫");
        worlds[1] = new World(70,8,11,0,-72,0,"コンピューター室");
        worlds[2] = new World(45,8,114,0,-145,0,"なにか");
        worlds[3] = new World(-50,8,108,0,-215,0,"寝室");
        worlds[4] = new World(-82,8,13,0,68,0,"ガラクタの山");
        worlds[5] = new World(0,60,40,90,31,0,"ぼす～");
    }
}

public class World
{
    public Vector3 cameraPos;
    public Vector3 cameraAngle;
    public string worldname;

    public World(Vector3 cameraPos, Vector3 cameraAngle)
    {
        this.cameraPos = cameraPos;
        this.cameraAngle = cameraAngle;
    }

    public World(float cameraPosX, float cameraPosY, float cameraPosZ, float cameraAngleX, float cameraAngleY, float cameraAngleZ,string worldname)
    {
        cameraPos = new Vector3(cameraPosX, cameraPosY, cameraPosZ);
        cameraAngle = new Vector3(cameraAngleX, cameraAngleY, cameraAngleZ);
        this.worldname = worldname;
    }
}
