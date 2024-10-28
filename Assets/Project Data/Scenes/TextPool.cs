using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TextPool : MonoBehaviour
{
    public TextMeshProUGUI textPrefab; // Prefab của Text
    public int poolSize = 20; // Kích thước của pool
    private List<TextMeshProUGUI> textPool; // Danh sách các Text trong pool

    void Start()
    {
        // Tạo pool với các đối tượng Text
        textPool = new List<TextMeshProUGUI>();

        for (int i = 0; i < poolSize; i++)
        {
            TextMeshProUGUI newText = Instantiate(textPrefab, transform);
            newText.gameObject.SetActive(false);
            textPool.Add(newText);
        }
    }

    // Lấy đối tượng Text không hoạt động từ pool
    public TextMeshProUGUI GetPooledText()
    {
        foreach (TextMeshProUGUI t in textPool)
        {
            if (!t.gameObject.activeInHierarchy)
            {
                return t;
            }
        }

        return null; // Trả về null nếu không có đối tượng nào sẵn sàng
    }
}
