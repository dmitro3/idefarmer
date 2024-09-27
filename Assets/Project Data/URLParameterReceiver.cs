using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class URLParameterReceiver : MonoBehaviour
{
    private string apiUrl = DataConfig.API;
    private  string token;
    public void ReceiveParameter(string param="")
    {
        Debug.Log("token Received parameter from URL: " + param);
        token = param;

        if (!string.IsNullOrEmpty(token))
        {

        }

        // Bạn có thể xử lý tham số ở đây, như lưu vào biến, hoặc sử dụng trong game logic
    }
}
