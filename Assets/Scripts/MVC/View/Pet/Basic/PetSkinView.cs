using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetSkinView : Module
{
    [SerializeField] private List<PetSelectBlockView> petSelectBlockViews;
    [SerializeField] private Animator petAnimator;

    public void SetSkins(List<int> skinIds) {
        for (int i = 0; i < petSelectBlockViews.Count; i++) {
            Pet pet = Pet.GetExamplePet((i < skinIds.Count) ? skinIds[i] : 0);
            petSelectBlockViews[i].SetPet(pet);
        }
    }

    public void Select(int index) {
        for (int i = 0; i < petSelectBlockViews.Count; i++) {
            petSelectBlockViews[i].SetChosen(i == index);
        }        
    }

    public async void SetPetAnimation(int skinId) {
        petAnimator.runtimeAnimatorController = await Pet.GetPetInfo(skinId)?.ui.animatorController;
    }

    public void OnSkinConfirm() {
        Hintbox hintbox = Hintbox.OpenHintbox();
        hintbox.SetTitle("提示");
        hintbox.SetContent("成功替换精灵皮肤", 16, FontOption.Arial);
        hintbox.SetOptionNum(1);
    }
}
