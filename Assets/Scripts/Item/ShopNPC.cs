using UnityEngine;

public class ShopNPC : MonoBehaviour
{
    [Header("Hàng bán")]
    public ItemData[] shopItems;

    [Header("UI")]
    public GameObject interactCanvas; 

    private bool playerNearby = false;

    void Start()
    {
        interactCanvas?.SetActive(false);
    }

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.F))
            ToggleShop();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            interactCanvas?.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            interactCanvas?.SetActive(false);
            ShopUI.Instance?.CloseShop();
        }
    }

    void ToggleShop()
    {
        if (ShopUI.Instance == null)
        {
            Debug.LogWarning("Không tìm thấy ShopUI!");
            return;
        }

        bool isOpen = ShopUI.Instance.shopPanel.activeSelf;

        if (isOpen)
            ShopUI.Instance.CloseShop();
        else
            ShopUI.Instance.OpenShop(shopItems);
    }
}
