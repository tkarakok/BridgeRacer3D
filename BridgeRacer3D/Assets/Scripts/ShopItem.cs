using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public int itemId, wearId;
    public int price;

    public Text priceText;
    public Button buyButton, equipButton, unEquipButton;
    public GameObject itemPrefab;

    public bool HasItem()
    {

        // 0 - satın alınmamış 
        // 1 - satın alınmış ama giyilmemiş 
        // 2 - satın alınıp giyilmiş 
        bool hasItem = PlayerPrefs.GetInt("item" + itemId.ToString()) != 0;
        return hasItem;
    }

    public bool IsEquipped()
    {
        bool equippedItem = PlayerPrefs.GetInt("item" + itemId.ToString()) != 0;
        return equippedItem;
    }

    public void IntializeItem()
    {
        priceText.text = price.ToString();
        if (HasItem())
        {
            buyButton.gameObject.SetActive(false);
            if (IsEquipped())
            {
                EquipItem();
            }
            else
            {
                equipButton.gameObject.SetActive(true);
            }
        }
        else
        {
            buyButton.gameObject.SetActive(true);
        }

    }

    public void BuyButton()
    {
        if (!HasItem())
        {
            int money = PlayerPrefs.GetInt("money");
            if (money >= price)
            {
                PlayerController.Current.itemAudioSource.PlayOneShot(PlayerController.Current.buyAudioClip, 0.15f);
                LevelController.Current.GiveMoneyToPlayer(-price);
                PlayerPrefs.SetInt("item" + itemId.ToString(), 1);
                buyButton.gameObject.SetActive(false);
                equipButton.gameObject.SetActive(true);
            }
        }
    }

    public void EquipItem()
    {
        UnEquipItem();
        ShopController.Current.equippedItems[wearId] =Instantiate(itemPrefab, PlayerController.Current.wearSpots[wearId].transform).GetComponent<Item>();
        ShopController.Current.equippedItems[wearId].itemId = itemId;
        equipButton.gameObject.SetActive(false);
        unEquipButton.gameObject.SetActive(true);
        PlayerPrefs.SetInt("item" + itemId.ToString(), 2);
    }

    public void UnEquipItem()
    {
        Item equippedItem = ShopController.Current.equippedItems[wearId];
        if (equippedItem != null)
        {
            ShopItem shopItem = ShopController.Current.Items[equippedItem.itemId];
            PlayerPrefs.SetInt("item" + shopItem.itemId, 1);
            shopItem.equipButton.gameObject.SetActive(true);
            shopItem.unEquipButton.gameObject.SetActive(false);
            Destroy(equippedItem.gameObject);
        }
    }


    public void EquipItemButton(){
        PlayerController.Current.itemAudioSource.PlayOneShot(PlayerController.Current.equipItemAudioClip, .15f);
        EquipItem();
    }
     public void UnEquipItemButton(){
        PlayerController.Current.itemAudioSource.PlayOneShot(PlayerController.Current.unEquipItemAudioClip, .15f);
        UnEquipItem();
    }



}
