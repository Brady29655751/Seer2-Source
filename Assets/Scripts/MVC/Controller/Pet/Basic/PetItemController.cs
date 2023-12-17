using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetItemController : Module
{
    [SerializeField] private PetItemModel itemModel;
    [SerializeField] private PetItemView itemView;
    [SerializeField] private PageView pageView;

    public event Action<Item, int> onItemUsedEvent;
    public event Action<Item> onItemSelectEvent;

    public Item[] GetSelectedItem() {
        return itemModel.currentSelectedItems;
    }

    public void SetPet(Pet pet) {
        itemModel.SetPet(pet);
    }

    public void SetItemBag(List<Item> items) {
        itemModel.SetStorage(items);
        OnItemSetPage();
    }

    public void Select(int index) {
        itemModel.Select(index);
        itemView.Select(index);
        onItemSelectEvent?.Invoke(itemModel.items[index]);
    }

    public void OnItemButtonClick(int index) {
        Item item = itemModel.items[index];

        if (item == null)
            return;

        if (!item.IsUsable(itemModel.currentPet, null)) {
            OnItemUsed(null, -1);
            return;
        }
        OnSelectItemUsedNum(item);
    }

    private void OnItemUsed(Item item, int usedNum) {
        bool success = (item != null);

        Hintbox hintbox = Hintbox.OpenHintbox();
        hintbox.SetTitle(success ? "使用成功" : "使用失败");
        hintbox.SetContent(success ? ("已使用 " + usedNum + " 个 " + item.name) : "这只精灵目前无法使用此道具", 14, FontOption.Arial);
        hintbox.SetOptionNum(1);

        if (!success)
            return;

        onItemUsedEvent?.Invoke(item, usedNum);    
    }

    private async void OnSelectItemUsedNum(Item item) {
        SelectNumHintbox ihb = Hintbox.OpenHintbox<SelectNumHintbox>();
        ihb.SetTitle(item.name);
        ihb.SetContent(item.info.effectDescription, 14, FontOption.Arial);
        ihb.SetIcon(await item.icon);
        ihb.SetMinValue(1);
        ihb.SetMaxValue(Mathf.Min(item.num, item.GetMaxUseCount(itemModel.currentPet, null), 999));
        ihb.SetOptionNum(2);
        ihb.SetOptionCallback(() => {
            int usedNum = item.Use(itemModel.currentPet, null, ihb.GetInputValue());
            OnItemUsed(item, usedNum);
        }, true); 
    }

    public void SetInfoPromptActive(bool active) {
        itemView.SetInfoPromptActive(active);
    }

    public void ShowItemInfo(int index) {
        if (!index.IsInRange(0, itemModel.items.Length)) {
            SetInfoPromptActive(false);
            return;
        }

        itemView.ShowItemInfo(itemModel.items[index]);
    }

    public void OnItemSetPage() {
        itemView.SetItems(itemModel.selections.ToList());
        pageView?.SetPage(itemModel.page, itemModel.lastPage);
        Select(0);
    }

    public void OnItemPrevPage() {
        itemModel.PrevPage();
        OnItemSetPage();
    }

    public void OnItemNextPage() {
        itemModel.NextPage();
        OnItemSetPage();
    }



}