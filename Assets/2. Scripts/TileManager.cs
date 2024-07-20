using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TileManager : MonoBehaviour
{
    public static TileManager instance;
    public List<Tile> houseTile = new List<Tile>();
    public Transform activeCaller;
    public Transform inActiveCaller;
    public float duration;
    public float value;
    public GameObject input;
    public Transform hpParent;
    public Transform uiParent;
    public List<Transform> hps;
    public string sceneName;
    bool useUI = true;
    private void Awake() {
        instance = this;
        input.SetActive(false);
    }

    private void Start() {
        if(sceneName !="Menu"){
        for(int i = 0; i < hpParent.childCount; i++){
            hps.Add(hpParent.GetChild(i));
            hpParent.GetChild(i).DOScale(1, 0.3f).From(0).SetEase(Ease.OutBack);
        }
        for(int i = 0; i < uiParent.childCount; i++){
            uiParent.GetChild(i).DOScale(1, 0.3f).From(0).SetEase(Ease.OutBack);
        }
        }
        
        Delay(() => {
            CallActive();
        }, 0.00f);
        Delay(() => {
            input.SetActive(true);
        }, 2.5f);
    }
    bool GetWater(){
        for(int i = 0; i <houseTile.Count; i++){
            if(!houseTile[i].haveWater){
                return false;
            }
        }
        return true;
    }
    public void CheckWater(){
        if(GetWater()){
            OffUI();
            Delay(() => {

            CallInActive();
            Delay(() => {
                DOTween.KillAll();
                UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
            }, 2.5f);
            }, 1);
        }
    }
    public void OffUI(){
        for(int i = 0; i < hpParent.childCount; i++){
            hpParent.GetChild(i).DOScale(0, 0.3f).SetEase(Ease.InBack);
        }
        for(int i = 0; i < uiParent.childCount; i++){
            uiParent.GetChild(i).DOScale(0, 0.3f).From(1).SetEase(Ease.InBack);
        }
    }
    public void Delay(System.Action action, float delay){
        StartCoroutine(DelayCor(action, delay));
    }
    IEnumerator DelayCor(System.Action action, float delay){
        float count = 0;
        while(count < delay){
            count += Time.deltaTime;
            yield return 0;
        }
        action();
    }
    public bool Mine(){
        if(hps.Count > 0){
            hps[^1].transform.DOScale(0, 0.5f).SetEase(Ease.InBack);
            hps.RemoveAt(hps.Count-1);
            return true;
        }
        return false;
    }
    public void CallActive(){
        activeCaller.DOLocalMoveX(value, duration).From(-value).SetEase(Ease.Linear);
    }
    public void CallInActive(){
        inActiveCaller.DOLocalMoveX(value, duration).From(-value).SetEase(Ease.Linear);
        input.SetActive(false);
    }
    public void Restart(){
        if(!useUI){
            return;
        }
        useUI = false;
        CallInActive();
        Delay(() => {
            DOTween.KillAll();
UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);

        }, 2.5f);
    }
    public void BackToMenu(){
        if(!useUI){
            return;
        }
        useUI = false;
CallInActive();
        Delay(() => {
            DOTween.KillAll();
UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");

        }, 2.5f);
    }
}