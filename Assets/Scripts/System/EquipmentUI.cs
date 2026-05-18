using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquipmentUI : MonoBehaviour
{
    [Header("Body Slots — 6 ô trang bị")]
    public EquipmentSlotUI slotAo;
    public EquipmentSlotUI slotQuan;
    public EquipmentSlotUI slotGang;
    public EquipmentSlotUI slotGiay;
    public EquipmentSlotUI slotRada;
    public EquipmentSlotUI slotGiap;

    [Header("Stat Preview")]
    public TMP_Text statsText;
    public Image characterPreview;

    [Header("Panel")]
    public GameObject equipmentPanel;

    private bool isOpen = false;

    void Update()
    {
        // Bấm C để mở equipment
        if (Input.GetKeyDown(KeyCode.C))
            TogglePanel();
    }

    public void TogglePanel()
    {
        isOpen = !isOpen;
        equipmentPanel.SetActive(isOpen);
        if (isOpen) Refresh();
        Time.timeScale = isOpen ? 0f : 1f;
    }

    public void Refresh()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;
        var eq = EquipmentSystem.Instance;
        if (eq == null) return;

        // Cập nhật từng slot
        slotAo?.Setup(eq.equipped[EquipmentType.Ao],    EquipmentType.Ao);
        slotQuan?.Setup(eq.equipped[EquipmentType.Quan], EquipmentType.Quan);
        slotGang?.Setup(eq.equipped[EquipmentType.Gang], EquipmentType.Gang);
        slotGiay?.Setup(eq.equipped[EquipmentType.Giay], EquipmentType.Giay);
        slotRada?.Setup(eq.equipped[EquipmentType.Rada], EquipmentType.Rada);
        slotGiap?.Setup(eq.equipped[EquipmentType.Giap], EquipmentType.Giap);

        if (gm.selectedRace?.characterSprite != null)
            characterPreview.sprite = gm.selectedRace.characterSprite;

        // Hiện tổng stats
        var stats = FindObjectOfType<PlayerStats>();
        if (stats != null && statsText != null)
        {
            statsText.text =
                $"HP:  {stats.currentHP} / {stats.maxHP}\n" +
                $"MP:  {stats.currentMP} / {stats.maxMP}\n" +
                $"ATK: {stats.ATK} (+{eq.GetTotalBonusATK()})\n" +
                $"DEF: {stats.def}";
        }
    }

    public void ClosePanel()
    {
        isOpen = false;
        equipmentPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}