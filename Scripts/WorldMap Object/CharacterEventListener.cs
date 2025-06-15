using UnityEngine;

public class CharacterEventListener
{
    /*  <주 목표 : 캐릭터에 관련된 정보를 관리한다.>
     *  1. 애니메이션
     *  2. Mesh Renderer
     */

    public enum AnimationTrigger
    {
        Idle = 0,
        Defense,
        Avoid,
        TakeSmallDamage,
        TakeHeavyDamage,
        Skill1,
        Skill2,
        Skill3,
        Skill4,
        Skill5,
        Skill6,
        Taunt,
        Dead,
        Revive,
        LevelUp,
        Escape,
        UseItem,
        Run,
        WorldMapDead,
        Victory,
    }

    // 해당 오브젝트의 애니메이션 컴포넌트
    private Animator _animator;

    // 해당 오브젝트의 머티리얼
    private SkinnedMeshRenderer _characterRenderers;

    private bool _isVisible = true;
    
    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            if (_isVisible == value) return;
            _isVisible = value;
            _characterRenderers.enabled = _isVisible;
        }
    } 

    public CharacterEventListener(Animator ownerAnimator)
    {
        _animator = ownerAnimator;
    }

    public void UpdateCharacterRenderer(SkinnedMeshRenderer ownerRenderer)
    {
        if (_characterRenderers != null) return;
       _characterRenderers = ownerRenderer;
    }
    
    public void SetRunTimeAnimator(RuntimeAnimatorController runtimeAnimatorController)
    {
        _animator.runtimeAnimatorController = runtimeAnimatorController;
    }

    public void PlayAnimationTrigger(AnimationTrigger animationTrigger)
    {
        _animator.SetTrigger(animationTrigger.ToString());
    }
    
    public Animator GetAnimator()
    {
        return _animator;
    }
    
}
