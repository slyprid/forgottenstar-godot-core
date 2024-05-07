// ReSharper disable CheckNamespace

#if TOOLS
using Godot;

[Tool]
public partial class ForgottenStarToolbox : EditorPlugin
{
    private PackedScene _toolboxScene;
    private Control _toolbox;
    
    public override void _EnterTree()
    {
        _toolboxScene = ResourceLoader.Load<PackedScene>("res://addons/forgotten_star_toolbox/scenes/toolbox.tscn");
        _toolbox = _toolboxScene.Instantiate<Control>();
        AddControlToBottomPanel(_toolbox, "Toolbox");
    }

	public override void _ExitTree()
    {
        RemoveControlFromBottomPanel(_toolbox);
        _toolbox?.Free();
    }
}
#endif