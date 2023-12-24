using UnityEngine;

public class EliteBoss : Monster
{
    public override void Dash(Vector3 direction)
    { 
    }

    protected override void OnPossessed()
    {
        base.OnPossessed();
        
        Status.Shield = null;
    }
}
