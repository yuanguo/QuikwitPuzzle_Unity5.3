using UnityEngine;
using System.Collections;


public enum ItemKinds
{
	tornado = 0,
	audience,
	timer,
	rope,
	bomb,
	blank,
}

public class ItemInfo : CEObject
{
	public ItemKinds kind = ItemKinds.blank;
	public int countOfItem = 0;

	private static System.Random m_randObj = new System.Random();
	public static ItemKinds m_lastItemKind = ItemKinds.blank;

	#region private and protected members
	protected OrderItemInfo m_orderInfo = null;
	#endregion

	public ItemInfo(ItemKinds _kind = ItemKinds.blank, int _count = 0)
	{
		kind = _kind;
		countOfItem = _count;
	}

	public static ItemInfo getRandomItem()
	{
		// add a randomized power up item 
		int randVale = m_randObj.Next((int)ItemKinds.blank);

		while ((ItemKinds)randVale == m_lastItemKind ||
			(ItemKinds)randVale == ItemKinds.blank) {
			randVale = m_randObj.Next((int)ItemKinds.blank);
		}
		m_lastItemKind = (ItemKinds)randVale;

		ItemInfo itemInfo = ItemInfo.InsertItem((ItemKinds)randVale, 1, true);

		return itemInfo;
	}

    public static ItemKinds getRandomItemKind()
    {
        // add a randomized power up item 
        int randVale = m_randObj.Next((int)ItemKinds.blank);

		while ((ItemKinds)randVale == m_lastItemKind ||
			(ItemKinds)randVale == ItemKinds.blank) {
			randVale = m_randObj.Next((int)ItemKinds.blank);
		}
		m_lastItemKind = (ItemKinds)randVale;

		return (ItemKinds)randVale;
    }

	public static ItemInfo InsertItem(ItemKinds _kind, int _count, bool _appendFlag = false)
	{
		ItemInfo info = null;
		if (GameData.Singleton.powerUpsItems.ContainsKey(_kind))
			info = GameData.Singleton.powerUpsItems[_kind];
		else
		{
			info = new ItemInfo(_kind, 0);
			GameData.Singleton.powerUpsItems.Add(_kind, info);
		}

		if (_appendFlag)
			info.countOfItem += _count;
		else
			info.countOfItem = _count;

		return info;
	}
}
