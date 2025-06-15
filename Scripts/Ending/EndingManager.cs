using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndingManager : MonoBehaviour
{
    public TextMeshProUGUI NowConversation;
    public TextMeshProUGUI NowWho;
    public AudioClip[] AudioClips;
 
    List<Conversation> ConversationList;

    private void Start()
    {
        ConversationList = new List<Conversation>()
        {

        };
    }

}
class Conversation
{
    public string Who;
    public string Text;
    public AudioClip AudioClip;

    public Conversation(string who, string conversation, AudioClip audioClip)
    {
        this.Who = who;
        this.Text = conversation;
        AudioClip = audioClip;
    }
}
