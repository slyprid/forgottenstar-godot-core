// ReSharper disable CheckNamespace
// ReSharper disable AccessToStaticMemberViaDerivedType

using Godot;

public partial class LoadingScreen 
    : Control
{
    #region Event Handlers

    [Signal] public delegate void TransitionInCompleteEventHandler();

    #endregion

    #region Fields / Properties

    private ProgressBar _progressBar;
    private AnimationPlayer _animationPlayer;
    private Timer _timer;

    private string _startingAnimationName;

    public AnimationPlayer AnimationPlayer => _animationPlayer;

    #endregion

    public override void _Ready()
    {
        _progressBar = GetNode<ProgressBar>("%ProgressBar");
        _animationPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");
        _timer = GetNode<Timer>("%Timer");

        _progressBar.Visible = false;
    }

    public void StartTransition(SceneTransitionTypes transitionType)
    {
        GD.Print("LoadingScreen.StartTransition");
        var animationName = transitionType.ToSnakeCase();

        if (!_animationPlayer.HasAnimation(animationName))
        {
            GD.PushWarning($"[{animationName}] animation does not exist");
            animationName = SceneTransitionTypes.FadeFromBlack.ToSnakeCase();
        }

        _startingAnimationName = animationName;
        _animationPlayer.Play(animationName);
        _timer.Start();
    }

    public async void FinishTransition()
    {
        GD.Print("LoadingScreen.FinishTransition");
        if (!_timer.IsStopped()) _timer.Stop();

        var endingAnimationName = _startingAnimationName.Replace("to", "from");

        if (!_animationPlayer.HasAnimation(endingAnimationName))
        {
            GD.PushWarning($"[{endingAnimationName}] animation does not exist");
            endingAnimationName = SceneTransitionTypes.FadeFromBlack.ToSnakeCase();
        }
        _animationPlayer.Play(endingAnimationName);
        await ToSignal(_animationPlayer, AnimationPlayer.SignalName.AnimationFinished);

        QueueFree();
    }

    private void ReportMidPoint()
    {
        GD.Print("LoadingScreen.ReportMidPoint");
        EmitSignal(SignalName.TransitionInComplete);
    }

    public void OnTimerTimeout()
    {
        GD.Print("LoadingScreen.OnTimerTimeout");
        _progressBar.Visible = true;
    }

    public void UpdateProgressBar(int value)
    {
        GD.Print("LoadingScreen.UpdateProgressBar");
        _progressBar.Value = value;
    }
}