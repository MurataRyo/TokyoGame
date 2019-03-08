using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarTask : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == GetTag.Player)
        {
            Debug.Log("クリア");
        }
    }
}
