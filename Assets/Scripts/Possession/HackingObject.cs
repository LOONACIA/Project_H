using UnityEngine;

public abstract class HackingObject : MonoBehaviour
{
    protected enum HackingSoundType {Hacking, Unhacking }


    #region PublicVariables
    #endregion

    #region PrivateVariables
    public AudioClip[] audios;
    #endregion

    #region PublicMethod
    public abstract void Interact();
    #endregion

    #region PrivateMethod
    #endregion
}
