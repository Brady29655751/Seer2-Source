using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAudioView : BattleBaseView
{
    [SerializeField] private AudioClip endLoadingSound;
    [SerializeField] private AudioClip[] battleBGM;

    public override void Init()
    {
        StartCoroutine(PlayBattleBGMRoutine());
    }

    private IEnumerator PlayBattleBGMRoutine() {
        AudioSystem.instance.PlaySound(endLoadingSound, AudioVolumeType.BattleSE);
        
        yield return new WaitForSeconds(1.5f);

        AudioClip bgm = GetBattleBGM(battle.settings.mode);
        AudioSystem.instance.PlayMusic(bgm, AudioVolumeType.BattleBGM);

    }

    private AudioClip GetBattleBGM(BattleMode mode) {
        switch (mode) {
            default:
                return battleBGM[0];
            case BattleMode.Normal:
                return battleBGM[0];
            case BattleMode.PVP:
                return battleBGM[2];
        }
    }
}
