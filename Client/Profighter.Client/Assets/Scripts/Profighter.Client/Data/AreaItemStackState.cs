using System;
using UnityEngine;

namespace Profighter.Client.Data
{
    public class AreaItemStackState
    {
        public Guid Guid { get; set; }

        public ItemStackState ItemStackState { get; set; }

        public Vector3 ItemStackPosition { get; set; }
    }
}