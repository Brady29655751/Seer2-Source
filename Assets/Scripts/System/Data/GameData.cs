﻿using System;
using System.Linq;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[XmlRoot("gameData")]
public class GameData
{
    public string version;
    public bool gender;
    public string nickname;
    [XmlIgnore] public int coin {
        get => Item.Find(Item.COIN_ID).num;
        set {
            var item = Item.Find(Item.COIN_ID);
            if (item == null)
                Item.Add(new Item(Item.COIN_ID, value));
            else 
                item.num = value;
        }
    }
    [XmlIgnore] public int diamond {
        get => Item.Find(Item.DIAMOND_ID).num;
        set {
            var item = Item.Find(Item.DIAMOND_ID);
            if (item == null)
                Item.Add(new Item(Item.DIAMOND_ID, value));
            else 
                item.num = value;
        }
    }
    public DateTime firstLoginDate;
    public DateTime lastLoginDate;

    public Pet[] petBag = new Pet[6];
    public List<Pet> petStorage = new List<Pet>();
    
    [XmlIgnore] public List<Item> itemBag { 
        get => itemStorage.OrderBy(x => x.id).ToList();
        set => itemStorage = value;
    }
    public List<Item> itemStorage = new List<Item>();
    public List<Mail> mailStorage = new List<Mail>();
    public List<Mission> missionStorage = new List<Mission>();
    public List<Activity> activityStorage = new List<Activity>();

    public SettingsData settingsData;

    public GameData() {
        InitGameData();
    }

    public void InitGameData() {
        version = string.Empty;
        firstLoginDate = DateTime.Now;
        lastLoginDate = DateTime.Now;

        gender = false;
        nickname = string.Empty;
        petBag = new Pet[6];
        petStorage = new List<Pet>();
        itemStorage = new List<Item>();
        mailStorage = new List<Mail>();
        missionStorage = new List<Mission>();
        activityStorage = new List<Activity>();

        settingsData = new SettingsData();
    }

    public static GameData GetDefaultData(int initCoin, int initDiamond) {
        GameData gameData = new GameData();
        gameData.petBag = new Pet[6] {
            new Pet(1, 10),
            new Pet(4, 10),
            new Pet(7, 10),
            new Pet(-1, 10),
            new Pet(-4, 10),
            new Pet(-7, 10)
        };
        gameData.itemStorage.Add(new Item(Item.COIN_ID, initCoin));
        gameData.itemStorage.Add(new Item(Item.DIAMOND_ID, initDiamond));
        gameData.itemStorage.Add(new Item(10237, 1));
        gameData.missionStorage.Add(new Mission(1));
        return gameData;
    }

    public bool IsEmpty() {
        return string.IsNullOrEmpty(nickname);
    }

}
