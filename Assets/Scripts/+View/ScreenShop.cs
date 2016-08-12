using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;

using System.Collections;
using System.Collections.Generic;

public class ScreenShop : CEBehaviour, IStoreListener, IPuzzleNetLogicDelegates
{
    const int DEFAULT_ITEM_COUNT = 3;

	public bool debugMode = false;

	// Unity IAP objects 
	private IStoreController m_Controller;
	//private IAppleExtensions m_AppleExtensions;

    [System.Serializable]
    public class ItemData
    {
        public Sprite sprite = null;
        public Image img = null;
        public Text name = null;
    }

    public List<ItemData> m_ItemDatas = new List<ItemData>();
    public Text m_TotalPriceWnd = null;

	private const string PURCHASE_ID_LIVES = "live";
	private const string PURCHASE_ID_TORNADO = "tornado";
	private const string PURCHASE_ID_BOMB = "bomb";
	private const string PURCHASE_ID_ROPE = "rope";
	private const string PURCHASE_ID_GLASS = "glass";
	private const string PURCHASE_ID_AUDIENCE = "audience";

	private static bool wasInitializedPaymentCtrl = false;

	ConfigurationBuilder m_purchasingBuilder = null;

	protected override void OnEnable()
	{
		base.OnEnable();

		if (m_purchasingBuilder == null)
		{
			var module = StandardPurchasingModule.Instance();

#if UNITY_EDITOR
			module.useMockBillingSystem = true; // Microsoft
			// The FakeStore supports: no-ui (always succeeding), basic ui (purchase pass/fail), and 
			// developer ui (initialization, purchase, failure code setting). These correspond to 
			// the FakeStoreUIMode Enum values passed into StandardPurchasingModule.useFakeStoreUIMode.
			module.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
#endif
			m_purchasingBuilder = ConfigurationBuilder.Instance(module);

			m_purchasingBuilder.Configure<IGooglePlayConfiguration>().SetPublicKey("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAzElFNjInBiULwgaxOx5Bw90S8MA/RaKeoSS/deaKM2n/CpRexVWwgaqYkIg9I+imE/xFNaYYegUEpcZQh15ThqPmAJAcQzUvg5ZyDPXA8x/BVdyLFD3KfjVwyzbA9PYZPXvIq/fOkJB0gXxTUWMeWI/zXhdc9GSdyUqM/NwtDubH9jAUybxjNbWf2g3N/kathFZTOBy5rZ8scBJLs/0ttdMXcXQojkxJC8UEHpI/mX3jeFU0Sk1gRBNsOBHjGemWJuUVQUAbz5GeN3TLf7JDF267jsO2DDI/4PY/fNZy1uf3eLIdQEapG8xqXsNJ/392L/Mf4JY1sAjFxKhvNLpr6wIDAQAB");

			// lives
			m_purchasingBuilder.AddProduct("live", ProductType.Consumable, new IDs
			{
				{"live", GooglePlay.Name},
				{"live", AppleAppStore.Name},
			});

			// lives
			m_purchasingBuilder.AddProduct("rope", ProductType.Consumable, new IDs
			{
				{"rope", GooglePlay.Name},
				{"rope", AppleAppStore.Name},
			});

			// lives
			m_purchasingBuilder.AddProduct("bomb", ProductType.Consumable, new IDs
			{
				{"bomb", GooglePlay.Name},
				{"bomb", AppleAppStore.Name},
			});

			// lives
			m_purchasingBuilder.AddProduct("tornado", ProductType.Consumable, new IDs
			{
				{"time", GooglePlay.Name},
				{"tornado", AppleAppStore.Name},
			});
			// lives
			m_purchasingBuilder.AddProduct("glass", ProductType.Consumable, new IDs
			{
				{"glass", GooglePlay.Name},
				{"glass", AppleAppStore.Name},
			});
			m_purchasingBuilder.AddProduct("audience", ProductType.Consumable, new IDs
			{
				{"audience", GooglePlay.Name},
				{"audience", AppleAppStore.Name},
			});


		}

		if (wasInitializedPaymentCtrl == false)
		{
			// Now we're ready to initialize Unity IAP.
			UnityPurchasing.Initialize(this, m_purchasingBuilder);
		
			if (wasInitializedPaymentCtrl == false)
				CELoadingBar.startLoadingAnimation("Initializing Billing...");
		}
	}

	protected override void OnDisable ()
	{
		base.OnDisable ();

		CELoadingBar.stopLoadingAnimation ();
	}

	void LateUpdate()
	{
		if (wasInitializedPaymentCtrl == true &&
			gameLogic.IsInGameState(GameLogic.GameState.Shop) &&
			CELoadingBar.isPlayingLoadingAni)
			CELoadingBar.stopLoadingAnimation ();
	}

	/// <summary>
	/// This will be called when Unity IAP has finished initialising.
	/// </summary>
	public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		wasInitializedPaymentCtrl = true;
		CELoadingBar.stopLoadingAnimation ();

		if (debugMode == true)
			gameLogic.ErrorBox ("OnInitialized");

		m_Controller = controller;
		//m_AppleExtensions = extensions.GetExtension<IAppleExtensions> ();

		//InitUI(controller.products.all);

		foreach (var item in controller.products.all)
		{
			if (item.availableToPurchase)
			{
				Debug.Log(string.Join(" - ",
					new[]
					{
						item.metadata.localizedTitle,
						item.metadata.localizedDescription,
						item.metadata.isoCurrencyCode,
						item.metadata.localizedPrice.ToString(),
						item.metadata.localizedPriceString
					}));
			}
		}

		// Populate the product menu now that we have Products
		for (int t = 0; t < m_Controller.products.all.Length; t++)
		{
			var item = m_Controller.products.all[t];
			var description = string.Format("{0} - {1}", item.metadata.localizedTitle, item.metadata.localizedPriceString);

			Debug.Log (description);
		}
	}

	public void OnInitializeFailed(InitializationFailureReason error)
	{
		wasInitializedPaymentCtrl = false;
		CELoadingBar.stopLoadingAnimation ();

		if (debugMode == true)
			gameLogic.ErrorBox ("OnInitializeFailed");

		switch (error)
		{
		case InitializationFailureReason.AppNotKnown:
			Debug.LogError("Is your App correctly uploaded on the relevant publisher console?");
			break;
		case InitializationFailureReason.PurchasingUnavailable:
			// Ask the user if billing is disabled in device settings.
			Debug.Log("Billing disabled!");
			break;
		case InitializationFailureReason.NoProductsAvailable:
			// Developer configuration error; check product metadata.
			Debug.Log("No products available for purchase!");
			break;
		}

		gameLogic.ChangeGameState (GameLogic.GameState.SetUp);
	}


	/// <summary>
	/// This will be called when a purchase completes.
	/// </summary>
	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
	{
		var id = e.purchasedProduct.definition.id;

		if (id == PURCHASE_ID_LIVES)
			gameData.userInfo.Lifes += 5;
		else if (id == PURCHASE_ID_TORNADO) {
			ItemInfo.InsertItem(ItemKinds.tornado, 3, true);
			PuzzleNetLogic.requestUpdatePowerUp (this, ItemKinds.tornado, 3, true, 0);
		}
		else if (id == PURCHASE_ID_BOMB) {
			ItemInfo.InsertItem(ItemKinds.bomb, 3, true);
			PuzzleNetLogic.requestUpdatePowerUp (this, ItemKinds.bomb, 3, true, 0);
		}
		else if (id == PURCHASE_ID_GLASS) {
			ItemInfo.InsertItem(ItemKinds.timer, 3, true);
			PuzzleNetLogic.requestUpdatePowerUp (this, ItemKinds.timer, 3, true, 0);
		}
		else if (id == PURCHASE_ID_AUDIENCE) {
			ItemInfo.InsertItem(ItemKinds.audience, 3, true);
			PuzzleNetLogic.requestUpdatePowerUp (this, ItemKinds.audience, 3, true, 0);
		}
		else if (id == PURCHASE_ID_ROPE) {
			ItemInfo.InsertItem(ItemKinds.rope, 3, true);
			PuzzleNetLogic.requestUpdatePowerUp (this, ItemKinds.rope, 3, true, 0);
		}

		// Indicate we have handled this purchase, we will not be informed of it again.x
		return PurchaseProcessingResult.Complete;
	}

	/// <summary>
	/// This will be called is an attempted purchase fails.
	/// </summary>
	public void OnPurchaseFailed(Product item, PurchaseFailureReason r)
	{
		Debug.Log("Purchase failed: " + item.definition.id);
		Debug.Log(r);

		if (debugMode == true)
			gameLogic.ErrorBox ("OnPurchaseFailed");
	}



	/// <summary>
	/// This will be called after a call to IAppleExtensions.RestoreTransactions().
	/// </summary>
	private void OnTransactionsRestored(bool success)
	{
		Debug.Log("Transactions restored.");

		if (debugMode == true)
			gameLogic.ErrorBox ("OnTransactionsRestored");
	}

	/// <summary>
	/// iOS Specific.
	/// This is called as part of Apple's 'Ask to buy' functionality,
	/// when a purchase is requested by a minor and referred to a parent
	/// for approval.
	/// 
	/// When the purchase is approved or rejected, the normal purchase events
	/// will fire.
	/// </summary>
	/// <param name="item">Item.</param>
	private void OnDeferred(Product item)
	{
		Debug.Log("Purchase deferred: " + item.definition.id);

		if (debugMode == true)
			gameLogic.ErrorBox ("OnDeferred");
	}


    public void OnBuyLives()
    {
		if (m_Controller != null)
			m_Controller.InitiatePurchase (PURCHASE_ID_LIVES);

		if (debugMode == true)
			gameLogic.ErrorBox ("OnBuyLives");
    }

	public void OnBuyTornado()
	{
		if (m_Controller != null)
			m_Controller.InitiatePurchase (PURCHASE_ID_TORNADO);

		if (debugMode == true)
			gameLogic.ErrorBox ("OnBuyTornado");
	}

	public void OnBuyBomb()
	{
		if (m_Controller != null)
			m_Controller.InitiatePurchase (PURCHASE_ID_BOMB);
	
		if (debugMode == true)
			gameLogic.ErrorBox ("OnBuyLives");
	}
	public void OnBuyGlass()
	{
		if (m_Controller != null)
			m_Controller.InitiatePurchase (PURCHASE_ID_GLASS);

		if (debugMode == true)
			gameLogic.ErrorBox ("OnBuyLives");
	}
	public void OnBuyRope()
	{
		if (m_Controller != null)
			m_Controller.InitiatePurchase (PURCHASE_ID_ROPE);

		if (debugMode == true)
			gameLogic.ErrorBox ("OnBuyLives");
	}
	public void OnBuyAudience()
	{
		if (m_Controller != null)
			m_Controller.InitiatePurchase (PURCHASE_ID_AUDIENCE);

		if (debugMode == true)
			gameLogic.ErrorBox ("OnBuyLives");
	}
    public void OnHelp()
    {

    }
	public void OnBackBtnClick()
	{
		gameLogic.ChangeGameState (GameLogic.GameState.SetUp);
	}

	public void willStartRequest(RequestState _state)
	{
	}
	public void parsingError(RequestState _state)
	{
	}
	public void didRecievedSuccessResponseFromServer(object _data, RequestState _state)
	{
	}
	public void didRecievedFailedResponseFromServer(string _message, RequestState _state)
	{
	}
	public void didReceivingError(string _error, RequestState _state)
	{
	}
	public void didReceivingTimeOut(RequestState _state)
	{
	}

}
