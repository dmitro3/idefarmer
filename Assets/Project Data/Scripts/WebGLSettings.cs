using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebGLSettings : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Kiểm tra nếu thiết bị là di động, giảm DPI để game chạy mượt mà hơn
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            float pixelRatio = Mathf.Min(Screen.width / 960.0f, Screen.height / 540.0f);
            pixelRatio = Mathf.Clamp(pixelRatio, 0.5f, 1.0f); // Giới hạn tỷ lệ DPI từ 0.5 đến 1
            //WebGLRenderingContext.SetResolution((int)(Screen.width * pixelRatio), (int)(Screen.height * pixelRatio), true);
        }
    }

}
