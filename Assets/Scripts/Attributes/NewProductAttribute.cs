using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class NewProductAttribute : PropertyAttribute
{
    public NewProductAttribute()
    {

    }
}