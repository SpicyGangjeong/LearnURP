using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Method)]
public class EnumActionAttribute : PropertyAttribute
{
    public Type EnumType { get; }
    public EnumActionAttribute(Type enumType) => EnumType = enumType;
}
