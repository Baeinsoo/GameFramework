using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EntityCreatorRegistrationAttribute : Attribute
    {
        public bool value { get; }

        public EntityCreatorRegistrationAttribute(bool value = true)
        {
            this.value = value;
        }
    }
}
