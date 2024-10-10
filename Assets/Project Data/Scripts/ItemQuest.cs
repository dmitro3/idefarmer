using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;
public class ItemQuest : MonoBehaviour
{
    public TextMeshProUGUI nameText;
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


//    public void OpenLink()
//    {
       
//        if (!string.IsNullOrEmpty(HyperLink))
//        {

//#if UNITY_WEBGL && !UNITY_EDITOR
//                    if (HyperLink.Trim().Contains("t.me"))
//                    {

//                       Application.ExternalCall("openLinkTelegramFromUnity", HyperLink);

//                      }
//                    else
//                    {

//                     Application.ExternalCall("openLinkFromUnity", HyperLink);

//                    }
//#endif
                

//            //Call APi nếu có -> gọi api user/do-quest  isDoing = true gọi verify -» user / success-quest



//        }

//    }


}
