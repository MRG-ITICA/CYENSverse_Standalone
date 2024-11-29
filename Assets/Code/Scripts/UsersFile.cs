using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using System.Linq;
using System;

public class UsersFile : MonoBehaviour
{
    private string ageRange;

    private string externalPath;

    private TextWriter tsw;

    void OnEnable()
    {
        externalPath = Path.Combine(Application.persistentDataPath, "userWords.csv");
        ageRange = "Prefer not to say";
    }

    public void SaveUserWords()
    {
        ageRange = PlayerPrefs.GetString("ageRange");
        var sb = new StringBuilder();

        List<string> userList = new List<string>
        {
            DateTime.Now.ToString(),
            ageRange
        };

        var wordCloud = gameObject.GetComponent<WordCloud>();

        WordData[] words = wordCloud.GetWords();
        foreach ( var wordData in words )
        {
            if (wordData.wordController.IsSelected())
            {
                userList.Add(wordData.word);
            }
        }

        var array = string.Join(",", userList.ToArray());
        sb.AppendLine(array);

        tsw = new StreamWriter(externalPath, true);
        tsw.Write(sb.ToString());
        tsw.Close();
    }
}
