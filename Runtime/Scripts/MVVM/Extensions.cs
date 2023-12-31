using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GameFramework
{
    public static partial class Extensions
    {
        private static bool SetProperty<T>(this object self, ref T field, T value, PropertyChangedEventHandler handler, EqualityComparer<T> equalityComparer = null, [CallerMemberName] string propertyName = null)
        {
            if ((equalityComparer ?? EqualityComparer<T>.Default).Equals(field, value))
            {
                return false;
            }

            field = value;
            handler?.Invoke(self, new PropertyChangedEventArgs(propertyName));

            return true;
        }

        public static bool SetProperty<T>(this object self, ref T field, T value, PropertyChangedEventHandler handler, [CallerMemberName] string propertyName = null)
        {
            return SetProperty(self, ref field, value, handler, null, propertyName);
        }

        public static bool SetProperty(this object self, ref Vector2 field, Vector2 value, PropertyChangedEventHandler handler, [CallerMemberName] string propertyName = null)
        {
            return SetProperty(self, ref field, value, handler, Vector2EqualityComparer.instance, propertyName);
        }

        public static bool SetProperty(this object self, ref Vector3 field, Vector3 value, PropertyChangedEventHandler handler, [CallerMemberName] string propertyName = null)
        {
            return SetProperty(self, ref field, value, handler, Vector3EqualityComparer.instance, propertyName);
        }

        public static bool SetProperty(this object self, ref Vector4 field, Vector4 value, PropertyChangedEventHandler handler, [CallerMemberName] string propertyName = null)
        {
            return SetProperty(self, ref field, value, handler, Vector4EqualityComparer.instance, propertyName);
        }
    }

    public class Vector2EqualityComparer : EqualityComparer<Vector2>
    {
        public static Vector2EqualityComparer instance = new Vector2EqualityComparer();

        public override bool Equals(Vector2 x, Vector2 y)
        {
            return x == y;
        }

        public override int GetHashCode(Vector2 obj)
        {
            return obj.GetHashCode();
        }
    }

    public class Vector3EqualityComparer : EqualityComparer<Vector3>
    {
        public static Vector3EqualityComparer instance = new Vector3EqualityComparer();

        public override bool Equals(Vector3 x, Vector3 y)
        {
            return x == y;
        }

        public override int GetHashCode(Vector3 obj)
        {
            return obj.GetHashCode();
        }
    }

    public class Vector4EqualityComparer : EqualityComparer<Vector4>
    {
        public static Vector4EqualityComparer instance = new Vector4EqualityComparer();

        public override bool Equals(Vector4 x, Vector4 y)
        {
            return x == y;
        }

        public override int GetHashCode(Vector4 obj)
        {
            return obj.GetHashCode();
        }
    }
}
