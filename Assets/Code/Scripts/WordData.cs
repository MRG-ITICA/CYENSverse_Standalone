// *********************************************
// *********************************************
// <info>
//   File: WordData.cs
//   Author: Fotos  Frangoudes
//   Project: CYENSverse/Assembly-CSharp
//   Creation Date: 2023/10/07 6:00 PM
//   Last Modification Date: 2023/10/08 9:03 AM
// </info>
// <copyright file="WordData.cs"/>
// Copyright (C) 2023
// *********************************************
// *********************************************

using System;
using System.Text;

using TriInspector;

[Serializable]
public class WordData
{
    #region Variables

    [ShowInInspector]
    public string word { get; private set; }

    [ShowInInspector]
    public int count { get; private set; }

    [ShowInInspector]
    public WordController wordController { get; private set; }

    #endregion Variables

    #region Constructor

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="wordValue"></param>
    /// <param name="countValue"></param>
    public WordData(string wordValue, int countValue)
    {
        word = wordValue;
        count = countValue;
    }

    /// <summary>
    ///     Set the word controller
    /// </summary>
    /// <param name="wordControllerValue"></param>
    public void SetController(WordController wordControllerValue)
    {
        wordController = wordControllerValue;
    }

    #endregion Constructor

    #region Formatter

    /// <summary>
    ///     Add word data to string builder in csv format
    /// </summary>
    /// <param name="sb"></param>
    public void ToCsv(StringBuilder sb)
    {
        sb ??= new StringBuilder();
        sb.Append(word);
        sb.Append(',');
        sb.Append(count);
    }

    #endregion Formatter

    #region Count setters

    /// <summary>
    ///     Reset the count
    /// </summary>
    public void ResetCount()
    {
        count = 0;
    }

    /// <summary>
    ///     Increase the count
    /// </summary>
    public void IncreaseCount()
    {
        count++;
    }

    /// <summary>
    ///     Decrease the count
    /// </summary>
    public void DecreaseCount()
    {
        count = (count > 0) ? count - 1 : 0;
    }

    #endregion Count setters
}