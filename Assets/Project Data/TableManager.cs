using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class Data
{
    public string Name;
    public string Type;
    public string Reward;
    public string Currency;

    public Data(string name, string type, string reward, string currency)
    {
        Name = name;
        Type = type;
        Reward = reward;
        Currency = currency;
    }
}
public class TableManager : MonoBehaviour
{
    public GameObject rowPrefab; // Prefab của RowTemplate
    public Transform content; // Vị trí của Content trong ScrollView
    public TextMeshProUGUI pageText; // Text hiển thị số trang
    public Button prevPageButton, nextPageButton; // Nút điều hướng
    private List<Data> dataList = new List<Data>();
    private int currentPage = 0;
    private int rowsPerPage = 10; // Số hàng mỗi trang


    void Start()
    {
        // Giả lập dữ liệu
        for (int i = 0; i < 12; i++)
        {
            dataList.Add(new Data("Name", "Type", "0.005", "100"));
        }

        // Gán sự kiện cho nút
        prevPageButton.onClick.AddListener(PreviousPage);
        nextPageButton.onClick.AddListener(NextPage);

        // Hiển thị trang đầu tiên
        DisplayPage(currentPage);
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

            columns[0].text = dataList[i].Name;
            columns[1].text = dataList[i].Type;
            columns[2].text = dataList[i].Reward + " × " + dataList[i].Currency;
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
