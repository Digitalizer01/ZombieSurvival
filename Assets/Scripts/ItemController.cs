using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    void OnColliderEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("AmmoBox"))
        {
            
        }
    }
}
