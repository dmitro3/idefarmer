using DG.Tweening;
using UnityEngine;

namespace Project_Data.Scripts
{
	public class HandAnimation : MonoBehaviour {
        
		void Start ()
		{
			Sequence seq = DOTween.Sequence();
			seq.Append(transform.DOLocalMoveY(transform.localPosition.y - 20f, 0.5f)).SetEase(Ease.Linear);
			seq.Append(transform.DOLocalMoveY(transform.localPosition.y, 0.5f)).SetEase(Ease.Linear);
			seq.SetLoops(-1);
		}
	}
}