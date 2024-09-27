using System.IO;
using UnityEditor;
using UnityEngine;

namespace Project_Data.Editor
{
    public class PlayerPrefsCleaner
    {
        [MenuItem("Tools/Delete Game Files")]
        public static void Clean()
        {
            PlayerPrefs.DeleteAll();
            
            for (int i = 0; i <= 5; i++)
            {
                string index = i == 0 ? "" : i + "";
                string filePath = Application.persistentDataPath + "/Managers" + index + ".dat";

                if (File.Exists(filePath))
                {
                    FileUtil.DeleteFileOrDirectory(filePath);
                }
            }

            for (int i = 0; i <= 5; i++)
            {
                string index = i == 0 ? "" : i + "";
                string bridgesFilePath = Application.persistentDataPath + "/Bridges" + index + ".dat";

                if (File.Exists(bridgesFilePath))
                {
                    FileUtil.DeleteFileOrDirectory(bridgesFilePath);
                }
            }

            Debug.Log("Player Data Clear");
        }
    }
}