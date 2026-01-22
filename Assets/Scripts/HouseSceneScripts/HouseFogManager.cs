using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;


public class HouseFogManager : MonoBehaviour
{
    [Header("Oda Listesi")]
    public List<RoomData> rooms; 

    [Header("Ayarlar")]
    public float fadeDuration = 0.5f;

    private void Start()
    {
        foreach (var room in rooms)
        {
            if (room.fogSprite != null)
            {
                Color c = room.fogSprite.color;
                c.a = 1f;
                room.fogSprite.color = c;
            }
        }
    }

    public void HandleRoomFade(Collider2D hitTrigger, bool entering)
    {
        foreach (var room in rooms)
        {
            if (room.roomTrigger == hitTrigger)
            {
                float targetAlpha = entering ? 0f : 1f;
                room.fogSprite.DOFade(targetAlpha, fadeDuration);
                break;
            }
        }
    }
}


[System.Serializable]
public class RoomData
{
    public string roomName;
    public Collider2D roomTrigger;
    public SpriteRenderer fogSprite;
}