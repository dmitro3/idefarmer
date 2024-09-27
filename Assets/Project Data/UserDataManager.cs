using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Generic;

public class User
{
    public string id { get; set; }
    public bool isStartedApp { get; set; }
    public string refId { get; set; }
    public string refCode { get; set; }
}

public class UserBalance
{
    public int unprocessed { get; set; }
    public long lastUpdate { get; set; } // UNIX timestamp
    public int pending { get; set; }
    public int used { get; set; }
    public string token { get; set; }
}

public class UserTruck
{
    public string userId { get; set; }
    public int level { get; set; }
    public double upgradePrice { get; set; }
}

public class UserConveyor
{
    public string userId { get; set; }
    public int level { get; set; }
    public double upgradePrice { get; set; }
}
public class UserData
{
    public User user { get; set; }
    public List<UserBalance> userBalances { get; set; }
    public int userHashRate { get; set; }
    public List<object> userSheepFarms { get; set; }
    public List<UserTruck> userTruck { get; set; }
    public List<UserConveyor> userConveyor { get; set; }
    public int countRef { get; set; }
}
public class UserDataManager : MonoBehaviour
{
    public static UserDataManager Instance { get; private set; }
    public UserData UserData;

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
}
