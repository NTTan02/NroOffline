using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Slider slider;
    private int maxHP;

    public void Init(int hp)
    {
        maxHP = hp;
        slider.maxValue = hp;
        slider.value = hp;
    }

    public void UpdateBar(int current)
    {
        slider.value = current;
    }

    void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
    }
}