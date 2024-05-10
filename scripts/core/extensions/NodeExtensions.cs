using Godot;
using System.Linq;
using System;
using System.Reflection;

public static class NodeExtensions
{
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
            var value = Convert.ChangeType(newNode, nodeType);
            propertyInfo.SetValue(node, value);
        }
    }
}