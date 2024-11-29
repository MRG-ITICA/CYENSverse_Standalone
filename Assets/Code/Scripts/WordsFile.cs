// *********************************************
// *********************************************
// <info>
//   File: WordsFile.cs
//   Author: Fotos  Frangoudes
//   Project: CYENSverse/Assembly-CSharp
//   Creation Date: 2023/10/06 7:34 AM
//   Last Modification Date: 2023/10/08 10:39 AM
// </info>
// <copyright file="WordsFile.cs"/>
// Copyright (C) 2023
// *********************************************
// *********************************************

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using UnityEngine;

[DefaultExecutionOrder(int.MinValue)]
public class WordsFile : MonoBehaviour
{
    #region Variables

    private string externalPath;

    #endregion Variables

    #region Unity Messages

    void Awake()
    {
        externalPath = Path.Combine(Application.persistentDataPath, "words.csv");
    }

    #endregion Unity Messages

    #region Words Input / Output

    /// <summary>
    ///     Load the words
    /// </summary>
    /// <returns></returns>
    public WordData[] LoadWords()
    {
        var data = string.Empty;
        if (!File.Exists(externalPath))
        {
            var dataFile = Resources.Load<TextAsset>("words");
            if (dataFile != null)
            {
                data = dataFile.text;
            }
        }
        else
        {
            data = File.ReadAllText(externalPath);
        }

        return ReadWords(data);
    }

    /// <summary>
    ///     Read words from "words" csv file
    /// </summary>
    /// <param name="data">data in a csv format</param>
    /// <returns></returns>
    private WordData[] ReadWords(string data)
    {
        // Splitting the dataset in the end of line
        var splitDataset = data.Split('\n');

        // Iterating through the split dataset in order to split into rows
        var numberOfRows = splitDataset.Length;
        // remove 1 from the number of words, due to the header
        var words = new WordData[numberOfRows - 1];
        for (var i = 1; i < numberOfRows; i++)
        {
            var row = splitDataset[i].Split(',');
            words[i - 1] = new WordData(row[0].Trim(), int.Parse(row[1]));
        }

        IList<WordData> shuffledWords = Shuffle(words.ToList());
        return shuffledWords.ToArray();
    }

    private IList<T> Shuffle<T>(IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
        return ts;
    }

    /// <summary>
    ///     Save all the words to a file
    /// </summary>
    /// <param name="words"></param>
    public void SaveWords(WordData[] words)
    {
        var sb = new StringBuilder("word,count");

        for (int i = 0, n = words.Length; i < n; i++)
        {
            sb.AppendLine();
            words[i].ToCsv(sb);
        }

        File.WriteAllText(externalPath, sb.ToString());
    }

    #endregion Words Input / Output
}