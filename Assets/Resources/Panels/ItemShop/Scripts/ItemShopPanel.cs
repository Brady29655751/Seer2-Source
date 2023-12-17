using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemShopPanel : Panel
{
    [SerializeField] protected ItemShopType shopType;
    [SerializeField] protected ItemShopController shopController;

    protected Dictionary<ItemShopType, string> shopNameDict = new Dictionary<ItemShopType, string>() {
        { ItemShopType.None, "道具商店" },
        { ItemShopType.PetPotion, "精灵道具商店" },
    };

    protected Dictionary<ItemShopType, List<int>> shopItemIdDict = new Dictionary<ItemShopType, List<int>>() {
        { ItemShopType.None, new List<int>() },
        { ItemShopType.PetPotion, new List<int>() { 
            10001, 10002, 10003, 10004, 10005,
            10011, 10012, 10013, 10014, 10015, 10016
        } },
    };

    public override void Init()
    {
        base.Init();
        SetTitle();
        SetStorage();
    }

    public void SetShopType(ItemShopType shopType) {
        if (this.shopType == shopType)
            return;

        this.shopType = shopType;
        SetTitle();
        SetStorage();
    }

    private void SetTitle() {
        shopController.SetTitle(shopNameDict.Get(shopType, "道具商店"));
    }

    private void SetStorage() {
        var idList = shopItemIdDict.Get(shopType, new List<int>());
        var itemList = idList.Select(x => new Item(x, -1)).ToList();
        shopController.SetStorage(itemList);
    }

}

public enum ItemShopType {
    None,
    PetPotion,
}