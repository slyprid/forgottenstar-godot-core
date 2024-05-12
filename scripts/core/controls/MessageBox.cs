// ReSharper disable CheckNamespace

using System;
using Godot;

public partial class MessageBox : CanvasLayer
{
    #region Controls

    [OnReady("%MessageText")] public RichTextLabel MessageText { get; set; }
    [OnReady("%OkButton")] public Button OkButton { get; set; }
    [OnReady("%YesButton")] public Button YesButton { get; set; }
    [OnReady("%NoButton")] public Button NoButton { get; set; }
    [OnReady("%CancelButton")] public Button CancelButton { get; set; }

    #endregion

    #region Fields

    private Action _onOkPressed;
    private Action _onNoPressed;
    private Action _onYesPressed;
    private Action _onCancelPressed;

    #endregion

    #region Properties

    private static PackedScene _packedScene;

    public string Message { get; set; }

    #endregion

    public static MessageBox Show(string message)
    {
        _packedScene ??= ResourceLoader.Load<PackedScene>(Registry.Scenes.MessageBox);

        var ret = _packedScene.Instantiate<MessageBox>();
        ret.Message = message;
        Globals.Root.AddChild(ret);

        return ret;
    }

    public override void _Ready()
    {
        this.SetOnReadyProperties();

        MessageText.Text = Message;

        OkButton.Visible = false;
        YesButton.Visible = false;
        CancelButton.Visible = false;
        NoButton.Visible = false;
    }

    public override void _Notification(int notification)
    {
        if (notification is (int)NotificationEnterTree or (int)NotificationExitTree)
        {
            GetTree().Paused = notification switch
            {
                (int)NotificationEnterTree => true,
                (int)NotificationExitTree => false
            };
        }
    }
    
    public void Close()
    {
        QueueFree();
    }

    #region Events / Signals

    public void OnNoButtonPressed()
    {
        _onNoPressed?.Invoke();
        Close();
    }

    public void OnOkButtonPressed()
    {
        _onOkPressed?.Invoke();
        Close();
    }

    public void OnYesButtonPressed()
    {
        _onYesPressed?.Invoke();
        Close();
    }

    public void OnCancelButtonPressed()
    {
        _onCancelPressed?.Invoke();
        Close();
    }

    #endregion

    #region Fluent Functions

    public MessageBox HasOkButton(Action onOkPressed = null)
    {
        OkButton.Visible = true;
        _onOkPressed = onOkPressed;
        return this;
    }

    public MessageBox DoesntHaveOkButton()
    {
        OkButton.Visible = false;
        return this;
    }

    public MessageBox HasYesButton(Action onYesPressed = null)
    {
        YesButton.Visible = true;
        _onYesPressed = onYesPressed;
        return this;
    }

    public MessageBox DoesntHaveYesButton()
    {
        YesButton.Visible = false;
        return this;
    }

    public MessageBox HasNoButton(Action onNoPressed = null)
    {
        NoButton.Visible = true;
        _onNoPressed = onNoPressed;
        return this;
    }

    public MessageBox DoesntHaveNoButton()
    {
        NoButton.Visible = false;
        return this;
    }

    public MessageBox HasCancelButton(Action onCancelPressed = null)
    {
        CancelButton.Visible = true;
        _onCancelPressed = onCancelPressed;
        return this;
    }

    public MessageBox DoesntHaveCancelButton()
    {
        CancelButton.Visible = false;
        return this;
    }

    #endregion
}