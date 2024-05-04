// ReSharper disable CheckNamespace
using System;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class OnReadyAttribute 
    : Attribute
{
    public string Name { get; set; }

    public OnReadyAttribute(string nodeName)
    {
        Name = nodeName;
    }
}