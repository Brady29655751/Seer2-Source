using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FightLoadingScreen : LoadingScreen
{
    private Battle battle => Player.instance.currentBattle;
    [SerializeField] private AudioClip startLoadingSound;
    [SerializeField] private BattlePetInfoView playerView, enemyView;

    public override void OnBeforeLoading(Action callback)
    {
        GameManager.instance.ChangeState(GameState.Fight);
        AudioSystem.instance.StopMusic();
        AudioSystem.instance.StopEffect();

        SetLoadingProgressNumber(0);
        playerView.SetPet(battle.currentState.myUnit.pet);
        enemyView.SetPet(battle.currentState.opUnit.pet);
        AudioSystem.instance.PlaySound(startLoadingSound, AudioVolumeType.BattleSE);
        StartCoroutine(WaitSecondsCoroutine(1.5f, callback));
    }

    protected override IEnumerator ChangeSceneCoroutine(int sceneIndex, Action finishedCallback = null) {
        int loadedResources = 0;

        Addressables.LoadAssetAsync<Sprite>("Maps/fightBg/" + Player.instance.currentMap.fightMapId).Completed += (handle) => {
            Player.SetSceneData("fightBg", handle.Result);
            loadedResources++;
        };

        Addressables.LoadAssetAsync<RuntimeAnimatorController>("Pets/capture/capture.controller").Completed += (handle) => {
            Player.SetSceneData("captureAnim", handle.Result);
            loadedResources++;
        };

        while (loadedResources < 2)
            yield return null;

        float progress = 0;
        if (PhotonNetwork.IsConnected) {
            if (PhotonNetwork.IsMasterClient) {
                PhotonNetwork.LoadLevel(sceneIndex);
            }
            while ((progress = PhotonNetwork.LevelLoadingProgress) < 1) {
                ShowLoadingProgress(progress);
                yield return null;
            }
        } else {
            var operation = SceneManager.LoadSceneAsync(sceneIndex);
            while (!operation.isDone) {
                ShowLoadingProgress(operation.progress / 0.9f);
                yield return null;
            }   
        }
        finishedCallback?.Invoke();
    }

    public override void OnAfterLoading(Action callback)
    {
        StartCoroutine(WaitSecondsCoroutine(1.2f, callback));
    }

}