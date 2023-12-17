using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PetDemoView : Module
{
    [SerializeField] private bool featurePromptLeft = false;

    [SerializeField] private InfoPrompt infoPrompt;
    [SerializeField] private Animator petAnimator;
    [SerializeField] private Text nameText;

    [SerializeField] private IButton featureButton;
    [SerializeField] private Text featureText;

    [SerializeField] private IButton elementButton;

    [SerializeField] private Image genderImage;
    [SerializeField] private Image specialGenderImage;
    
    [SerializeField] private IButton emblemButton;
    
    [SerializeField] private IButton IVButton;

    public async void SetPet(Pet pet) {
        gameObject.SetActive(pet != null);
        if (pet == null)
            return;
        
        SetName(pet.name);
        SetElement(pet.element);
        SetFeature(pet.info.feature.feature);
        SetGender(pet.info.basic.gender);
        SetEmblem(pet.hasEmblem, pet.info.feature.emblem);
        SetIVRank(pet.talent.IVRank);
        SetAnimation(await pet.ui.animatorController);
    }

    public void SetInfoPromptActive(bool active) {
        infoPrompt.SetActive(active);
    }

    public void SetName(string name) {
        nameText.text = name;
    }

    public void SetElement(Element element) {
        elementButton?.image?.SetElementSprite(element);
    }

    public void SetElementInfoPromptContent(Element element) {
        string text = element.ToString();
        Vector2 size = new Vector2(30 + 10 * text.Length, 30);
        infoPrompt.SetInfoPromptWithAutoSize(text, TextAnchor.MiddleCenter);
    }

    public void SetFeature(Feature feature) {
        featureText.text = feature.name;
    }

    public void SetFeatureInfoPromptContent(Feature feature) {
        string text = feature.description;
        infoPrompt.SetInfoPromptWithAutoSize(text, TextAnchor.MiddleLeft);
        if (featurePromptLeft) {
            infoPrompt.SetPosition(new Vector2(-infoPrompt.size.x, 2));
        }
    } 

    public void SetGender(int gender) {
        genderImage.SetGenderSprite(gender);
        specialGenderImage.gameObject.SetActive(gender == 2);
    }

    public async void SetEmblem(bool hasEmblem, Emblem emblem) {
        Sprite nullEmblem = Emblem.GetNullEmblemSprite();
        emblemButton.SetSprite(hasEmblem ? await emblem.GetSprite() : nullEmblem);
    }

    public void SetEmblemInfoPromptContent(Emblem emblem, int index = 0) {
        if (emblem == null)
            return;

        string text = emblem.description;
        infoPrompt.SetInfoPromptWithAutoSize(text, TextAnchor.MiddleLeft);
    }

    public void SetIVRank(IVRanking ranking) {
        IVButton.SetSprite(ranking.GetSprite());
    }

    public void SetIVInfoPromptContent(int iv) {
        string text = "个体值︰" + iv.ToString();
        Vector2 size = new Vector2(95, 30);
        infoPrompt.SetInfoPromptWithAutoSize(text, TextAnchor.MiddleCenter);
    }

    public void SetAnimation(RuntimeAnimatorController animatorController) {
        petAnimator.runtimeAnimatorController = animatorController;
    }
}
