using System.Collections.Generic;
using UnityEngine;


public class ListLevel : MonoBehaviour
{

    //private static IList serializedObject;
    public List<string> names = new List<string>() ;


    [ContextMenu("List elements")]
    public void ListAll()
    {
        foreach (string s in names)
            Debug.Log(s);
    }

    [ContextMenu("Clean Up Path")]
    public void CleanUpPath()
    {
        for(int i =0; i < names.Count; i++)
        {
            Debug.Log(i);
            names[i] = names[i].Replace("Assets/Resources/","");
        }
    }


}
