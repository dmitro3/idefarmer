using Newtonsoft.Json;
using Project_Data.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
//using TonSdk.Connect;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ResponeMessage
{
    public  string message { get; set; }
    public string error { get; set; }
    public int statusCode { get; set; }
}
[System.Serializable]
public class RequestData
{
    public string address;
    public string token;
    public string amount;
}
public class BuyManager : MonoBehaviour
{
    public GameObject toastPanel;
    private string token = DataConfig.token;

    public void LoadData(Action onComplete = null)
    {
        //Cap nhat du lieu
        //UserDataManager.Instance.UserData = null;
        StartCoroutine(GetUserData(onComplete));

    }
    public void LoadHistory(int index, int limit,Action onComplete = null)
    {
        StartCoroutine(GetHistory(index,limit,onComplete));

    }


    #region DrawCoin

    public void WithDrawTonCoin(string address, string idtoken, string amout,Action onComplete = null)
    {
        string apiUrl = DataConfig.API + DataConfig.WITHDRAW_QUEST_METHOD;
        StartCoroutine(PostWithDrawRequest(apiUrl, address,idtoken,amout, onComplete));

    }

    #endregion
    public void DoQuest(int questId, Action onComplete = null)
    {
        string apiUrl = DataConfig.API + DataConfig.DO_QUEST_METHOD;
        StartCoroutine(PostDoQuestRequest(apiUrl, questId, onComplete));

    }
    public void SucessQuest(int questId, Action onComplete = null)
    {
        string apiUrl = DataConfig.API + DataConfig.SUCCESS_QUEST_METHOD;
        StartCoroutine(PostDoQuestRequest(apiUrl, questId, onComplete));

    }

    public void GetEarnRef(Action onComplete = null)
    {
        string apiUrl = DataConfig.API + DataConfig.GET_EARN_REF_METHOD;
        StartCoroutine(GetEarnRefRequest(apiUrl, onComplete));

    }

    public void GetQuest(Action onComplete = null)
    {
        string apiUrl = DataConfig.API + DataConfig.GET_QUEST_METHOD;
        StartCoroutine(GetQuestRequest(apiUrl, onComplete));

    }
    public void BuyFarm(int sheepFarmId,Action onComplete = null)
    {
        string apiUrl = DataConfig.API + DataConfig.FARM_BUY_METHOD;
        StartCoroutine(PostRequest(apiUrl, sheepFarmId, onComplete));

    }
    public void UpgrapeFarm(int sheepFarmId, Action onComplete = null)
    {
        Debug.Log("UpgrapeFarm sheepFarmId " + sheepFarmId);
        string apiUrl = DataConfig.API + DataConfig.UPGRADE_FARM_BUY_METHOD;
        StartCoroutine(PostRequest(apiUrl, sheepFarmId, onComplete));

    }

    public void UpgrapeuConveyor(int sheepFarmId, Action onComplete = null)
    {
        Debug.Log("UpgrapeuConveyor sheepFarmId " + sheepFarmId);
        string apiUrl = DataConfig.API + DataConfig.UPGRADE_CONVEYOR_BUY_METHOD;
        StartCoroutine(PostRequest(apiUrl, sheepFarmId, onComplete));
       
    }
    public void UpgrapeTruck(int sheepFarmId, Action onComplete = null)
    {
        Debug.Log("UpgrapeuTruck sheepFarmId " + sheepFarmId);
        string apiUrl = DataConfig.API + DataConfig.UPGRADE_TRUCK_BUY_METHOD;
        StartCoroutine(PostRequest(apiUrl, sheepFarmId, onComplete));

    }
    IEnumerator PostWithDrawRequest(string apiUrl, string address,string idtoken, string amount, Action onComplete = null)
    {
        RequestData requestData = new RequestData
        {
            address = address,
            token = idtoken.ToString(),
            amount = amount.ToString()
        };

        // Chuyển đổi đối tượng thành chuỗi JSON
        string jsonData = JsonUtility.ToJson(requestData);

        // Khởi tạo request với phương thức POST
        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            // Chuyển nội dung JSON thành byte và thêm vào request
            byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            // Thêm header cho request
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("accept", "*/*");
            request.SetRequestHeader("Authorization", "Bearer " + token); // Thêm Authorization header

            // Gửi request và chờ phản hồi
            yield return request.SendWebRequest();

            // Kiểm tra kết quả phản hồi
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Response: " + request.downloadHandler.text);
                Debug.Log("HTTP Response Code: " + request.responseCode);
                //Call 1 cái veryfi
                onComplete?.Invoke();
            }
            else
            {
                // In ra lỗi nếu request thất bại
                Debug.Log("Error: " + request.downloadHandler.text);

                // In ra message từ nội dung phản hồi nếu có
                if (request.downloadHandler != null)
                {
                    ResponeMessage refData = JsonConvert.DeserializeObject<ResponeMessage>(request.downloadHandler.text);
                    GameManager.Instance.ShowFailToast(refData.message);
                }

                // In ra HTTP response code để kiểm tra chi tiết hơn
                Debug.Log("HTTP Response Code: " + request.responseCode);
            }
        }
    }
    public void BuyManagerSheep(int sheepFarmId, Action onComplete = null)
    {
        Debug.Log("BuyManagerSheep sheepFarmId "+ sheepFarmId);

        //Kiểm tra ví Ton
        //Call Api tru
        string apiUrl = DataConfig.API + DataConfig.SHEPP_MANAGER_BUY_METHOD;
        StartCoroutine(PostRequest(apiUrl, sheepFarmId, onComplete));
        //Goi lại userdata

    }
    IEnumerator GetHistory(int page,int limit,Action onComplete = null)
    {
        string apiUrl =DataConfig.API + DataConfig.DRAW_HISTORY_METHOD+ "?page="+page+"&limit="+limit;

        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        request.SetRequestHeader("accept", "*/*");
        request.SetRequestHeader("Authorization", "Bearer " + token);
      
        // Gửi request và chờ phản hồi
        yield return request.SendWebRequest();

        // Kiểm tra kết quả phản hồi
        if (request.result == UnityWebRequest.Result.Success)
        {

            string responseText = request.downloadHandler.text;
            //Debug.Log("Response history: " + responseText);

            UserDataManager.Instance.withDrawHistoryData= JsonConvert.DeserializeObject<WithDrawHistoryData>(responseText);
         
             onComplete?.Invoke();

        }
        else
        {
            Debug.Log("Error: " + request.error);
        }

        request.Dispose();



    }
    IEnumerator GetUserData(Action onComplete = null)
    {
        string apiUrl = DataConfig.API + DataConfig.USER_DATA_METHOD;

        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        request.SetRequestHeader("accept", "*/*");
        request.SetRequestHeader("Authorization", "Bearer " + token);

        // Gửi request và chờ phản hồi
        yield return request.SendWebRequest();

        // Kiểm tra kết quả phản hồi
        if (request.result == UnityWebRequest.Result.Success)
        {

            string responseText = request.downloadHandler.text;
            Debug.Log("Response: " + responseText);
            UserData userData = JsonConvert.DeserializeObject<UserData>(responseText);
            Debug.Log(userData);
            // Save the response data into UserDataManager
            UserDataManager.Instance.UserData = userData;
            //Load lai cac thong so
            GameManager.Instance.ShowBalance();
            onComplete?.Invoke();

        }
        else
        {
            Debug.Log("Error: " + request.error);
        }

        request.Dispose();



    }
    IEnumerator PostRequest(string apiUrl, int sheepFarmId, Action onComplete = null)
    {
        // Tạo nội dung JSON cần gửi
        string jsonData = "{\"sheepFarmId\": " + sheepFarmId + "}";

        // Khởi tạo request với phương thức POST
        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            // Chuyển nội dung JSON thành byte và thêm vào request
            byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            // Thêm header cho request
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("accept", "*/*");
            request.SetRequestHeader("Authorization", "Bearer " + token); // Thêm Authorization header

            // Gửi request và chờ phản hồi
            yield return request.SendWebRequest();

            // Kiểm tra kết quả phản hồi
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Response: " + request.downloadHandler.text);
                
                Debug.Log("HTTP Response Code: " + request.responseCode);
                LoadData(onComplete);
               
            }
            else
            {
                // In ra lỗi nếu request thất bại
                Debug.Log("Error: " + request.error);

                // In ra message từ nội dung phản hồi nếu có
                if (request.downloadHandler != null)
                {
                    ResponeMessage refData = JsonConvert.DeserializeObject<ResponeMessage>(request.downloadHandler.text);
                    GameManager.Instance.ShowToast(refData.message);
                    Debug.Log("Message: " + request.downloadHandler.text);

                }

                // In ra HTTP response code để kiểm tra chi tiết hơn
                Debug.Log("HTTP Response Code: " + request.responseCode);
            }
        }
    }
    IEnumerator GetQuestRequest(string apiUrl, Action onComplete = null)
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        request.SetRequestHeader("accept", "*/*");
        request.SetRequestHeader("Authorization", "Bearer " + token);

        // Gửi request và chờ phản hồi
        yield return request.SendWebRequest();

        // Kiểm tra kết quả phản hồi
        if (request.result == UnityWebRequest.Result.Success)
        {

            string responseText = request.downloadHandler.text;
            Debug.Log("Response task: " + responseText);
            List<Quest> questData = JsonConvert.DeserializeObject<List<Quest>>(responseText);
            UserDataManager.Instance.questData = questData;
            onComplete?.Invoke();

        }
        else
        {
            Debug.Log("Error: " + request.error);
        }

        request.Dispose();
    }
    IEnumerator GetEarnRefRequest(string apiUrl, Action onComplete = null)
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        request.SetRequestHeader("accept", "*/*");
        request.SetRequestHeader("Authorization", "Bearer " + token);

        // Gửi request và chờ phản hồi
        yield return request.SendWebRequest();

        // Kiểm tra kết quả phản hồi
        if (request.result == UnityWebRequest.Result.Success)
        {

            string responseText = request.downloadHandler.text;
            Debug.Log("Response GetEarnRefRequest: " + responseText);
            RefData refData = JsonConvert.DeserializeObject<RefData>(responseText);
            UserDataManager.Instance.refData = refData;
            onComplete?.Invoke();

        }
        else
        {
            Debug.Log("Error: " + request.error);
        }

        request.Dispose();
    }
    IEnumerator PostSuccessQuestRequest(string apiUrl, int questId, Action onComplete = null)
    {
        // Tạo nội dung JSON cần gửi
        string jsonData = "{\"questId\": " + questId + "}";

        // Khởi tạo request với phương thức POST
        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            // Chuyển nội dung JSON thành byte và thêm vào request
            byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            // Thêm header cho request
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("accept", "*/*");
            request.SetRequestHeader("Authorization", "Bearer " + token); // Thêm Authorization header

            // Gửi request và chờ phản hồi
            yield return request.SendWebRequest();

            // Kiểm tra kết quả phản hồi
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Response: " + request.downloadHandler.text);
                Debug.Log("HTTP Response Code: " + request.responseCode);
                //Call 1 cái veryfi

            }
            else
            {
                // In ra lỗi nếu request thất bại
                Debug.Log("Error: " + request.error);

                // In ra message từ nội dung phản hồi nếu có
                if (request.downloadHandler != null)
                {
                    ResponeMessage refData = JsonConvert.DeserializeObject<ResponeMessage>(request.downloadHandler.text);
                    GameManager.Instance.ShowToast(refData.message);
                }

                // In ra HTTP response code để kiểm tra chi tiết hơn
                Debug.Log("HTTP Response Code: " + request.responseCode);
            }
        }
    }
    IEnumerator PostDoQuestRequest(string apiUrl, int questId, Action onComplete = null)
    {
        // Tạo nội dung JSON cần gửi
        string jsonData = "{\"questId\": " + questId + "}";

        // Khởi tạo request với phương thức POST
        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            // Chuyển nội dung JSON thành byte và thêm vào request
            byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            // Thêm header cho request
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("accept", "*/*");
            request.SetRequestHeader("Authorization", "Bearer " + token); // Thêm Authorization header

            // Gửi request và chờ phản hồi
            yield return request.SendWebRequest();

            // Kiểm tra kết quả phản hồi
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Response: " + request.downloadHandler.text);
                Debug.Log("HTTP Response Code: " + request.responseCode);
                //Call 1 cái veryfi
                onComplete?.Invoke();
            }
            else
            {
                // In ra lỗi nếu request thất bại
                Debug.Log("Error: " + request.error);

                // In ra message từ nội dung phản hồi nếu có
                if (request.downloadHandler != null)
                {
                    ResponeMessage refData = JsonConvert.DeserializeObject<ResponeMessage>(request.downloadHandler.text);
                    GameManager.Instance.ShowToast(refData.message);
                }

                // In ra HTTP response code để kiểm tra chi tiết hơn
                Debug.Log("HTTP Response Code: " + request.responseCode);
            }
        }
    }
}
