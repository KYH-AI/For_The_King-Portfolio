using UnityEngine;

public interface I_SanctuaryBuff
{
    /// <summary>
    /// 성소 버프 활성화
    /// </summary>
    /// <param name="request">요청자</param>
    public void EnableSanctuaryBuff(PlayerStats request);

    /// <summary>
    /// 성소 버프 비활성화
    /// </summary>
    /// <param name="request">요청자</param>
    public void DisableSanctuaryBuff(PlayerStats request);
}
