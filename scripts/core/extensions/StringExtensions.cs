// ReSharper disable CheckNamespace

using System;
using System.Linq;
using System.Text;
using Godot;

public static class StringExtensions
{
    public static string ToSnakeCase(this string input)
    {
        if (input == null) throw new ArgumentNullException(nameof(input));
        if (input.Length < 2) return input.ToLowerInvariant();
        var sb = new StringBuilder();
        sb.Append(char.ToLowerInvariant(input[0]));
        for (var i = 1; i < input.Length; ++i)
        {
            var c = input[i];
            if (char.IsUpper(c))
            {
                sb.Append('_');
                sb.Append(char.ToLowerInvariant(c));
            }
            else
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }

    public static string ToSnakeCase(this object input)
    {
        return ToSnakeCase(input.ToString());
    }

    public static string ToCamelCase(this string input)
    {
        if (input == null) throw new ArgumentNullException(nameof(input));

        return string.Join("", input.Split('_')
            .Select(w => w.Trim())
            .Where(w => w.Length > 0)
            .Select(w => w[..1].ToUpper() + w[1..].ToLower()));
    }

    public static SceneTransitionTypes ToSceneTransitionType(this string input)
    {
        if (input.Contains("_")) input = input.ToCamelCase();
        if (Enum.TryParse(input, out SceneTransitionTypes ret))
        {
            return ret;
        }

        GD.PushError($"[{input}] is not a SceneTransitionType");
        return SceneTransitionTypes.NoTransition;
    }
}