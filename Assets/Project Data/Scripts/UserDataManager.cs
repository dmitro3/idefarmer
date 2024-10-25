using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using Unity.VisualScripting;
using System.Linq;

public class WithDrawHistoryData
{
    public int Total { get; set; }
    public List<DataItem> Data { get; set; }
}

public class DataItem
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string TokenId { get; set; }
    public double Amount { get; set; }
    public string Address { get; set; }
    public string Status { get; set; }
    public string TransactionHash { get; set; }
    public bool AutomationTransfer { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? SucceededAt { get; set; }
    public string QueryId { get; set; }
    public bool Processed { get; set; }
    public bool WasRecreated { get; set; }
    public long ProcessedAt { get; set; }
}

public class TotalData
{
    public string userName { get; set; }
    public double? TON { get; set; }
    public double? SHEEP { get; set; }
}

public class RefData
{
    public double? sheep { get; set; }
    public double? ton { get; set; }
    public List<TotalData> totalData { get; set; }
}
public class Quest
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double TonAmount { get; set; }
    public double SheepAmount { get; set; }
    public string HyperLink { get; set; }
    public string QuestType { get; set; }
    public bool Active { get; set; }
    public int Priority { get; set; }
    public bool IsDone { get; set; }
    public bool IsDoing { get; set; }
    public DateTime? DoneAt { get; set; }
}
public class User
{
    public string id { get; set; }
    public bool isStartedApp { get; set; }
    public string refId { get; set; }
    public string refCode { get; set; }
    public string userName { get; set; }
    public bool isBoughtCombo { get; set; }
}

public class UserBalance
{
    public double unprocessed { get; set; }
    public double lastUpdate { get; set; } // UNIX timestamp
    public double pending { get; set; }
    public double used { get; set; }
    public string token { get; set; }
}

public class UserTruck
{
    public string UserId { get; set; }                   // Mã người dùng
    public int Level { get; set; }                       // Cấp độ hiện tại
    public double UpgradePrice { get; set; }             // Giá nâng cấp
    public double Upgrade3LvPrice { get; set; }          // Giá nâng cấp cho cấp độ 3
    public double HashRate { get; set; }                 // Tốc độ băm hiện tại
    public double ROI { get; set; }                      // Tỷ suất lợi nhuận đầu tư
    public double HashRate1Lv { get; set; }              // Tốc độ băm cho cấp độ 1
    public double HashRate3Lv { get; set; }              // Tốc độ băm cho cấp độ 3
    public double HashRate5Lv { get; set; }              // Tốc độ băm cho cấp độ 5
    public double ToThisLvPrice { get; set; }            // Giá để đạt cấp độ hiện tại
    public double Upgrade5LvPrice { get; set; }          // Giá nâng cấp cho cấp độ 5
    public double SheepHashRate { get; set; }            // Tốc độ băm của sheep
    public double SheepHashRate1Lv { get; set; }         // Tốc độ băm sheep cấp 1
    public double SheepHashRate3Lv { get; set; }         // Tốc độ băm sheep cấp 3
    public double SheepHashRate5Lv { get; set; }         // Tốc độ băm sheep cấp 5
}

public class UserConveyor
{
    public string UserId { get; set; }                   // Mã người dùng
    public int Level { get; set; }                       // Cấp độ hiện tại
    public double UpgradePrice { get; set; }             // Giá nâng cấp
    public double HashRate { get; set; }                 // Tốc độ băm hiện tại
    public double ROI { get; set; }                      // Tỷ suất lợi nhuận đầu tư
    public double HashRate1Lv { get; set; }              // Tốc độ băm cho cấp độ 1
    public double HashRate3Lv { get; set; }              // Tốc độ băm cho cấp độ 3
    public double HashRate5Lv { get; set; }              // Tốc độ băm cho cấp độ 5
    public double ToThisLvPrice { get; set; }            // Giá để đạt cấp độ hiện tại
    public double Upgrade5LvPrice { get; set; }          // Giá nâng cấp cho cấp độ 5
    public double Upgrade3LvPrice { get; set; }          // Giá nâng cấp cho cấp độ 3
    public double SheepHashRate { get; set; }            // Tốc độ băm của sheep
    public double SheepCal1LvHashRate { get; set; }      // Tốc độ băm sheep cấp 1
    public double SheepHashRate3Lv { get; set; }         // Tốc độ băm sheep cấp 3
    public double SheepHashRate5Lv { get; set; }         // Tốc độ băm sheep cấp 5
    public double Balance { get; set; }         // Tốc độ băm sheep cấp 5
}
public class UserSheepFarm
{
    public string UserId { get; set; }                   // Mã người dùng
    public int SheepFarmId { get; set; }                 // Mã trang trại
    public int Level { get; set; }                       // Cấp độ hiện tại
    public bool Shepherd { get; set; }                   // Trạng thái người chăn cừu (true/false)
    public bool IsActive { get; set; }                   // Trạng thái kích hoạt (true/false)
    public double UpgradePrice { get; set; }             // Giá nâng cấp cấp độ hiện tại
    public double ActivePrice { get; set; }              // Giá kích hoạt
    public double ROI { get; set; }                      // Tỷ suất lợi nhuận đầu tư (Return on Investment)
    public double Upgrade3LvPrice { get; set; }          // Giá nâng cấp lên cấp độ 3
    public double Upgrade5LvPrice { get; set; }          // Giá nâng cấp lên cấp độ 5
    public double ToThisLvPrice { get; set; }            // Giá để đạt đến cấp độ hiện tại
    public double HashRate { get; set; }                 // Tốc độ băm hiện tại
    public double HashRate1Lv { get; set; }              // Tốc độ băm cho cấp độ 1
    public double HashRate3Lv { get; set; }              // Tốc độ băm cho cấp độ 3
    public double HashRate5Lv { get; set; }              // Tốc độ băm cho cấp độ 5
    public double SheepHashRate { get; set; }            // Tốc độ băm của sheep hiện tại
    public double SheepHashRate1Lv { get; set; }         // Tốc độ băm của sheep cho cấp độ 1
    public double SheepHashRate3Lv { get; set; }         // Tốc độ băm của sheep cho cấp độ 3
    public double SheepHashRate5Lv { get; set; }         // Tốc độ băm của sheep cho cấp độ 5
    public double Balance { get; set; }         // Tốc độ băm của sheep cho cấp độ 5
    public double currentConveyorHashRate { get; set; }         // Tốc độ băm của sheep cho cấp độ 5
    
}
public class UserData
{
    public User user { get; set; }
    public List<UserBalance> userBalances { get; set; }
    public double userHashRate { get; set; }
    public double userSheepHashRate { get; set; }
    public List<UserSheepFarm> userSheepFarms { get; set; }
    public List<UserTruck> userTruck { get; set; }
    public List<UserConveyor> userConveyor { get; set; }
    public int countRef { get; set; }
    public long timeToEndSpecial { get; set; }
}
public class UserDataManager : MonoBehaviour
{
    public static UserDataManager Instance { get; private set; }
    public UserData UserData { get; set; }
    public List<Quest> questData;
    public RefData refData;
    public WithDrawHistoryData withDrawHistoryData;
    public string walletAddress = "";
    public bool isConnectWallet= false;
    public double balanceWalletAddress = 0;
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
    /// <summary>
    /// Lấy HashRate
    /// </summary>
    /// <returns></returns>
    public double GetuserHashRate()
    {

        try
        {
            return UserData.userHashRate;
        }
        catch {
            return 0;
        }
       
    }
    /// <summary>
    /// Lấy BalanceTon
    /// </summary>
    /// <returns></returns>
    public double GetBalanceTon()
    {
        double balanceTon = 0.0;
        try
        {
           UserBalance userBalance= UserData.userBalances[0];
           long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
           balanceTon = (currentTime - userBalance.lastUpdate ) / 1000 / 86400 * UserData.userHashRate + userBalance.unprocessed - userBalance.pending - userBalance.used;

            return balanceTon;
        }
        catch
        {
            return 0;
        }

    }
    public double GetBalanceAddSeepTon()
    {
        double balanceTon = 0.0;
        try
        {
            UserBalance userBalance = UserData.userBalances[1];
            long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            balanceTon = (currentTime - userBalance.lastUpdate) / 1000 / 86400 * UserData.userSheepHashRate + userBalance.unprocessed - userBalance.pending - userBalance.used;

            return balanceTon;
        }
        catch
        {
            return 0;
        }

    }

    /// <summary>
    /// Lấy Balance SheepCoin
    /// </summary>
    /// <returns></returns>
    public double GetBalanceSheepCoin()
    {
        double balanceTon = 0.0;
        try
        {
            UserBalance userBalance = UserData.userBalances[1];
            long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            balanceTon = (currentTime - userBalance.lastUpdate) / 1000 / 86400 * UserData.userSheepHashRate + userBalance.unprocessed - userBalance.pending - userBalance.used;

            return balanceTon;
        }
        catch
        {
            return 0;
        }

    }
    public int GetNumberSheepFarms()
    {
        return UserData.userSheepFarms.Where(n => n.IsActive).Count();
    }

    public List<UserSheepFarm> GetSheepFarms()
    {
        return UserData.userSheepFarms.Where(n => n.IsActive).OrderBy(n => n.SheepFarmId).ToList();
    }

    public UserSheepFarm  GetOneSheepFarm(int sheepFarmId)
    {
        // return UserData.userSheepFarms.Where(n => n.IsActive).Where(n => n.SheepFarmId == sheepFarmId).FirstOrDefault();
        List<UserSheepFarm> userSheepFarms = UserData.userSheepFarms; // Lưu tạm đối tượng

        // Truy vấn dữ liệu
        UserSheepFarm result = userSheepFarms.Where(n => n.IsActive).Where(n => n.SheepFarmId == sheepFarmId).FirstOrDefault();
        return result;
    }
    public UserConveyor GetOneUserConveyor()
    {
        // return UserData.userSheepFarms.Where(n => n.IsActive).Where(n => n.SheepFarmId == sheepFarmId).FirstOrDefault();
        List<UserConveyor> userConveyors = UserData.userConveyor; // Lưu tạm đối tượng

        // Truy vấn dữ liệu
        UserConveyor result = userConveyors[0];
        return result;
    }
    public UserTruck GetOneUserTruck()
    {
        // return UserData.userSheepFarms.Where(n => n.IsActive).Where(n => n.SheepFarmId == sheepFarmId).FirstOrDefault();
        List<UserTruck> userTrucks = UserData.userTruck; // Lưu tạm đối tượng

        // Truy vấn dữ liệu
        UserTruck result = userTrucks[0];
        return result;
    }
    public UserSheepFarm GetOneInAllSheepFarm(int sheepFarmId)
    {
        // return UserData.userSheepFarms.Where(n => n.IsActive).Where(n => n.SheepFarmId == sheepFarmId).FirstOrDefault();
        List<UserSheepFarm> userSheepFarms = UserData.userSheepFarms; // Lưu tạm đối tượng

        // Truy vấn dữ liệu
        UserSheepFarm result = userSheepFarms.Where(n => n.SheepFarmId == sheepFarmId).FirstOrDefault();
        return result;
    }
    //Get Active farm tiep theo
    public double UpgradefarmPrice()
    {
        try {

            //Lay chi sô index trai cừu cần mua tiếp theo
            int index = GetindexFarm();
            double price = 0;
            UserSheepFarm userSheepFarm = UserDataManager.Instance.GetOneInAllSheepFarm(index);
            if (userSheepFarm != null)
            {
                price = userSheepFarm.ActivePrice;
            }
            return price;

        }
        catch
        {

            return 0;
        }
    


    }

    public int GetindexFarm()
    {
        try
        {
          

            List<UserSheepFarm> userSheepFarms = UserData.userSheepFarms.Where(n => n.IsActive).OrderBy(n => n.SheepFarmId).ToList();

            int count = userSheepFarms.Count;

            if ((count + 1) <= 5)
            {

                return count + 1;
            }
            else
            {
                return 0;

            }

        }
        catch
        {

            return 0;
        }



    }

}
