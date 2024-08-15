/*
 * This script is from the Unity 5 Standard Assets with edits from Devin Curry
 * Search for changes tagged with the //DCURRY comment
 * Watch the tutorial here: www.Devination.com
 */
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityStandardAssets.CrossPlatformInput
{
	public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
	{
		public enum AxisOption
		{
			// Options for which axes to use
			Both, // Use both
			OnlyHorizontal, // Only horizontal
			OnlyVertical // Only vertical
		}

		public bool snapToFinger = false;
		public int MovementRange = 100;
		public AxisOption axesToUse = AxisOption.Both; // The options for the axes that the still will use
		public string horizontalAxisName = "Horizontal"; // The name given to the horizontal axis for the cross platform input
		public string verticalAxisName = "Vertical"; // The name given to the vertical axis for the cross platform input

		Vector3 m_StartPos;
		bool m_UseX; // Toggle for using the x axis
		bool m_UseY; // Toggle for using the Y axis
		CrossPlatformInputManager.VirtualAxis m_HorizontalVirtualAxis; // Reference to the joystick in the cross platform input
		CrossPlatformInputManager.VirtualAxis m_VerticalVirtualAxis; // Reference to the joystick in the cross platform input

		bool firstTouch = true;

		void Start()
        {
			if (transform.name == "Transparent")
				GetComponent<RectTransform>().sizeDelta = new Vector2(275, 275);
			else GetComponent<RectTransform>().sizeDelta = new Vector2(200, 200);
		}

		void OnEnable()
        {
			if (firstTouch == false)
			{
				CreateVirtualAxes();
				transform.position = m_StartPos;
				UpdateVirtualAxes(m_StartPos);
			}
			else
            {
				CreateVirtualAxes();
				UpdateVirtualAxes(m_StartPos);
			}
        }

		void Initialize() //DCURRY: Changed to Start from OnEnable
		{
			m_StartPos = transform.position;
			GameObject obj = new GameObject();
			obj.transform.parent = transform.parent;
			obj.transform.localPosition = new Vector3(36, 0, 0);

			MovementRange = (int)Vector2.Distance(transform.position, obj.transform.position);

			Destroy(obj);

			CreateVirtualAxes();
		}

		void UpdateVirtualAxes(Vector3 value)
		{
			var delta = m_StartPos - value;
			delta.y = -delta.y;
			delta /= MovementRange;
			if (m_UseX)
			{
				m_HorizontalVirtualAxis.Update(-delta.x);
			}

			if (m_UseY)
			{
				m_VerticalVirtualAxis.Update(delta.y);
			}
		}

		void CreateVirtualAxes()
		{
			// set axes to use
			m_UseX = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyHorizontal);
			m_UseY = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyVertical);

			// create new axes based on axes to use
			if (m_UseX)
			{
				m_HorizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(horizontalAxisName);
				CrossPlatformInputManager.RegisterVirtualAxis(m_HorizontalVirtualAxis);
			}
			if (m_UseY)
			{
				m_VerticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(verticalAxisName);
				CrossPlatformInputManager.RegisterVirtualAxis(m_VerticalVirtualAxis);
			}
		}


		public void OnDrag(PointerEventData data)
		{
			Vector3 newPos = Vector3.zero;

			if (m_UseX)
			{
				int delta = (int)(data.position.x - m_StartPos.x);
				//delta = Mathf.Clamp(delta, - MovementRange, MovementRange); //DCURRY: Dont want to clamp individual axis
				newPos.x = delta;
			}

			if (m_UseY)
			{
				int delta = (int)(data.position.y - m_StartPos.y);
				//delta = Mathf.Clamp(delta, -MovementRange, MovementRange); //DCURRY: Dont want to clamp individual axis
				newPos.y = delta;
			}
			//DCURRY: ClampMagnitude to clamp in a circle instead of a square
			transform.position = Vector3.ClampMagnitude(new Vector3(newPos.x, newPos.y, newPos.z), MovementRange) + m_StartPos;
			UpdateVirtualAxes(transform.position);
		}


		public void OnPointerUp(PointerEventData data)
		{
			transform.position = m_StartPos;
			UpdateVirtualAxes(m_StartPos);
		}

		public void OnPointerDown(PointerEventData data)
		{
			if(firstTouch)
            {
				firstTouch = false;
				Initialize();
            }
			if (snapToFinger)
			{
				transform.position = Vector3.ClampMagnitude((Vector3)data.position - m_StartPos, MovementRange) + m_StartPos;
				UpdateVirtualAxes(transform.position);
			}
		}

		/*void OnDisable()
		{

			// remove the joysticks from the cross platform input
			if (m_UseX)
			{
				m_HorizontalVirtualAxis.Remove();
			}
			if (m_UseY)
			{
				m_VerticalVirtualAxis.Remove();
			}
		}*/
	}
}