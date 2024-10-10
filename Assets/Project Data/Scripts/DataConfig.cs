using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataConfig 
{
    
    public static string addressReceiver = "UQBYLNmEIjlB3ESpbUPP8t5mg0jv7oHeSXBb0Ldn29ptUQ0T";
    public static string linkref = "https://t.me/tonthesheep_bot/game?startapp=";
    public static string token = "";
    public static string API = "https://api.tonthesheep.com/api/"; //API lấy data
    public static string USER_DATA_METHOD = "user/user-data"; //API lấy data
    public static string FARM_BUY_METHOD = "user/active-sheep-farm"; //API lấy data
    public static string SHEPP_MANAGER_BUY_METHOD = "user/rent-shepherd"; //API lấy data
    public static string UPGRADE_FARM_BUY_METHOD = "user/upgrade-sheep-farm"; //API lấy data
    public static string UPGRADE_CONVEYOR_BUY_METHOD = "user/upgrade-conveyor"; //API lấy data
    public static string UPGRADE_TRUCK_BUY_METHOD = "user/upgrade-truck"; //API lấy data
    public static string GET_QUEST_METHOD = "user/quest"; //API lấy data
    public static string GET_EARN_REF_METHOD = "user/ref-earned"; //API lấy data
    public static string DO_QUEST_METHOD = "user/do-quest"; //API lấy data
    public static string SUCCESS_QUEST_METHOD = "user/sucess-quest"; //API lấy data
    public static string DRAW_HISTORY_METHOD = "user/withdrawal-histories"; //API lấy data lấy lịch sử rút
    public static string WITHDRAW_QUEST_METHOD = "user/withdraw-request"; //API lấy data

    //upgrade :
    //Ton Transfer payload:
    //format:
    //userId, objectId, index
    //với userId
    //objectId: trại cừu 1, băng chuyền 2, kho 3, chăn cừu: 4, active trại cừu : 5
    //index: trại cừu 1-5, băng chuyền 1, kho 1
    //upgradeFarmSheep upgradeTruck upgradeConveyor activeSheepFarm activeSheepFarm  rentShepherd
}
