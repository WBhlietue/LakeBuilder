using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveCaller : MonoBehaviour
{
    private void OnCollisionEnter(Collision other) {
        Debug.Log(other.gameObject.tag);
        if(other.gameObject.CompareTag("ActiveCaller")){
            GetComponentInParent<Tile>().Active();
        }else if(other.gameObject.CompareTag("InActiveCaller")){
            GetComponentInParent<Tile>().InActive();
        }
    }
}
