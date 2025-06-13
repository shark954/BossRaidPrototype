using UnityEngine;

[CreateAssetMenu(fileName = "ClassDatabase", menuName = "Game/Class Database")]
public class ClassDatabase : ScriptableObject
{
    public ClassData[] classes;

    public ClassData GetClassByID(int id)
    {
        foreach (var c in classes)
        {
            if (c.classID == id)
                return c;
        }
        return null;
    }
}
