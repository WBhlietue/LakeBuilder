using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum TileType{
    Ground, House, River, Mountain, Forest
}

public class Tile : MonoBehaviour
{
    public TileType type;
    public string tileCode;
    public bool isOrigin;
    string originCode;
    List<string> strTypes = new List<string>{"q", "a", "z", "e", "d", "c","qa", "qz", "qe", "qd", "qc", "az", "ae", "ad", "ac", "ze", "zd", "zc", "ed", "ec", "dc", 
                                            "qaz", "qae", "qad", "qac", "qze", "qzd", "qzc", "qed", "qec", "qdc", "aze", "azd", "azc", "aed", "aec", "adc", "zed", "zec", "zdc", "edc", 
                                            "qaze", "qazd", "qazc", "qaed", "qaec", "qadc", "qzed", "qzec", "qzdc", "qedc", "azed", "azec", "azdc","aedc", "zedc",
                                            "qazed", "qazec", "qazdc", "qaedc", "qzedc", "azedc", 
                                            "qazedc"};
    public float selectUp;
    public float selectDuration;
    public Ease selectEase;
    public Ease deSelectEase;
    public Transform tiles;
    public GameObject ground;
    public GameObject[] houses;
    public GameObject[] rivers;
    public GameObject mountains;
    public GameObject forests;
    public Tile[] between = new Tile[6];
    public LayerMask targetLayer;
    GameObject currentTile;
    public bool isWater = false;
    List<string> dirs =new List<string> {"q", "a", "z", "e", "d", "c"};

    public float floatValue;
    public float floatDur;
    public Ease downEase;
    public Ease upEase;
    public float activeForce;
    bool isActive = false;
    public bool useInMenu = false;
    public bool haveWater = false;
    public Animator anim;
    void Start()
    {
        if(isOrigin){
            originCode = tileCode;
        }
        RaycastHit hit;
        if(Physics.Raycast(transform.position, new Vector3(0, 0, 1), out  hit, 1f,targetLayer )){
            between[3] = hit.collider.transform.parent.GetComponent<Tile>();
        }
        if(Physics.Raycast(transform.position, new Vector3(-1, 0, 1), out  hit, 1f,targetLayer )){
            between[0] = hit.collider.transform.parent.GetComponent<Tile>();
        }
        if(Physics.Raycast(transform.position, new Vector3(-1, 0, -1), out  hit, 1f,targetLayer )){
            between[1] = hit.collider.transform.parent.GetComponent<Tile>();
        }
        if(Physics.Raycast(transform.position, new Vector3(1, 0, 1), out  hit, 1f,targetLayer )){
            between[4] = hit.collider.transform.parent.GetComponent<Tile>();
        }
        if(Physics.Raycast(transform.position, new Vector3(1, 0, -1), out  hit, 1f,targetLayer )){
            between[5] = hit.collider.transform.parent.GetComponent<Tile>();
        }
        if(Physics.Raycast(transform.position, new Vector3(0, 0, -1), out  hit, 1f,targetLayer )){
            between[2] = hit.collider.transform.parent.GetComponent<Tile>();
        }
        
        ground.SetActive(false);
        Debug.Log(strTypes.Count);
        ChangeTile();
        tiles.transform.localPosition = new Vector3(0, -10, 0);
        if(type == TileType.House){
            TileManager.instance.houseTile.Add(this);
            for(int i = 0; i < 6; i++){
                if(between[i] != null){
                    if(between[i].isOrigin){
                        haveWater = true;
        anim.SetBool("active", true);
                    }
                }
            }
            
        }
    }
    public void Active(){
        if(isActive){
            return;
        }
        isActive = true;
        tiles.DOLocalMoveY(0, 1f).SetEase(Ease.OutBack, activeForce);
    }
    public void InActive(){
        if(!isActive){
            return;
        }
        isActive = false;
        tiles.DOLocalMoveY(-10, 1f).SetEase(Ease.InBack);
    }
    public void Shake(){
            transform.DOLocalMoveY(-selectUp, floatDur/2).OnComplete(() => {
            transform.DOLocalMoveY(0, floatDur/2);
        });
    }
    public void Pull(){
        transform.DOLocalMoveY(floatValue, floatDur).SetEase(downEase).OnComplete(() => {
            SetWater();
            transform.DOLocalMoveY(0, floatDur*1.5f).SetEase(upEase);
        });
    }
    public void Mine(){
        if(type != TileType.Ground){
            Shake();
            return;
        }
        for(int i = 0; i < between.Length; i++){
            if(between[i] != null && between[i].isWater){
                if(TileManager.instance.Mine()){
                Pull();
                }else{
                    Shake();
                }
                return;
            }
        }
        Shake();
    }
    public void WaterActive(){
        haveWater = true;
        anim.SetBool("active", true);
        TileManager.instance.CheckWater();
    }
    public void SetWater(bool update = true){
        int[] windows = {0, 0, 0, 0, 0, 0};
        if(isOrigin){
            for(int i = 0; i < originCode.Length; i++){
                windows[dirs.IndexOf(originCode[i]+"")]=  1;
            }
        }
        for(int i = 0; i < 6; i++){
            if(between[i] == null){
                continue;
            }
            if(between[i].isWater){
                windows[i]=1;
            }
        }
        string code = "";
        for(int i = 0; i < 6; i++){
            if(windows[i] == 1){
                code += dirs[i];
            }
        }
        tileCode = code;
        type = TileType.River;
        ChangeTile();
        if(update){
            for(int i = 0; i < 6; i++){
                if(between[i] == null){
                    continue;
                }
                if(between[i].isWater){
                    between[i].SetWater(false);
                }
                if(between[i].type == TileType.House){
                    between[i].WaterActive();
                }
            }
        }

    }
    void ChangeTile(){
        if(useInMenu){
            return;
        }
        currentTile?.SetActive(false);
        switch(type){
            case TileType.Ground:
                currentTile = ground;
                isWater = false;
                break;
            case TileType.House:
                currentTile = houses[int.Parse(tileCode)];
                isWater = false;
                break;
            case TileType.Mountain:
                currentTile = mountains;
                isWater = false;
                break;
            case TileType.Forest:
                currentTile = forests;
                isWater = false;
                break;
            case TileType.River:
            Debug.Log(tileCode);
                currentTile = rivers[strTypes.IndexOf(tileCode)];
                isWater = true;
                break;
            default:
                currentTile = ground;
                isWater = false;
                break;
        }
        currentTile.SetActive(true);
    }
    public void Select(){
        tiles.DOLocalMoveY(selectUp, selectDuration).SetEase(selectEase, 5);
    }
    public void DeSelect(){
        tiles.DOLocalMoveY(0, selectDuration).SetEase(deSelectEase);
    }

}
