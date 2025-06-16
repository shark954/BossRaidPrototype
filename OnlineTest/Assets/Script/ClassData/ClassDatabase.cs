using UnityEngine;

[CreateAssetMenu(fileName = "ClassDatabase", menuName = "Game/Class Database")]
public class ClassDatabase : ScriptableObject
{
    public ClassData[] m_classes;

    public ClassData GetClassByID(int id)
    {
        foreach (var c in m_classes)
        {
            if (c.m_classID == id)
                return c;
        }
        return null;
    }
}
