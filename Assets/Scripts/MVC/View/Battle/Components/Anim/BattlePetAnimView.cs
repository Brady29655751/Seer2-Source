using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlePetAnimView : BattleBaseView
{
    [SerializeField] private IAnimator captureAnim;
    [SerializeField] private IAnimator petAnim;

    public bool isDone => isPetDone && isCaptureDone;
    protected bool isCaptureDone = true;
    protected bool isPetDone = true;
    protected string defalutSuperTrigger = "super";
    protected string defalutSecondSuperTrigger = "secondSuper";

    public override void Init()
    {
        base.Init();
        captureAnim.anim.runtimeAnimatorController = (RuntimeAnimatorController)Player.GetSceneData("captureAnim");
    }

    public async void SetPet(BattlePet pet) {
        SettingsData settingsData = Player.instance.gameData.settingsData;
        float animSpeed = (battle.settings.mode == BattleMode.PVP) ? 1f : settingsData.battleAnimSpeed;
        
        petAnim.anim.runtimeAnimatorController = await pet.ui.animatorController;
        petAnim.anim.SetFloat("speed", animSpeed);
        captureAnim.anim.SetFloat("speed", animSpeed);

        var animParams = new HashSet<string>(petAnim.anim.parameters.Select(x => x.name));
        var baseStatus = pet.info.basic.baseStatus;
        var defaultTrigger = (baseStatus.atk > baseStatus.mat) ? "physic" : "special";

        defalutSuperTrigger = animParams.Contains("super") ?  "super" : defaultTrigger;
        defalutSecondSuperTrigger = animParams.Contains("secondSuper") ?  "secondSuper" : defaultTrigger;
    }

    public void SetPetAnim(Skill skill, PetAnimationType type) {
        string trigger = type switch {
            PetAnimationType.CaptureSuccess => "success",
            PetAnimationType.CaptureFail => "fail",
            PetAnimationType.Physic => "physic",
            PetAnimationType.Special => "special",
            PetAnimationType.Property => "property",
            PetAnimationType.Super => defalutSuperTrigger,
            PetAnimationType.SecondSuper => defalutSecondSuperTrigger,
            _ => string.Empty
        };
        
        if (trigger == string.Empty)
            return;

        bool isCaptureSuccess = (type == PetAnimationType.CaptureSuccess);
        bool isCaptureFail = (type == PetAnimationType.CaptureFail);
        if (isCaptureSuccess || isCaptureFail) {
            isCaptureDone = false;
            captureAnim.onAnimHitEvent.SetListener(() => OnPetCapture(isCaptureSuccess));
            captureAnim.onAnimEndEvent.AddListener(OnPetEnd);
            captureAnim.anim.SetTrigger(trigger);
            return;
        }

        isPetDone = false;
        petAnim.transform.SetAsLastSibling();
        petAnim.onAnimHitEvent.SetListener(OnPetHit);
        petAnim.onAnimEndEvent.SetListener(OnPetEnd);
        petAnim.anim.SetTrigger(trigger);

        //! Prevent pet anim stuck, manually invoke event after timeout seconds.
        StartCoroutine(PreventAnimStuckCoroutine(8));
    }

    /*
    protected void PlaySkillSound(Skill skill) {
        string type = (skill.type == SkillType.必杀) ? "Super" : "Normal";
        string soundId = skill.soundId;
        AudioClip sound = ResourceManager.instance.GetSE("Skill/" + type + "/" + soundId);
        AudioSystem.instance.PlaySound(sound, AudioVolumeType.BattleSE);
        petAnim.onAnimStartEvent.RemoveAllListeners();
    }
    */

    protected IEnumerator PreventAnimStuckCoroutine(float timeout) {
        float stuckTime = 0;
        while (stuckTime < timeout) {
            if (isPetDone)
                yield break;
            
            stuckTime += 2f;
            yield return new WaitForSeconds(2);
        }
        OnPetHit();
        yield return new WaitForSeconds(2);
        OnPetEnd();
    }

    protected void OnPetCapture(bool isSuccess) {
        petAnim.gameObject.SetActive(!isSuccess);
    }

    protected void OnPetHit() {
        UI.ProcessQuery(true);   
    }

    protected void OnPetEnd() {
        isPetDone = true;
        isCaptureDone = true;
    }

}