// ReSharper disable CheckNamespace

using Godot;
using System.Linq;
using System;

public static class NodeExtensions
{
    internal static bool Debug = false;

    public static void SetOnReadyProperties(this Node node)
    {
        var properties = node.GetType().GetProperties();
        foreach (var propertyInfo in properties)
        {
            if (!Attribute.IsDefined(propertyInfo, typeof(OnReadyAttribute))) continue;
            var attributes = propertyInfo.GetCustomAttributes(typeof(OnReadyAttribute), true);
            var attribute = ((OnReadyAttribute)attributes.Single());
            var nodeName = attribute.Name;
            var nodeType = propertyInfo.PropertyType;
            var newNode = node.GetNode(nodeName);

            if (nodeType != newNode.GetType())
            {
                GD.PushError($">> ERROR: OnReady Node Type Mismatch");
                GD.PushError($">> [{nodeName}] is {nodeType} but found to be {newNode.GetType()}");
                throw new Exception("OnReadyNodeTypeMismatch");
            }

            if (Debug)
            {
                GD.Print($">> Name: {nodeName} => Type: {nodeType} => New: {newNode}");
            }

            var value = Convert.ChangeType(newNode, nodeType);
            propertyInfo.SetValue(node, value);
        }
    }
}