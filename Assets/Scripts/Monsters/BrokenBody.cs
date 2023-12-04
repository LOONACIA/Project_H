using UnityEngine;

public class BrokenBody : Monster
{
    public override void Move(Vector3 direction)
    {
    }

    public override void TryJump()
    {
    }

    public override void TryAttack()
    {
    }

    public override void Skill()
    {
    }

    public override void Dash(Vector3 direction)
    {
    }

    public override void Block(bool value)
    {
    }

    protected override void OnPossessed()
    {
        base.OnPossessed();
        
        GameManager.Effect.PlayBrokenBodyViewEffect();
    }

    protected override void OnUnPossessed()
    {
        base.OnUnPossessed();
        
        GameManager.Effect.StopBrokenBodyViewEffect();
    }
}
