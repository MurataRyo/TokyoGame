using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchHit : MonoBehaviour
{
    private List<GameObject> m_hitObjects = new List<GameObject>();
    new BoxCollider2D collider2D;
    int select = 0;

    void Start()
    {
        collider2D = gameObject.GetComponent<BoxCollider2D>();
    }
    
    void Update()
    {
        Debug.Log(m_hitObjects.Count);
        if (Input.GetKeyDown(KeyCode.G))
        {
            select++;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            select--;
        }

        if (select > m_hitObjects.Count - 1)
        {
            select = 0;
        }
        if (select < 0)
        {
            select = m_hitObjects.Count - 1;
        }

        //Debug.Log(select);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag != "Launch")
        {
            return;
        }
        m_hitObjects.Add(gameObject);

        for (int i = 0; i < m_hitObjects.Count; i++)
        {
            LaunchControl[] launchControl = collision.gameObject.GetComponents<LaunchControl>();
            launchControl[select].selectFlag = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Launch")
        {
            return;
        }
        m_hitObjects.Remove(gameObject);
    }
}
