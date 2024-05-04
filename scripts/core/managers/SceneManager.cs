// ReSharper disable CheckNamespace
// ReSharper disable SuspiciousTypeConversion.Global

using System;
using Godot;
using Array = Godot.Collections.Array;

public partial class SceneManager 
    : Node
{
    public static SceneManager Instance => Core.GetSingleton<SceneManager>();

    #region Events Handlers

    [Signal] public delegate void ContentFinishedLoadingEventHandler(Node content);
    [Signal] public delegate void ContentInvalidEventHandler(string contentPath);
    [Signal] public delegate void ContentFailedToLoadEventHandler(string contentPath);

    [Signal] public delegate void LoadStartEventHandler(string loadingScreen);
    [Signal] public delegate void SceneAddedEventHandler(Node loadedScene, string loadingScreen);
    [Signal] public delegate void LoadCompleteEventHandler(Node loadedScene);

    #endregion

    #region Fields / Properties

    private PackedScene _loadingScreenScene;
    private LoadingScreen _loadingScreen;
    private string _transition;
    private Vector2 _transitionDirection;
    private string _contentPath;
    private Timer _loadProgressTimer;
    private Node _loadSceneInto;
    private Node _sceneToUnload;
    private bool _loadingInProgress;

    #endregion

    public override void _Ready()
    {
        ContentInvalid += OnContentInvalid;
        ContentFailedToLoad += OnContentFailedToLoad;
        ContentFinishedLoading += OnContentFinishedLoading;

        _loadingScreenScene = ResourceLoader.Load<PackedScene>(Registry.Scenes.LoadingScreen);
    }

    #region Methods / Functions

    private void AddLoadingScreen(SceneTransitionTypes transitionType = SceneTransitionTypes.FadeToBlack)
    {
        GD.Print("SceneManager.AddLoadingScreen");
        _transition = transitionType == SceneTransitionTypes.NoTransition
            ? SceneTransitionTypes.NoToTransition.ToSnakeCase()
            : transitionType.ToSnakeCase();

        _loadingScreen = _loadingScreenScene.Instantiate<LoadingScreen>();
        GetTree().Root.AddChild(_loadingScreen);
        _loadingScreen.StartTransition(_transition.ToSceneTransitionType());
    }

    public void SwapScenes(string sceneToLoad, Node loadInto = null, Node sceneToUnload = null, SceneTransitionTypes transitionType = SceneTransitionTypes.FadeToBlack)
    {
        GD.Print("SceneManager.SwapScenes");
        if (_loadingInProgress)
        {
            GD.PushWarning("SceneManager is already loading something");
            return;
        }

        _loadingInProgress = true;
        loadInto ??= GetTree().Root;
        _loadSceneInto = loadInto;
        _sceneToUnload = sceneToUnload;

        AddLoadingScreen(transitionType);
        LoadContent(sceneToLoad);
    }

    public void SwapScenes(string sceneToLoad, Vector2 moveDirection, Node loadInto = null, Node sceneToUnload = null, SceneTransitionTypes transitionType = SceneTransitionTypes.SceneToSceneSlide)
    {
        GD.Print("SceneManager.SwapScenes");
        if (_loadingInProgress)
        {
            GD.PushWarning("SceneManager is already loading something");
            return;
        }

        _transition = transitionType.ToSnakeCase();
        _loadSceneInto = loadInto;
        _sceneToUnload = sceneToUnload;
        _transitionDirection = moveDirection;
        LoadContent(sceneToLoad);
    }

    private async void LoadContent(string contentPath)
    {
        GD.Print("SceneManager.LoadContent");
        EmitSignal(SignalName.LoadStart, _loadingScreen);

        if (_transition != SceneTransitionTypes.SceneToSceneSlide.ToSnakeCase())
        {
            await ToSignal(_loadingScreen, LoadingScreen.SignalName.TransitionInComplete);
        }

        _contentPath = contentPath;
        var loader = ResourceLoader.LoadThreadedRequest(contentPath);
        if (!ResourceLoader.Exists(contentPath))
        {
            EmitSignal(SignalName.ContentInvalid);
            return;
        }

        _loadProgressTimer = new Timer
        {
            WaitTime = 0.1
        };
        _loadProgressTimer.Timeout += MonitorLoadStatus;

        GetTree().Root.AddChild(_loadProgressTimer);
        _loadProgressTimer.Start();
    }

    private void MonitorLoadStatus()
    {
        GD.Print("SceneManager.MonitorLoadStatus");
        var loadProgress = new Array();
        var loadStatus = ResourceLoader.LoadThreadedGetStatus(_contentPath, loadProgress);

        switch (loadStatus)
        {
            case ResourceLoader.ThreadLoadStatus.InvalidResource:
                EmitSignal(SignalName.ContentInvalid);
                _loadProgressTimer.Stop();
                break;
            case ResourceLoader.ThreadLoadStatus.InProgress:
                _loadingScreen?.UpdateProgressBar((int)loadProgress[0] * 100);
                break;
            case ResourceLoader.ThreadLoadStatus.Failed:
                EmitSignal(SignalName.ContentFailedToLoad);
                _loadProgressTimer.Stop();
                break;
            case ResourceLoader.ThreadLoadStatus.Loaded:
                _loadProgressTimer.Stop();
                _loadProgressTimer.QueueFree();
                EmitSignal(SignalName.ContentFinishedLoading, (ResourceLoader.LoadThreadedGet(_contentPath) as PackedScene)?.Instantiate());
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    #endregion

    #region Special Scene Events

    public void OpenOptionsScreen()
    {
        var scene = ResourceLoader.Load<PackedScene>(Registry.Scenes.OptionsScreen);
        var optionsScreen = scene.Instantiate();

        GetTree().Root.AddChild(optionsScreen);
    }

    #endregion

    #region Events

    private void OnContentInvalid(string contentPath)
    {
        GD.PrintErr($"Error: Failed to load resource [{contentPath}]");
    }

    private void OnContentFailedToLoad(string contentPath)
    {
        GD.PrintErr($"Error: Failed to load resource [{contentPath}]");
    }

    private async void OnContentFinishedLoading(Node incomingScene)
    {
        GD.Print("SceneManager.OnContentFinishedLoading");
        var outgoingScene = _sceneToUnload;

        if (outgoingScene != null)
        {
            if (outgoingScene is IScene oScene && incomingScene is IScene iScene)
            {
                iScene.ReceiveData(oScene.GetData());
            }
            else if (outgoingScene is IScene)
            {
                GD.PushWarning("Outgoing scene implemented GetData but Incoming Scene doesn't have a ReceiveData");
            }
            else if (incomingScene is IScene)
            {
                GD.PushWarning("Incoming scene implemented GetData but Outgoing Scene doesn't have a ReceiveData");
            }
        }

        _loadSceneInto.AddChild(incomingScene);
        EmitSignal(SignalName.SceneAdded, incomingScene, _loadingScreen);

        if (_transition == SceneTransitionTypes.SceneToSceneSlide.ToSnakeCase())
        {
            var x = _transitionDirection.X * GetViewport().GetVisibleRect().Size.X;
            var y = _transitionDirection.Y * GetViewport().GetVisibleRect().Size.Y;
            (incomingScene as Node2D).Position = new Vector2(x, y);
            
            var tweenIn = GetTree().CreateTween();
            tweenIn.TweenProperty(incomingScene, "position", Vector2.Zero, 1).SetTrans(Tween.TransitionType.Sine);

            var tweenOut = GetTree().CreateTween();
            var vosX = -_transitionDirection.X * GetViewport().GetVisibleRect().Size.X;
            var vosY = -_transitionDirection.Y * GetViewport().GetVisibleRect().Size.Y;
            var vectorOffScreen = new Vector2(vosX, vosY);
            tweenOut.TweenProperty(outgoingScene, "position", vectorOffScreen, 1).SetTrans(Tween.TransitionType.Sine);

            await ToSignal(tweenIn, Tween.SignalName.Finished);
        }

        if (_sceneToUnload != null)
        {
            if (_sceneToUnload != GetTree().Root) _sceneToUnload.QueueFree();
        }

        if (incomingScene is IScene)
        {
            (incomingScene as IScene).InitializeScene();
        }

        if (_loadingScreen != null)
        {
            _loadingScreen.FinishTransition();
            await ToSignal(_loadingScreen.AnimationPlayer, AnimationPlayer.SignalName.AnimationFinished);
        }

        if (incomingScene is IScene)
        {
            (incomingScene as IScene).StartScene();
        }

        _loadingInProgress = false;
        EmitSignal(SignalName.LoadComplete, incomingScene);
    }

    #endregion
}