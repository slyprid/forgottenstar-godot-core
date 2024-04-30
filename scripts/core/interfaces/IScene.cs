using Godot;

public interface IScene
{
    dynamic GetData();
    void ReceiveData(dynamic input);
    void InitializeScene();
    void StartScene();
}