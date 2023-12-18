using Unity.VisualScripting;

public class Shooter : Monster
{
    // TODO: Remove this
    public override void Block(bool value)
    {
    }

    protected override void UpdateAnimator()
    {
        base.UpdateAnimator();

        // 3인칭인 경우, 타겟이 없으면 BaseLayer, 있으면 AimLayer를 실행
        if (!IsPossessed)
            Animator.SetLayerWeight(Animator.GetLayerIndex(ConstVariables.ANIMATOR_LAYER_AIM_LAYER), 
                                    (Targets.Count > 0 ? 1 : 0));
    }
}
