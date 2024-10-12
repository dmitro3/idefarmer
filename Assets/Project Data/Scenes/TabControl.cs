using Newtonsoft.Json;
using Project_Data.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabControl : MonoBehaviour
{
    public Button tab1Button;
    public Button tab2Button;
    public GameObject scrollView1;
    public GameObject scrollView2;
    public GameObject transformPartner;
    public GameObject transformDialy;
    public GameObject itemPartnerPrefab;
    public GameObject Tab1ImageBg;
    public GameObject Tab2ImageBg;
    // Import các hàm từ file OpenLinkTelegram.jslib
    [DllImport("__Internal")]
    private static extern void openLinkTelegramFromUnity(string link);

    [DllImport("__Internal")]
    private static extern void openLinkFromUnity(string link);
    // Start is called before the first frame update
    void Start()
    {
        // Gán sự kiện cho các nút tab
        tab1Button.onClick.AddListener(ShowScrollView1);
        tab2Button.onClick.AddListener(ShowScrollView2);
        CreateItemPartner();
        // Mặc định hiển thị ScrollView 1
        ShowScrollView1();
    }
    void CreateItemPartner()
    {
        GameManager.Instance.buyManager.GetQuest(() => {

            List<Quest> questData = UserDataManager.Instance.questData.Where(n =>!n.IsDone &&  n.QuestType.Equals("SOCIAL")).ToList();
            for (int i = 0; i < questData.Count; i++)
            {
                GameObject newItem = Instantiate(itemPartnerPrefab);
                newItem.transform.SetParent(transformPartner.transform);
                newItem.transform.localScale = Vector3.one;
                ItemQuest itemQuest = newItem.GetComponent<ItemQuest>();
                Quest quest = questData[i];
                itemQuest.Id = quest.Id;
                itemQuest.Name = quest.Name;
                itemQuest.Description = quest.Description;
                itemQuest.TonAmount = quest.TonAmount;
                itemQuest.SheepAmount = quest.SheepAmount;
                itemQuest.HyperLink = quest.HyperLink;
                itemQuest.QuestType = quest.QuestType;
                itemQuest.Active = quest.Active;
                itemQuest.Priority = quest.Priority;
                itemQuest.IsDoing = quest.IsDoing;
                itemQuest.IsDone = quest.IsDone;
                itemQuest.DoneAt = quest.DoneAt;

                TextMeshProUGUI[] texts = newItem.GetComponentsInChildren<TextMeshProUGUI>();

                foreach (TextMeshProUGUI txt in texts)
                {
                    if (txt.name == "nametask")
                    {
                        txt.text = quest.Name;
                       
                    }
                    else if (txt.name == "tontaskparneramount")
                    {

                        txt.text = "X" + quest.TonAmount.ToString("F2");

                    }
                    else if(txt.name == "sheeptaskAmount")
                    {
                        txt.text = "X" + quest.SheepAmount.ToString("F2");

                    }
                }

                // Tìm phần tử Button bên trong prefab
                Button button = newItem.GetComponentInChildren<Button>();

                // Đảm bảo button không null
                if (button != null)
                {
                    // Gắn sự kiện với tham số cho button
                    button.onClick.AddListener(() => DoQuest(itemQuest.Id, itemQuest.HyperLink));
                }
                
                Image[] images = newItem.GetComponentsInChildren<Image>();
                foreach (Image img in images)
                {

                    if (img.name == "icondone")
                    {
                        Debug.Log("okok");
                        if (itemQuest.IsDone)
                        {

                            button.gameObject.SetActive(false);
                            img.gameObject.SetActive(true);
                        }
                        else
                        {
                            button.gameObject.SetActive(true);
                            img.gameObject.SetActive(false);

                        }
                        break;
                    }
                   
                }

               

                //newItem.transform.Find("sheeptaskAmount").GetComponent<TextMeshProUGUI>().text ="X" + quest.SheepAmount.ToString("F2");

                // Đặt scale là 1 để không bị lỗi hiển thị                                              //ob.transform.position = pos;
            }
            UpdatePartContentHeight();

            List<Quest> questDialyData = UserDataManager.Instance.questData.Where(n => !n.IsDone &&  n.QuestType.Equals("DAILY")).ToList();
            Debug.Log("questDialyData " + questDialyData.Count);
            for (int i = 0; i < questDialyData.Count; i++)
            {
                GameObject newItem = Instantiate(itemPartnerPrefab);
                newItem.transform.SetParent(transformDialy.transform);
                newItem.transform.localScale = Vector3.one;
                ItemQuest itemQuest = newItem.GetComponent<ItemQuest>();
                Quest quest = questDialyData[i];
                itemQuest.Id = quest.Id;
                itemQuest.Name = quest.Name;
                itemQuest.Description = quest.Description;
                itemQuest.TonAmount = quest.TonAmount;
                itemQuest.SheepAmount = quest.SheepAmount;
                itemQuest.HyperLink = quest.HyperLink;
                itemQuest.QuestType = quest.QuestType;
                itemQuest.Active = quest.Active;
                itemQuest.Priority = quest.Priority;
                itemQuest.IsDoing = quest.IsDoing;
                itemQuest.IsDone = quest.IsDone;
                itemQuest.DoneAt = quest.DoneAt;

                TextMeshProUGUI[] texts = newItem.GetComponentsInChildren<TextMeshProUGUI>();

                foreach (TextMeshProUGUI txt in texts)
                {
                    if (txt.name == "nametask")
                    {
                        txt.text = quest.Name;

                    }
                    else if (txt.name == "tontaskparneramount")
                    {

                        txt.text = "X " + quest.TonAmount.ToString("F2");

                    }
                    else if (txt.name == "sheeptaskAmount")
                    {
                        txt.text = "X " + quest.SheepAmount.ToString("F2");

                    }
                }
                // Tìm phần tử Button bên trong prefab
                Button button = newItem.GetComponentInChildren<Button>();

                // Đảm bảo button không null
                if (button != null)
                {
                    // Gắn sự kiện với tham số cho button
                    button.onClick.AddListener(() => DoQuest(itemQuest.Id, itemQuest.HyperLink));
                }

                Image[] images = newItem.GetComponentsInChildren<Image>();
                foreach (Image img in images)
                {

                    if (img.name == "icondone")
                    {
                        
                        if (itemQuest.IsDone)
                        {

                            button.gameObject.SetActive(false);
                            img.gameObject.SetActive(true);
                        }
                        else
                        {
                            button.gameObject.SetActive(true);
                            img.gameObject.SetActive(false);

                        }
                        break;
                    }

                }
            }
            UpdateDailyContentHeight();

        });
       
    }
    void UpdatePartContentHeight()
    {
        RectTransform content = transformPartner.gameObject.GetComponent<RectTransform>();
        // Tính toán tổng chiều cao: Height của từng mục + khoảng cách (Spacing)
        float totalHeight = content.childCount * (itemPartnerPrefab.GetComponent<RectTransform>().sizeDelta.y + 10); // 10 là khoảng cách giữa các mục
        content.sizeDelta = new Vector2(content.sizeDelta.x, totalHeight);
    }
    void UpdateDailyContentHeight()
    {
        RectTransform content = transformDialy.gameObject.GetComponent<RectTransform>();
        // Tính toán tổng chiều cao: Height của từng mục + khoảng cách (Spacing)
        float totalHeight = content.childCount * (itemPartnerPrefab.GetComponent<RectTransform>().sizeDelta.y + 10); // 10 là khoảng cách giữa các mục
        content.sizeDelta = new Vector2(content.sizeDelta.x, totalHeight);
    }
    // Hàm xử lý có tham số
    void DoQuest(int index, string hyperLink)
    {
        if (!string.IsNullOrEmpty(hyperLink))
        {

            //#if UNITY_WEBGL && !UNITY_EDITOR
            OpenTelegramLink(hyperLink);
            //#endif
            GameManager.Instance.buyManager.DoQuest(index, () =>
            {
                GameManager.Instance.buyManager.SucessQuest(index, () =>
                {
                    GameManager.Instance.buyManager.LoadData(()=> {
                        foreach (Transform child in transformPartner.transform)
                        {
                            Destroy(child.gameObject);
                        }
                        foreach (Transform child in transformDialy.transform)
                        {
                            Destroy(child.gameObject);
                        }
                        CreateItemPartner();
                    });
                   

                });

            });

        }
        
       
    }
    public void OpenTelegramLink(string link)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
            openLinkTelegramFromUnity(link);
#else
        Debug.LogWarning("This function only works on WebGL build.");
#endif
    }

    // Hàm mở liên kết thông thường từ Unity WebGL
    public void OpenBrowserLink(string link)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
            openLinkFromUnity(link);
#else
        Application.OpenURL(link);
#endif
    }
    void ShowScrollView1()
    {
        // Xóa các hàng hiện tại

        scrollView1.SetActive(true);
        Tab1ImageBg.SetActive(true);

        Tab2ImageBg.SetActive(false);
        scrollView2.SetActive(false);
    }

    void ShowScrollView2()
    {

        Debug.Log("chon view 2");
        scrollView1.SetActive(false);
        Tab1ImageBg.SetActive(false);

        Tab2ImageBg.SetActive(true);
        scrollView2.SetActive(true);
    }
}
