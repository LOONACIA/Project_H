using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LOONACIA.Unity.UI
{
	public class UIEventHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
	{
		public Action<PointerEventData> Clicked;

		public Action<PointerEventData> PointerEntered;

		public Action<PointerEventData> PointerExited;

		public void OnPointerClick(PointerEventData eventData)
		{
			Clicked?.Invoke(eventData);
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			PointerEntered?.Invoke(eventData);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			PointerExited?.Invoke(eventData);
		}
	}
}