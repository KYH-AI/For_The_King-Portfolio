using UnityEngine;

public interface I_SanctuaryBuff
{
    /// <summary>
    /// ���� ���� Ȱ��ȭ
    /// </summary>
    /// <param name="request">��û��</param>
    public void EnableSanctuaryBuff(PlayerStats request);

    /// <summary>
    /// ���� ���� ��Ȱ��ȭ
    /// </summary>
    /// <param name="request">��û��</param>
    public void DisableSanctuaryBuff(PlayerStats request);
}
