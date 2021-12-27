using System;
using UnityEngine;

namespace Profighter.Client.PlayerInput
{
	public static class MultiPlatformInputManager
	{
		public enum ActiveInputMethod
		{
			Hardware,
			Touch
		}


		private static InputVirtual Active;

		private static InputVirtual STouch;
		private static InputVirtual SHardware;


		static MultiPlatformInputManager()
		{
			STouch = new InputMobile();
			SHardware = new InputStandalone();
#if MOBILE_INPUT
            Active = STouch;
#else
			activeInput = s_HardwareInput;
#endif
		}

		public static void SwitchActiveInputMethod(ActiveInputMethod activeInputMethod)
		{
			switch (activeInputMethod)
			{
				case ActiveInputMethod.Hardware:
					Active = SHardware;
					break;

				case ActiveInputMethod.Touch:
					Active = STouch;
					break;
			}
		}

		public static bool AxisExists(string name)
		{
			return Active.AxisExists(name);
		}

		public static bool ButtonExists(string name)
		{
			return Active.ButtonExists(name);
		}

		public static void RegisterVirtualAxis(VirtualAxis axis)
		{
			Active.RegisterVirtualAxis(axis);
		}


		public static void RegisterVirtualButton(VirtualButton button)
		{
			Active.RegisterVirtualButton(button);
		}


		public static void UnRegisterVirtualAxis(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			Active.UnRegisterVirtualAxis(name);
		}


		public static void UnRegisterVirtualButton(string name)
		{
			Active.UnRegisterVirtualButton(name);
		}


		// returns a reference to a named virtual axis if it exists otherwise null
		public static VirtualAxis VirtualAxisReference(string name)
		{
			return Active.VirtualAxisReference(name);
		}


		// returns the platform appropriate axis for the given name
		public static float GetAxis(string name)
		{
			return GetAxis(name, false);
		}


		public static float GetAxisRaw(string name)
		{
			return GetAxis(name, true);
		}


		// private function handles both types of axis (raw and not raw)
		private static float GetAxis(string name, bool raw)
		{
			return Active.GetAxis(name, raw);
		}


		// -- Button handling --
		public static bool GetButton(string name)
		{
			return Active.GetButton(name);
		}


		public static bool GetButtonDown(string name)
		{
			return Active.GetButtonDown(name);
		}


		public static bool GetButtonUp(string name)
		{
			return Active.GetButtonUp(name);
		}


		public static void SetButtonDown(string name)
		{
			Active.SetButtonDown(name);
		}


		public static void SetButtonUp(string name)
		{
			Active.SetButtonUp(name);
		}


		public static void SetAxisPositive(string name)
		{
			Active.SetAxisPositive(name);
		}


		public static void SetAxisNegative(string name)
		{
			Active.SetAxisNegative(name);
		}


		public static void SetAxisZero(string name)
		{
			Active.SetAxisZero(name);
		}


		public static void SetAxis(string name, float value)
		{
			Active.SetAxis(name, value);
		}


		public static Vector3 mousePosition
		{
			get { return Active.MousePosition(); }
		}


		public static void SetVirtualMousePositionX(float f)
		{
			Active.SetVirtualMousePositionX(f);
		}


		public static void SetVirtualMousePositionY(float f)
		{
			Active.SetVirtualMousePositionY(f);
		}


		public static void SetVirtualMousePositionZ(float f)
		{
			Active.SetVirtualMousePositionZ(f);
		}


		// virtual axis and button classes - applies to mobile input
		// Can be mapped to touch joysticks, tilt, gyro, etc, depending on desired implementation.
		// Could also be implemented by other input devices - kinect, electronic sensors, etc
		public class VirtualAxis
		{
			public string name { get; private set; }
			private float m_Value;
			public bool matchWithInputManager { get; private set; }


			public VirtualAxis(string name)
				: this(name, true)
			{
			}


			public VirtualAxis(string name, bool matchToInputSettings)
			{
				this.name = name;
				matchWithInputManager = matchToInputSettings;
			}


			// removes an axes from the cross platform input system
			public void Remove()
			{
				UnRegisterVirtualAxis(name);
			}


			// a controller gameobject (eg. a virtual thumbstick) should update this class
			public void Update(float value)
			{
				m_Value = value;
			}


			public float GetValue
			{
				get { return m_Value; }
			}


			public float GetValueRaw
			{
				get { return m_Value; }
			}
		}

		// a controller gameobject (eg. a virtual GUI button) should call the
		// 'pressed' function of this class. Other objects can then read the
		// Get/Down/Up state of this button.
		public class VirtualButton
		{
			public string name { get; private set; }
			public bool matchWithInputManager { get; private set; }

			private int m_LastPressedFrame = -5;
			private int m_ReleasedFrame = -5;
			private bool m_Pressed;


			public VirtualButton(string name)
				: this(name, true)
			{
			}


			public VirtualButton(string name, bool matchToInputSettings)
			{
				this.name = name;
				matchWithInputManager = matchToInputSettings;
			}


			// A controller gameobject should call this function when the button is pressed down
			public void Pressed()
			{
				if (m_Pressed)
				{
					return;
				}
				m_Pressed = true;
				m_LastPressedFrame = Time.frameCount;
			}


			// A controller gameobject should call this function when the button is released
			public void Released()
			{
				m_Pressed = false;
				m_ReleasedFrame = Time.frameCount;
			}


			// the controller gameobject should call Remove when the button is destroyed or disabled
			public void Remove()
			{
				UnRegisterVirtualButton(name);
			}


			// these are the states of the button which can be read via the cross platform input system
			public bool GetButton
			{
				get { return m_Pressed; }
			}


			public bool GetButtonDown
			{
				get
				{
					return m_LastPressedFrame - Time.frameCount == -1;
				}
			}


			public bool GetButtonUp
			{
				get
				{
					return (m_ReleasedFrame == Time.frameCount - 1);
				}
			}
		}
	}
}
