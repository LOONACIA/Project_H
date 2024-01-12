using UnityEngine;
using UnityEngine.Video;

public class ModalDialog : Notification
{
    [field: SerializeField]
    public VideoClip Video { get; set; }

    [field: SerializeReference]
    public ModalDialog RelatedDialog { get; set; }
}