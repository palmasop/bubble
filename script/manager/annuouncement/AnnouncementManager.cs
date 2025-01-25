using System;
using Godot;

public partial class AnnouncementManager : Singleton<AnnouncementManager>
{
    [Export] PackedScene textAnnouncementScene;
    [Export] PackedScene comfirmationAnnouncementScene;

    CanvasLayer messageContainer;

    public void TextAnnounce(string message, int fontSize = -1, float displayTime = 2.0f)
    {
        ClearExistingAnnouncements<PopupAnnouncement>();
        var instance = (PopupAnnouncement)textAnnouncementScene.Instantiate();
        GetMessageContainer().AddChild(instance);
        instance.ShowAnnouncement(message, fontSize, displayTime);
    }

    public void ConfirmationAnnounce(string message, Action onConfirm)
    {
        ClearExistingAnnouncements<ConfirmationAnnouncement>();
        var instance = (ConfirmationAnnouncement)comfirmationAnnouncementScene.Instantiate();
        GetMessageContainer().AddChild(instance);
        instance.ShowAnnouncement(message, onConfirm);
    }

    private void ClearExistingAnnouncements<T>() where T : Node
    {
        foreach (var child in GetMessageContainer().GetChildren())
        {
            if (child is T)
                child.QueueFree();
        }
    }

    #region Old
    void CreateMessageContainer()
    {
        messageContainer?.QueueFree();
        messageContainer = new CanvasLayer();
        messageContainer.SetName("Announce Messages");
        AddChild(messageContainer);
    }

    CanvasLayer GetMessageContainer()
    {
        if (messageContainer == null || !messageContainer.IsVisible())
            CreateMessageContainer();
        return messageContainer;
    }

    void ResetMessage(bool isGameScene, string scenePath)
    {
        if (!isGameScene)
        {
            foreach (var child in messageContainer.GetChildren())
                child?.QueueFree();
        }
    }
    #endregion
}