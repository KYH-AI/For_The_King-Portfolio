using TMPro;
using UnityEngine;

public class UI_playerToken : UI_Base
{
    private enum Token
    {
        Stun = 0,
        Bleeding,
        Poison,
        Weaking,
    }
    private enum TokenCount
    {
        StunCount,
        BleedingCount,
        PoisonCount,
        WeakingCount,  
    }
    public override void Init()
    {
        Bind<GameObject>(typeof(Token));
        Bind<TextMeshProUGUI>(typeof(TokenCount));
    }

    public void PutToken(TokenType type,int Count)
    {
        int index = TypeMapping(type);
        Get<GameObject>(index).SetActive(true);
        Get<TextMeshProUGUI>(index).text = Count.ToString();
    }
    public void ReMoveToken(TokenType type)
    {
        int index = TypeMapping(type);
        Get<GameObject>(index).SetActive(false);
    }
    public void ReMoveAll()
    {
        for(int i=0; i < 4; i++)
        {
            Get<GameObject>(i).SetActive(false);
        }
    }

    public int TypeMapping(TokenType type)
    {
        switch (type)
        {
            case TokenType.Stun:
                return 0;
            case TokenType.Bleeding:
                return 1;
            case TokenType.Poison:
                return 2;
            case TokenType.Weakening:
                return 3;
            default: return 0;
        }
    }
}
    
