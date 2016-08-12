using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
public class ItemAssets : IStoreAssets
{

    /// <summary>
    /// see parent.
    /// </summary>
    public int GetVersion()
    {
        return 0;
    }

    /// <summary>
    /// see parent.
    /// </summary>
    public VirtualCurrency[] GetCurrencies()
    {
        return new VirtualCurrency[] { };
    }

    /// <summary>
    /// see parent.
    /// </summary>
    public VirtualGood[] GetGoods()
    {
        return new VirtualGood[] { LIVE_ITEM, ROPE_ITEM, BOMB_ITEM, TIME_ITEM, GLASS_ITEM, AUDIENCE_ITEM };
    }

    /// <summary>
    /// see parent.
    /// </summary>
    public VirtualCurrencyPack[] GetCurrencyPacks()
    {
        return new VirtualCurrencyPack[] { };
    }

    /// <summary>
    /// see parent.
    /// </summary>
    public VirtualCategory[] GetCategories()
    {
        return new VirtualCategory[] { GENERAL_CATEGORY };
    }

    /** Static Final Members *

    public static string LIVE_ITEM_ID = "live";
    //public static string LIVE_ITEM_ID = "android.test.purchased";

    public static string ROPE_ITEM_ID = "rope";
    //public static string ROPE_ITEM_ID = "android.test.canceled";

    public static string BOMB_ITEM_ID = "bomb";
    //public static string BOMB_ITEM_ID = "android.test.refunded";

    public static string TIME_ITEM_ID = "time";

    public static string GLASS_ITEM_ID = "glass"; // tornado ?// yuanyuan

    public static string AUDIENCE_ITEM_ID = "audience";


    /** Virtual Goods *

    public static VirtualGood LIVE_ITEM = new SingleUseVG(
            "Live",                                                         // name
            "Give customers a live",                                        // description
            LIVE_ITEM_ID,                                                   // product id
            new PurchaseWithMarket(new MarketItem(LIVE_ITEM_ID, 0.99f)));    

    public static VirtualGood ROPE_ITEM = new SingleUseVG(
            "Skipping Rope",                                         	            // name
            "Gives customers a skipping chance",                                    // description
            ROPE_ITEM_ID,                                          		            // product id
            new PurchaseWithMarket(new MarketItem(ROPE_ITEM_ID, 0.99f))); 

    public static VirtualGood BOMB_ITEM = new SingleUseVG(
            "Question Bomb",                                   		    // name
            "Gives customers a bomb",	 		                        // description
            BOMB_ITEM_ID,                                   		    //  product id
            new PurchaseWithMarket(new MarketItem(BOMB_ITEM_ID, 0.99f)));  


    public static VirtualGood TIME_ITEM = new SingleUseVG(
            "Back In Time",                                        		// name
            "Increase question time",   	                            // description
            TIME_ITEM_ID,                                        		// product id
            new PurchaseWithMarket(new MarketItem(TIME_ITEM_ID, 0.99f)));  
    
    public static VirtualGood GLASS_ITEM = new SingleUseVG(
            "Hour Glass",                                        		// name
            "Gives customers hour glass chance",   	                    // description
            GLASS_ITEM_ID,                                        		// product id
            new PurchaseWithMarket(new MarketItem(GLASS_ITEM_ID, 0.99f)));  

    public static VirtualGood AUDIENCE_ITEM = new SingleUseVG(
            "Ask The Audience",                                        	// name
            "Gives customers to Ask The Audience",  	                // description
            AUDIENCE_ITEM_ID,                                        	// product id
            new PurchaseWithMarket(new MarketItem(AUDIENCE_ITEM_ID, 0.99f)));  


    /** Virtual Categories 
    // The muffin rush theme doesn't support categories, so we just put everything under a general category.
    public static VirtualCategory GENERAL_CATEGORY = new VirtualCategory(
            "General", new List<string>(new string[] { LIVE_ITEM_ID, ROPE_ITEM_ID, BOMB_ITEM_ID, TIME_ITEM_ID, GLASS_ITEM_ID, AUDIENCE_ITEM_ID})
    );
}
*/