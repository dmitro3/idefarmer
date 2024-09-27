using DG.Tweening;
using UnityEngine;

namespace Project_Data.Scripts
{
	public class CloudsFloating : MonoBehaviour {
	
		void Start () {

			Sequence seq = DOTween.Sequence();
			seq.Append(transform.DOScaleY(0.95f, 0.3f)).SetEase(Ease.Linear);
			seq.Append(transform.DOScaleY(1f, 0.35f)).SetEase(Ease.Linear);
			seq.AppendInterval(1f);
			seq.SetLoops(-1);
		}
	}
}
