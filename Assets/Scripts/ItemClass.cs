using System;
using UnityEngine;

public abstract class ItemData
{
    public int id;
    public string uniqueID;
    public string iconPath;
    public ItemCategory category;
    public ItemRank rank;

    public string name;
    public string description;
    protected int itemCount = 0;
    //public Sprite Icon  => ResourceManager.Instance.SafeInvoke(v=>v.GetSprite(iconPath));

    public event Action<ItemData> OnChanged;
    public ItemData(int id, string iconPath, string name = "", string desc = "")
    {
        this.id = id;
        this.iconPath = iconPath;
        this.name = name;
        this.description = desc; 
        this.uniqueID = Guid.NewGuid().ToString();
        rank = ItemRank.NONE;
    }

    public ItemData(ItemData other)
    {
        id = other.id;
        uniqueID = other.uniqueID;
        iconPath = other.iconPath;
        category = other.category;
        name = other.name;
        description = other.description;
        itemCount = other.itemCount;
        rank = other.rank;
    }

    public int GetCount() => itemCount;
    public virtual void SetCount(int newCount)
    {
        itemCount = newCount;
        NotifyChanged();
    }

    public virtual void ModifyCount(int delta)
    {
        itemCount += delta;
        if (itemCount < 0) itemCount = 0;
        NotifyChanged();
    }

    protected void NotifyChanged()
    {
        OnChanged?.Invoke(this);
    }

    public abstract ItemData Copy();

    //public string LocalizedName
    //{
    //    get
    //    {
    //        if (LocalizationManager.Instance != null)
    //            return LocalizationManager.Instance.GetText(name);
    //        else
    //            return name;
    //    }
    //}
    //public string LocalizedDescription
    //{
    //    get
    //    {
    //        if(LocalizationManager.Instance != null) 
    //            return LocalizationManager.Instance.GetText(description);
    //        return description; 
    //    }
    //}
}


public class IngredientItem
    : ItemData
{
    public IngredientItem(int id, string iconPath, int itemCategory) 
        : base(id, iconPath)
    {
        category = (ItemCategory)itemCategory;
        itemCount = 1; 
    }

    public IngredientItem(int id, string iconPath, ItemCategory category = ItemCategory.INGREDIANT
        , string name = "", string desc = "") 
        : base(id, iconPath, name, desc)
    {
        this.category = category;
        itemCount = 1; 
    }

    public override ItemData Copy()
    {
        var copy = new IngredientItem(id, iconPath, category, name, description);
        copy.uniqueID = uniqueID;
        copy.itemCount = itemCount;
        return copy;
    }
}

public class CurrencyItem
    : ItemData
{
    private CurrencyType type;
    public CurrencyType Type { get => type; }
    public CurrencyItem(int id, string iconPath, CurrencyType type, string name = "", string desc = "")
        : base(id, iconPath, name, desc)
    {
        category = ItemCategory.CURRENCY;
        this.type = type;
    }

    public override ItemData Copy()
    {
        var copy = new CurrencyItem(id, iconPath, Type, name ,description);
        copy.uniqueID = uniqueID;
        copy.itemCount = itemCount;
        return copy;
    }
}


public class ShopItem : ItemData
{
    private int targetItemID;
    public int TargetItemID => targetItemID;
    private CurrencyType currencyType;
    public CurrencyType CurrencyType => currencyType;
    private int price;
    public int Price => price; 
    public ItemData TargetItemData { get; private set; }

    public ShopItem(int id, int targetItemID, string iconPath ,
        int price = 0, CurrencyType currencyType = CurrencyType.NONE) 
        : base(id, iconPath)
    {
        this.price = price;
        this.targetItemID = targetItemID;
        this.currencyType = currencyType;
    }
    public ShopItem(ItemData data) :base(data) 
    { 
        if(data is ShopItem shopItem)
        {
            this.targetItemID = shopItem.targetItemID;
            this.price = shopItem.Price;
            this.currencyType = shopItem.currencyType;
        }
    }

    public void SetShopData(ShopItem shopItem)
    {
        if (shopItem == null) return;

        this.targetItemID = shopItem.targetItemID;
        this.price = shopItem.Price;
        this.currencyType = shopItem.currencyType;
    }

    public override ItemData Copy()
    {
        return new ShopItem(id, targetItemID, iconPath, price, currencyType);
    }
}