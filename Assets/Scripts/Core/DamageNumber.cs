using UnityEngine;
using TMPro;
using System.Collections;

public class DamageNumber : MonoBehaviour
{
    public TMP_Text text;
    private Vector3 velocity = new Vector3(0, 2f, 0);

    public static void Show(Vector3 pos, int amount, Color color, string msg = "")
    {
        var prefab = Resources.Load<GameObject>("DamageNumber");
        if (prefab == null) return;

        var go = Instantiate(prefab, pos + Vector3.up * 0.5f, Quaternion.identity);
        var dn = go.GetComponent<DamageNumber>();
        dn.text.text  = msg != "" ? msg : amount.ToString();
        dn.text.color = color;
    }

    void Update()
    {
        transform.position += velocity * Time.deltaTime;
        velocity *= 0.9f;
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.8f);
        Destroy(gameObject);
    }
}