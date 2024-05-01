using System;
using Godot;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public partial class OnReadyAttribute 
    : Attribute
{
    public string Name { get; set; }

    public OnReadyAttribute(string nodeName)
    {
        Name = nodeName;
    }
}