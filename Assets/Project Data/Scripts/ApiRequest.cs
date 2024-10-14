using Newtonsoft.Json;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Text;
// Định nghĩa lớp yêu cầu (Request)
[System.Serializable]
public class SheepFarmRequest
{
    public int sheepFarmId;
}
public class ApiRequest : MonoBehaviour
{

    private float targetProgress = 0f;  // Tiến độ mục tiêu
    private float smoothSpeed = 0.05f;  // Tốc độ làm mượt loading
    public Slider loadingBar;  // Slider UI element for the loading bar
    private string token = "";
    //private string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjQwOTI3MDk5MyIsImlhdCI6MTcyNjUwMzY3M30.spMPGwWZgMV8m9OJAdbVH1sZa2HHM2CfD39CGcUXRM4";
    // Start is called before the first frame update
    public static ApiRequest Instance { get; private set; }
    
    private void Awake()
    {
        // Ensure this is the only instance and don't destroy on load
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Make this object persist across scenes
        }
        else
        {
            Destroy(gameObject); // Prevent duplicate instances
        }
    }
    //private void Start()
    //{
    //    DataConfig.token = token;
    //    StartCoroutine(GetUserData(token));
    //}

    public void ReceiveParameter(string param = "")
    {
       // Debug.Log("token Received parameter from URL: " + param);

        if (!string.IsNullOrEmpty(param))
        {
            token = param;
            DataConfig.token = param;
            StartCoroutine(GetUserData(param));

        }
    }

    IEnumerator GetUserData(string token)
    {
        string apiUrl= DataConfig.API + DataConfig.USER_DATA_METHOD;

        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        request.SetRequestHeader("accept", "*/*");
        request.SetRequestHeader("Authorization", "Bearer " + token);

        // Bắt đầu gửi yêu cầu và chờ kết quả
        request.SendWebRequest();

        // Cập nhật tiến độ loading trong khi chờ phản hồi từ API
        while (!request.isDone)
        {
            // Tiến độ mục tiêu từ API
            targetProgress = Mathf.Clamp01(request.downloadProgress);

            // Làm mượt quá trình chuyển giá trị
            while (loadingBar.value < targetProgress)
            {
                loadingBar.value = Mathf.Lerp(loadingBar.value, targetProgress, smoothSpeed);  // Cập nhật mượt mà
                yield return null;  // Đợi một khung hình để tiếp tục cập nhật
            }

            yield return null;  // Đợi một khung hình để tiếp tục kiểm tra tiến độ
        }

       // Debug.Log("API Response: ");
        // Khi API hoàn tất, đảm bảo thanh loading đạt 100%
        targetProgress = 1f;

        while (Mathf.Abs(loadingBar.value - targetProgress) > 0.001f)
        {
            loadingBar.value = Mathf.Lerp(loadingBar.value, targetProgress, smoothSpeed);  // Làm mượt đến 100%
            yield return null;  // Chờ một khung hình trước khi tiếp tục
        }

       // Debug.Log("API Response tai xong : ");
        // Khi dữ liệu đã tải xong
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("API Error: " + request.error);
        }
        else
        {
            string responseText = request.downloadHandler.text;
            //Debug.Log(responseText);
            UserData userData = JsonConvert.DeserializeObject<UserData>(responseText);
            //Debug.Log(userData);
            // Save the response data into UserDataManager
            UserDataManager.Instance.UserData = userData;
            // Bạn có thể xử lý dữ liệu API tại đây và chuyển qua màn hình tiếp theo, ví dụ:
            yield return new WaitForSeconds(0.5f);  // Tạm dừng một chút để hiển thị 100%
            SceneManager.LoadScene(1);
        }
        
    }
    // Hàm để chuyển sang màn hình tiếp theo sau khi hoàn tất
    void LoadNextScene()
    {
        // Ví dụ chuyển sang scene tiếp theo với index 1 trong Build Settings
        SceneManager.LoadScene(1);
    }

    public void BuyFarm()
    {
        //Kiem tra vi ton
        //call api mua farm
        string apiUrl = DataConfig.API + DataConfig.FARM_BUY_METHOD;
        StartCoroutine(PostRequest(apiUrl, new SheepFarmRequest { sheepFarmId = 1 }));
        //cap nhat 
    }
    IEnumerator PostRequest(string url, SheepFarmRequest requestData)
    {
        // Chuyển đối tượng thành JSON và in ra để kiểm tra
        string jsonData = JsonUtility.ToJson(requestData);
        //Debug.Log("Sending POST request to: " + url);
        //Debug.Log("Request Body: " + jsonData);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {

            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("accept", "*/*");
            request.SetRequestHeader("Authorization", "Bearer " + token);
            request.SetRequestHeader("Content-Type", "application/json");
            
            // Gửi request và chờ phản hồi
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                Debug.Log("Response: " + request.downloadHandler.text);
            }
        }
    }
}
