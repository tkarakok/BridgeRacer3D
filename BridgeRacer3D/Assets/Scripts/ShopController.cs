using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    public static ShopController Current;
    public List<ShopItem> Items;
    public List<Item> equippedItems;
    public GameObject shopMenu;
    public void IntializeShopController()
    {
        Current = this;
        foreach (ShopItem item in Items)
        {
            item.IntializeItem();
        }
    }

    public void ActivateShopMenu(bool active){
        shopMenu.SetActive(active);
    }
}
