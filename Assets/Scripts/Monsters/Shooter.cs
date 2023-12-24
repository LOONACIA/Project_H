public class Shooter : Monster
{
    protected override void UpdateAnimator()
    {
        base.UpdateAnimator();

        // 3인칭인 경우, 타겟이 없으면 BaseLayer, 있으면 AimLayer를 실행
        if (!IsPossessed)
        {
            float weight = Targets.Count > 0 ? 1 : 0;
            Animator.SetLayerWeight(Animator.GetLayerIndex(ConstVariables.ANIMATOR_LAYER_AIM_LAYER), weight);
        }
    }

    protected override void OnUnPossessed()
    {
        base.OnUnPossessed();

        // Shooter는 빙의 해제 시 스나이퍼로 강제 전환
        Attack.ChangeWeapon(Animator.GetComponent<Sniper>());
    }
}