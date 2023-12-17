using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NpcActionHandler
{
    public static void SetNpcParam(NpcController npc, NpcButtonHandler handler, Dictionary<int, NpcController> npcList) {
        if ((handler.param == null) || (handler.param.Count < 2))
            return;

        int id = int.Parse(handler.param[0]);
        for (int i = 1; i < handler.param.Count; i++) {
            var option = handler.param[i].Split('=');
            switch (option[0]) {
                default:
                    break;
                case "active":
                    npcList.Get(id, npc).SetActive(bool.Parse(option[1]));
                    break;
                case "sprite":
                    npcList.Get(id, npc).SetIcon(option[1]);
                    break;
                case "color":
                    npcList.Get(id, npc).SetColor(option[1].ToColor(Color.white));
                    break;
            }
        }
    }

    public static async void OpenHintbox(NpcButtonHandler handler) {
        if ((handler.param == null) || (handler.param.Count == 0))
            return;
        
        Hintbox hintbox;
        var type = handler.param[0].Split('=');
        hintbox = type[1] switch {
            "Item" => Hintbox.OpenHintbox<ItemHintbox>(),
            _ => Hintbox.OpenHintbox()
        };
        
        for (int i = 1; i < handler.param.Count; i++) {
            var option = handler.param[i].Split('=');
            switch (option[0]) {
                default:
                    break;
                case "title":
                    hintbox.SetTitle(option[1]);
                    break;
                case "content":
                    hintbox.SetContent(option[1], 14, FontOption.Arial);
                    break;
                case "option_num":
                    hintbox.SetOptionNum(int.Parse(option[1]));
                    break;
                case "item_icon":
                    Sprite icon = null;
                    var splitIndex = option[1].IndexOf('[');
                    if (splitIndex != -1) {
                        var category = option[1].Substring(0, splitIndex).ToLower();
                        if (int.TryParse(option[1].TrimParentheses(), out int id)) {
                            icon = category switch {
                                "pet" => await Pet.GetPetInfo(id)?.ui.icon,
                                "item" => await Item.GetItemInfo(id)?.icon,
                                "emblem" => await Pet.GetPetInfo(id)?.ui.emblemIcon,
                                _ => await NpcInfo.GetIcon(option[1]),
                            };
                        }
                    }
                    icon ??= await NpcInfo.GetIcon(option[1]);
                    ((ItemHintbox)hintbox)?.SetIcon(icon);
                    break;
            }
        }
    }

    public static void OpenPanel(NpcButtonHandler handler) {
        if ((handler.param == null) || (handler.param.Count == 0))
            return;

        Panel.OpenPanel(handler.param[0]);
    }

    public static void OpenDialog(NpcInfo npcInfo, NpcButtonHandler handler) {
        if ((handler.param == null) || (handler.param.Count == 0))
            return;

        if (handler.param[0] == "null") {
            DialogManager.instance.CloseDialog();
            return;
        }
        DialogInfo dialogInfo = npcInfo.dialogHandler.Find(x => x.id == handler.param[0]);
        DialogManager.instance.SetCurrentNpc(npcInfo);
        DialogManager.instance.OpenDialog(dialogInfo);
    }

    public static void Teleport(NpcButtonHandler handler) {
        if ((handler.param == null) || (handler.param.Count == 0))
            return;

        int mapId = int.Parse(handler.param[0]);
        if (handler.param.Count == 1) {
            TeleportHandler.Teleport(mapId);
            return;
        }
        TeleportHandler.Teleport(mapId, handler.param[1].ToVector2());
    }

    public static void SetItem(NpcButtonHandler handler) {
        if ((handler.param == null) || (handler.param.Count < 2))
            return;

        Action<Item> itemFunc = handler.param[0] switch {
            "add" => Item.Add,
            "remove" => (x) => Item.Remove(x.id, x.num),
            _ => Item.Add
        };
        for (int i = 1; i < handler.param.Count; i++) {
            var itemInfo = handler.param[i].ToIntList();
            itemFunc.Invoke(new Item(itemInfo[0], itemInfo[1]));
        }
    }

    public static void GetPet(NpcButtonHandler handler) {
        if ((handler.param == null) || (handler.param.Count == 0))
            return;

        for (int i = 0; i < handler.param.Count; i++) {
            var petInfo = handler.param[i].ToIntList();
            int count = petInfo.Count;
            int id = petInfo[0];
            int level = (count < 2) ? 1 : petInfo[1];
            bool emblem = (count < 3) ? true : (petInfo[2] != 0);
            Pet pet = new Pet(id, level, emblem);
            Player.instance.gameData.petStorage.Add(pet);
        }
        SaveSystem.SaveData();
    }

    public static void SetPet(NpcButtonHandler handler) {
        if ((handler.param == null) || (handler.param.Count == 0))
            return;

        for (int i = 0; i < handler.param.Count; i++) {
            var petInfo = handler.param[i].Split('=');
            if (petInfo.Length < 2)
                continue;

            Player.instance.gameData.petBag[0].SetPetIdentifier(petInfo[0], float.Parse(petInfo[1]));
        }
        SaveSystem.SaveData();
    }

    public static void EvolvePet(NpcButtonHandler handler) {
        if ((handler.param == null) || (handler.param.Count == 0))
            return;

        for (int i = 0; i < handler.param.Count; i++) {
            var petInfo = handler.param[i].ToIntList();
            Player.instance.gameData.petBag[0].MegaEvolve(petInfo[0]);
        }
        SaveSystem.SaveData();
    }

    public static void SetMission(NpcButtonHandler handler) {
        if ((handler.param == null) || (handler.param.Count < 2))
            return;

        int id = int.Parse(handler.param[0]);
        switch (handler.param[1]) {
            case "start":
                Mission.Start(id);
                break;
            case "complete":
                Mission.Complete(id);
                break;
            case "checkpoint":
                Mission.Checkpoint(id, handler.param[2]);
                break;
        }
        SaveSystem.SaveData();
    }

    public static void SetActivity(NpcButtonHandler handler) {
        if ((handler.param == null) || (handler.param.Count < 1))
            return;
            
        var activity = Activity.Find(handler.param[0]);
        for (int i = 1; i < handler.param.Count; i++) {
            var option = handler.param[i].Split('=');
            activity.SetData(option[0], option[1]);
        }
    }

    public static void StartBattle(NpcInfo npcInfo, NpcButtonHandler handler) {
        if ((handler.param == null) || (handler.param.Count == 0))
            return;

        string id = handler.param[0];
        Player.instance.currentNpcId = (npcInfo == null) ? 0 : npcInfo.id;
        BattleInfo battleInfo = npcInfo.battleHandler.Find(x => x.id == handler.param[0]);

        bool isPlayerPetBag = (battleInfo.playerInfo == null) || (battleInfo.playerInfo.Count == 0);
        bool isFirstPetDead = (Player.instance.petBag[0] == null) || (Player.instance.petBag[0].currentStatus.hp == 0);
        if (isPlayerPetBag && isFirstPetDead) {
            Hintbox hintbox = Hintbox.OpenHintbox();
            hintbox.SetTitle("提示");
            hintbox.SetContent("首发精灵血量耗尽，快去恢复精灵吧！", 14, FontOption.Arial);
            hintbox.SetOptionNum(1);
            return;
        }
        Battle battle = new Battle(battleInfo);
        SceneLoader.instance.ChangeScene(SceneId.Battle);
    }
}