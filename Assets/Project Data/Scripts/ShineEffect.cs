using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Project_Data.Scripts
{
    public class ShineEffect : MonoBehaviour {

        public GameObject shine;
        public bool shouldPlayEffect;
    
        void Start ()
        {
            StartCoroutine("startShineEffectCoroutine");
        }

        public void startShineEffect()
        {
            shouldPlayEffect = true;
        }

        public void stopShineEffect()
        {
            shouldPlayEffect = false;
        }

        IEnumerator startShineEffectCoroutine()
        {
            while (true)
            {
                if (shouldPlayEffect)
                {
                    Tween moveTween = shine.transform.DOLocalMoveX(-shine.transform.localPosition.x, 0.5f).SetEase(Ease.Linear);
                    yield return moveTween.WaitForCompletion();

                    shine.transform.DOLocalMoveX(-shine.transform.localPosition.x, 0.001f).SetEase(Ease.Linear);
                    yield return new WaitForSeconds(2f);
                }
                else
                {
                    yield return 0;
                }
            }
        }
    }
}