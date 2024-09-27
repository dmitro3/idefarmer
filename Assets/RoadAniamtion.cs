using Project_Data.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoadAniamtion : MonoBehaviour
{
    public Sprite[] fireAnimSprites;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("managerAnimation");

    }

    public IEnumerator managerAnimation()
    {
        while (true)
        {
            for (int j = 0; j < fireAnimSprites.Length; j++)
            {
                
                this.GetComponent<Image>().overrideSprite = fireAnimSprites[j];
                yield return new WaitForSeconds(0.1f);
            }

            
        }
    }
}
