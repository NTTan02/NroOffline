using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance;

    [Header("UI")]
    public Transform itemContainer;
    public GameObject itemSlotPrefab;
    public TMP_Text goldText;
    public GameObject shopPanel; 

    void Awake()
    {
        Instance = this;
        shopPanel?.SetActive(false);
    }

    public void OpenShop(ItemData[] items)
    {
        shopPanel?.SetActive(true);
        Time.timeScale = 0f;

        foreach (Transform child in itemContainer)
            Destroy(child.gameObject);
        foreach (var item in items)
        {
            var slot = Instantiate(itemSlotPrefab, itemContainer);
            slot.transform.Find("ItemIcon").GetComponent<Image>().sprite  = item.icon;
            slot.transform.Find("ItemName").GetComponent<TMP_Text>().text = item.itemName;
            slot.transform.Find("ItemPrice").GetComponent<TMP_Text>().text = $"{item.price} G";
            slot.transform.Find("ItemDesc").GetComponent<TMP_Text>().text  = item.description;

            ItemData captured = item;
            slot.transform.Find("BtnBuy").GetComponent<Button>()
                .onClick.AddListener(() => BuyItem(captured));
        }

        UpdateGoldText();
    }

    public void CloseShop()
    {
        shopPanel?.SetActive(false);
        Time.timeScale = 1f;
    }

    void BuyItem(ItemData item)
    {
        var gm = GameManager.Instance;
        if (gm.playerGold < item.price)
        {
            Debug.Log("Không đủ tiền!");
            return;
        }

        if (!Inventory.Instance.AddItem(item)) return;

        gm.playerGold -= item.price;
        UpdateGoldText();
        FindObjectOfType<HUDManager>()?.UpdateHUD();
    }

    void UpdateGoldText()
    {
        if (goldText != null)
            goldText.text = $"Vàng: {GameManager.Instance.playerGold} G";
    }
}
