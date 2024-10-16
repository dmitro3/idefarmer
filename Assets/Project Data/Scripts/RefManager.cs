using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Project_Data.Scripts;
using System.Runtime.InteropServices;
using UnityEditor;
public class DataRef
{
    public string Name;
    public string Type;
    public string Reward;
    public string Currency;

    public DataRef(string name, string type, string reward, string currency)
    {
        Name = name;
        Type = type;
        Reward = reward;
        Currency = currency;
    }
}


public class RefManager : MonoBehaviour
{
    // Khai báo hàm JavaScript từ .jslib
    [DllImport("__Internal")]
    private static extern void ShareOnTelegram(string url, string message);
    [DllImport("__Internal")]
    private static extern void copyText(string str);

    public GameObject rowPrefab; // Prefab của RowTemplate
    public Transform content; // Vị trí của Content trong ScrollView
    public TextMeshProUGUI pageText; // Text hiển thị số trang
    public Button prevPageButton, nextPageButton; // Nút điều hướng
    private List<TotalData> dataList = new List<TotalData>();
    private int currentPage = 0;
    private int rowsPerPage = 10; // Số hàng mỗi trang
    public TextMeshProUGUI linkrefText;
    public TextMeshProUGUI wordText;
    void Start()
    {
        linkrefText.text = DataConfig.linkref+ UserDataManager.Instance.UserData.user.refCode;
        CreateItemPartner();
        // Gán sự kiện cho nút
        prevPageButton.onClick.AddListener(PreviousPage);
        nextPageButton.onClick.AddListener(NextPage);

      
    }
    public void CopyToClipboard()
    {
        if (linkrefText.text != null)
        {
            //GUIUtility.systemCopyBuffer = linkrefText.text;
            CopyTextToClipboard(linkrefText.text);
            GameManager.Instance.ShowToast("Text copied");
        }
    }
    // Hàm public để gọi hàm sao chép văn bản
    public void CopyTextToClipboard(string text)
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
                copyText(text);
        #else
                Debug.LogWarning("Copy text functionality is only available on WebGL platform.");
        #endif
    }

    public void Shared()
    {
        string refLink = DataConfig.linkref + UserDataManager.Instance.UserData.user.refCode;
        string linkToShare = "https://t.me/share/url?url=" + refLink + "&text=🌾 Ready to Farm & Earn?";

    #if UNITY_WEBGL && !UNITY_EDITOR
              ShareOnTelegram(linkToShare,"");
    #else
        Debug.LogWarning("Tính năng chia sẻ chỉ hoạt động trên build WebGL.");
    #endif

    }
    void CreateItemPartner()
    {
        GameManager.Instance.buyManager.GetEarnRef(() => {

            RefData data = UserDataManager.Instance.refData;
            dataList = data.totalData;
            wordText.text = "(" + dataList.Count + ")";
            // Hiển thị trang đầu tiên
            DisplayPage(currentPage);

        });

    }
    void DisplayPage(int pageIndex)
    {
        // Xóa các hàng hiện tại
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // Tính toán số hàng hiển thị trên trang hiện tại
        int start = pageIndex * rowsPerPage;
        int end = Mathf.Min(start + rowsPerPage, dataList.Count);

        // Tạo các hàng cho dữ liệu hiện tại
        for (int i = start; i < end; i++)
        {
            GameObject row = Instantiate(rowPrefab, content);
            TextMeshProUGUI[] columns = row.GetComponentsInChildren<TextMeshProUGUI>();

            columns[0].text = dataList[i].userName;
            columns[1].text = dataList[i].SHEEP.ToString();
            columns[2].text = dataList[i].TON.ToString();
            //columns[2].text = dataList[i].Reward + " × " + dataList[i].Currency;
        }

        // Cập nhật Text số trang
        pageText.text = $"{pageIndex + 1}/{Mathf.CeilToInt((float)dataList.Count / rowsPerPage)}";

        // Vô hiệu hóa nút nếu không có trang tiếp theo hoặc trước đó
        prevPageButton.interactable = pageIndex > 0;
        nextPageButton.interactable = end < dataList.Count;
    }
    void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            DisplayPage(currentPage);
        }
    }

    void NextPage()
    {
        if ((currentPage + 1) * rowsPerPage < dataList.Count)
        {
            currentPage++;
            DisplayPage(currentPage);
        }
    }
}
