using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
{
    Camera cam;
    public LayerMask detectLayer;
    Tile detectedTile = null;
    string detectedName = "";
    private void Start() {
        cam = Camera.main;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
        Ray ray = cam.ScreenPointToRay(eventData.position);
        if (Physics.Raycast(ray, out RaycastHit hit, detectLayer, 100))
        {
            if(detectedTile!= null){
                if(detectedTile.gameObject.name.Split("-")[0] == "Scene"){
                    TileManager.instance.CallInActive();
                    string n = detectedTile.gameObject.name.Split("-")[1];
                    TileManager.instance.Delay(() => {
                        UnityEngine.SceneManagement.SceneManager.LoadScene(n);
                    }, 2.5f);
                }
            }
            detectedTile?.Mine();
            // Debug.Log("Pointer down hit: " + hit.collider.gameObject.name);
        }
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        Ray ray = cam.ScreenPointToRay(eventData.position);
        if (Physics.Raycast(ray, out RaycastHit hit, detectLayer, 100))
        {
            GameObject tar = hit.collider.transform.parent.gameObject;
            if(detectedName != tar.name){
                detectedName = tar.name;
                detectedTile?.DeSelect();
                detectedTile = tar.GetComponent<Tile>();
                detectedTile.Select();
            }
            // Debug.Log("Pointer down hit: " + hit.collider.gameObject.name);
        }else{
            if(detectedName != ""){
                detectedName = "";
                detectedTile?.DeSelect();
                detectedTile = null;
            }
        }
    }

}
