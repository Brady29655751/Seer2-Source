using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;

public class PetUI
{
    [XmlIgnore] public PetUIInfo info => Database.instance.GetPetInfo(id)?.ui;
    [XmlAttribute] public int id;
    [XmlAttribute] public int baseId;
    [XmlIgnore] public int skinId {
        get => (id == 0) ? info.defaultSkinId : id;
        set => id = value;
    }
    [XmlIgnore] public int skinBaseId {
        get => (baseId == 0) ? info.baseId : baseId;
        set => baseId = value;
    }
    [XmlIgnore] public Task<Sprite> icon => PetUISystem.GetPetIcon(skinId);
    [XmlIgnore] public Task<Sprite> emblemIcon => PetUISystem.GetEmblemIcon(skinBaseId);
    [XmlIgnore] public Task<RuntimeAnimatorController> animatorController => PetUISystem.GetAnimatorController(skinId);

    public PetUI() {}

    public PetUI(int id, int baseId) {
        this.id = id;
        this.baseId = baseId;
    }

    public PetUI(PetUI rhs) {
        id = rhs.id;
        baseId = rhs.baseId;
    }
}