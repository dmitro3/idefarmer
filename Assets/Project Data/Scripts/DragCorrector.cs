using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Project_Data.Scripts
{
    public class DragCorrector : MonoBehaviour {

        public Text printText;
        public int baseTH = 6;
        public int basePPI = 210;
        public int dragTH = 0;

        void Start()
        {
            dragTH = baseTH * (int)Screen.dpi / basePPI;

            EventSystem es = GetComponent<EventSystem>();

            if (es) es.pixelDragThreshold = dragTH;
        
            printText.text = dragTH + "";
            printText.gameObject.SetActive(false);
        }
    }
}