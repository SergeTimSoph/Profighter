using System.Collections.Generic;
using UnityEngine;

namespace Profighter.Client.PlayerInput
{
    public abstract class InputVirtual
    {
        public Vector3 virtualMousePosition { get; private set; }
        
        
        protected Dictionary<string, MultiPlatformInputManager.VirtualAxis> m_VirtualAxes =
            new Dictionary<string, MultiPlatformInputManager.VirtualAxis>();
            // Dictionary to store the name relating to the virtual axes
        protected Dictionary<string, MultiPlatformInputManager.VirtualButton> m_VirtualButtons =
            new Dictionary<string, MultiPlatformInputManager.VirtualButton>();
        protected List<string> m_AlwaysUseVirtual = new List<string>();
            // list of the axis and button names that have been flagged to always use a virtual axis or button
        

        public bool AxisExists(string name)
        {
            return m_VirtualAxes.ContainsKey(name);
        }

        public bool ButtonExists(string name)
        {
            return m_VirtualButtons.ContainsKey(name);
        }


        public void RegisterVirtualAxis(MultiPlatformInputManager.VirtualAxis axis)
        {
            // check if we already have an axis with that name and log and error if we do
            if (m_VirtualAxes.ContainsKey(axis.name))
            {
                Debug.LogError("There is already a virtual axis named " + axis.name + " registered.");
            }
            else
            {
                // add any new axes
                m_VirtualAxes.Add(axis.name, axis);

                // if we dont want to match with the input manager setting then revert to always using virtual
                if (!axis.matchWithInputManager)
                {
                    m_AlwaysUseVirtual.Add(axis.name);
                }
            }
        }


        public void RegisterVirtualButton(MultiPlatformInputManager.VirtualButton button)
        {
            // check if already have a buttin with that name and log an error if we do
            if (m_VirtualButtons.ContainsKey(button.name))
            {
                Debug.LogError("There is already a virtual button named " + button.name + " registered.");
            }
            else
            {
                // add any new buttons
                m_VirtualButtons.Add(button.name, button);

                // if we dont want to match to the input manager then always use a virtual axis
                if (!button.matchWithInputManager)
                {
                    m_AlwaysUseVirtual.Add(button.name);
                }
            }
        }


        public void UnRegisterVirtualAxis(string name)
        {
            // if we have an axis with that name then remove it from our dictionary of registered axes
            if (m_VirtualAxes.ContainsKey(name))
            {
                m_VirtualAxes.Remove(name);
            }
        }


        public void UnRegisterVirtualButton(string name)
        {
            // if we have a button with this name then remove it from our dictionary of registered buttons
            if (m_VirtualButtons.ContainsKey(name))
            {
                m_VirtualButtons.Remove(name);
            }
        }


        // returns a reference to a named virtual axis if it exists otherwise null
        public MultiPlatformInputManager.VirtualAxis VirtualAxisReference(string name)
        {
            return m_VirtualAxes.ContainsKey(name) ? m_VirtualAxes[name] : null;
        }


        public void SetVirtualMousePositionX(float f)
        {
            virtualMousePosition = new Vector3(f, virtualMousePosition.y, virtualMousePosition.z);
        }


        public void SetVirtualMousePositionY(float f)
        {
            virtualMousePosition = new Vector3(virtualMousePosition.x, f, virtualMousePosition.z);
        }


        public void SetVirtualMousePositionZ(float f)
        {
            virtualMousePosition = new Vector3(virtualMousePosition.x, virtualMousePosition.y, f);
        }


        public abstract float GetAxis(string name, bool raw);
        
        public abstract bool GetButton(string name);
        public abstract bool GetButtonDown(string name);
        public abstract bool GetButtonUp(string name);

        public abstract void SetButtonDown(string name);
        public abstract void SetButtonUp(string name);
        public abstract void SetAxisPositive(string name);
        public abstract void SetAxisNegative(string name);
        public abstract void SetAxisZero(string name);
        public abstract void SetAxis(string name, float value);
        public abstract Vector3 MousePosition();
    }
}