using System.Linq;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;

public static class PetUISystem
{
    public static Task<Sprite> GetPetIcon(int petId) {
        return Addressables.LoadAssetAsync<Sprite>("Pets/" + petId + "/icon.png").Task;
    }

    public static Task<Sprite> GetEmblemIcon(int petId) {
        return Addressables.LoadAssetAsync<Sprite>("Emblems/" + petId).Task;
    }

    public static Task<RuntimeAnimatorController> GetAnimatorController(int petId) {
        return Addressables.LoadAssetAsync<RuntimeAnimatorController>("Pets/" + petId + "/anim.controller").Task;
    }

    public static Sprite GetSprite(this Element element) {
        int index = (int)element;
        return ResourceManager.instance.GetSprite("Elements/" + index.ToString());
    }

    public static Sprite GetSprite(this IVRanking ranking) {
        int index = (int)ranking;
        return ResourceManager.instance.GetSprite("IVRank/" + index.ToString());
    }

}

public enum PetAnimationType {
    None = 0,
    Idle = 1,
    Dying = 2,
    Win = 3,
    Lose = 4,
    CaptureSuccess = 5,
    CaptureFail = 6,
    Hurt = 7,
    Evade = 8,
    Physic = 9,
    Special = 10,
    Property = 11,
    Super = 12,
    SecondSuper = 13,
}
