using UnityEngine;

[CreateAssetMenu(fileName = "NewMission", menuName = "Workday/Mission")]
public class MissionData : ScriptableObject
{
    public enum MissionType { Mail, Folders, Report, DataEntry }
    public MissionType type;

    [Header("Content")]
    public string title;
    [TextArea] public string bodyText;
    public bool isGlitchy;
}