using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

[Serializable]
public class NonConsumbleItem
{
    public string Name;
    public string Id;
}

public class ShopScript : MonoBehaviour, IStoreListener
{
    public static ShopScript instance;

    IStoreController m_StoreController;
    public NonConsumbleItem[] ncItem;

    Action action;
    private void Awake()
    {
        if (instance == null) instance = this;
    }
    void Start()
    {
        SetupBuilder();
    }

    void SetupBuilder()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct(ncItem[0].Id, ProductType.Consumable);
        builder.AddProduct(ncItem[1].Id, ProductType.NonConsumable);

        UnityPurchasing.Initialize(this, builder);
    }
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("Success");
        m_StoreController = controller;
    }
    public void NonConsumable_Press(Action action)
    {
        this.action = action;
        m_StoreController.InitiatePurchase(ncItem[0].Id);
    }
    public void NonConsumableRemoveAd_Press(Action action)
    {
        this.action = action;
        m_StoreController.InitiatePurchase(ncItem[1].Id);
    }
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        var product = purchaseEvent.purchasedProduct;

        Debug.Log(product.definition.id);
        if(product.definition.id == ncItem[0].Id || product.definition.id == ncItem[1].Id)
        {
            action?.Invoke();
        }

        return PurchaseProcessingResult.Complete;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("Failed " + error);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.Log("Failed " + error + message);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log("Failed");
    }
}
