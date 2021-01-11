﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBag : Inventory
{


    // Start is called before the first frame update

    public SimpleBag()
    {
        //this.Items.Add(new Item { itemType = Item.ItemType.TV, amount = 1});
        //this.Items.Add(new Item { itemType = Item.ItemType.Tablet, amount = 1 });
        //this.Items.Add(new Item { itemType = Item.ItemType.Cash, amount = 5 });
        //this.Items.Add(new Item { itemType = Item.ItemType.Phone, amount = 2 });
    }

    public override void AddItem(Item item, GameObject itemObject)
    {
        counter++;
        this.TotalValue += item.value;
        itemObject.SetActive(false);
        ItemWrapper itemWrapper = new ItemWrapper(item, counter, itemObject);
        this.Items.Add(itemWrapper);
    }
}
