using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEffectController : MonoBehaviour
{
    public Button[] buttons;           // Mảng chứa các nút
    public Vector3 enlargedScale;      // Kích thước khi phóng to
    public Vector3 normalScale;        // Kích thước bình thường
    public Sprite[] normalSprites;     // Mảng hình ảnh mặc định cho các nút
    public Sprite[] selectedSprites;   // Mảng hình ảnh khi nút được chọn cho các nút
    void Start()
    {
        // Gán sự kiện cho từng nút
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;  // Lưu lại chỉ số của nút
            buttons[i].onClick.AddListener(() => OnButtonClick(index));
        }

        // Đặt hình ảnh mặc định ban đầu cho các nút
        ResetButtons();
    }
    void ResetButtons()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].transform.localScale = normalScale;  // Đặt kích thước mặc định
            buttons[i].GetComponent<Image>().sprite = normalSprites[i];  // Đặt hình ảnh mặc định
        }
        buttons[0].transform.DOScale(enlargedScale, 0.3f).SetEase(Ease.OutBack);
        buttons[0].GetComponent<Image>().sprite = selectedSprites[0];  // Đổi sang hình ảnh tương ứng khi được chọn
    }

    // Hàm gọi khi nút được nhấn
    // Hàm gọi khi một nút được nhấn

    void OnButtonClick(int clickedIndex)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i == clickedIndex)
            {
                // Nút được nhấn sẽ phóng to và đổi hình ảnh
                buttons[i].transform.DOScale(enlargedScale, 0.3f).SetEase(Ease.OutBack);
                buttons[i].GetComponent<Image>().sprite = selectedSprites[i];  // Đổi sang hình ảnh tương ứng khi được chọn
            }
            else
            {
                // Các nút còn lại sẽ thu nhỏ lại và đổi về hình ảnh mặc định của chúng
                buttons[i].transform.DOScale(normalScale, 0.3f).SetEase(Ease.OutBack);
                buttons[i].GetComponent<Image>().sprite = normalSprites[i];  // Đổi về hình ảnh mặc định tương ứng
            }
        }
    }
}
