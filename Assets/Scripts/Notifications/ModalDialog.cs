using UnityEngine.Video;

public class ModalDialog : NotificationBase
{
    public VideoClip Video { get; set; }
    
    public ModalDialog RelatedDialog { get; set; }
}