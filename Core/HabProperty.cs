// Decompiled with JetBrains decompiler
// Type: ReplaySeeker.Core.HabProperty
// Assembly: ReplaySeeker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System;
using System.Collections.Generic;

namespace ReplaySeeker.Core
{
  public class HabProperty
  {
    public static char HeaderSeparator = ',';

    public static string GetNameWithHeader(string name, string header)
    {
      return header + (object) HabProperty.HeaderSeparator + name;
    }

    public static unsafe KeyValuePair<string, object> FromString(string lpText)
    {
      fixed (char* chPtr1 = lpText)
      {
        char* chPtr2 = chPtr1;
        while ((int) *chPtr2 != 61 && (int) *chPtr2 != 0)
          ++chPtr2;
        if ((int) *chPtr2 == 0)
          return new KeyValuePair<string, object>(lpText, (object) null);
        char* chPtr3;
        return new KeyValuePair<string, object>(new string(chPtr1, 0, (int) (chPtr2 - chPtr1)), (object) new string(chPtr3 = chPtr2 + 1));
      }
    }

    public static unsafe void FromStringFastUnsafe(ref char* ptr, out string name, out string value)
    {
      char* chPtr1 = ptr;
      while ((int) *ptr != 61 && (int) *ptr != 13)
        ++ptr;
      if ((int) *ptr == 13)
      {
        name = new string(ptr);
        value = (string) null;
      }
      else
      {
        name = new string(chPtr1, 0, (int) (ptr - chPtr1));
        char* chPtr2 = ++ptr;
        while ((int) *ptr != 13)
          ++ptr;
        value = new string(chPtr2, 0, (int) (ptr - chPtr2));
      }
      ptr += 2;
    }

    public static unsafe void FromStringFastUnsafe(ref sbyte* ptr, out string name, out string value)
    {
      sbyte* numPtr1 = ptr;
      while ((int) *ptr != 61 && (int) *ptr != 13)
        ++ptr;
      if ((int) *ptr == 13)
      {
        name = new string(ptr);
        value = (string) null;
      }
      else
      {
        name = new string(numPtr1, 0, (int) (ptr - numPtr1));
        sbyte* numPtr2 = ++ptr;
        while ((int) *ptr != 13)
          ++ptr;
        value = new string(numPtr2, 0, (int) (ptr - numPtr2));
      }
      ptr += 2;
    }

    public static KeyValuePair<string, object> Create(string name, object value)
    {
      return new KeyValuePair<string, object>(name, value);
    }

    public static unsafe List<string> SplitValue(string value)
    {
      List<string> stringList = new List<string>();
      fixed (char* chPtr1 = value)
      {
        char* chPtr2 = chPtr1;
        while ((int) *chPtr2 != 0)
        {
          char* chPtr3;
          char* chPtr4;
          if ((int) *chPtr2 == 34)
          {
            char* chPtr5;
            chPtr3 = chPtr5 = chPtr2 + 1;
            while ((int) *chPtr5 != 34)
              ++chPtr5;
            char* chPtr6 = chPtr5;
            chPtr2 = (char*) ((IntPtr) chPtr6 + 2);
            chPtr4 = chPtr6;
            while ((int) *chPtr2 != 0 && (int) *chPtr2++ != 44)
              ;
          }
          else
          {
            chPtr3 = chPtr2;
            while ((int) *chPtr2 != 44 && (int) *chPtr2 != 0)
              ++chPtr2;
            chPtr4 = chPtr2;
            if ((int) *chPtr2 != 0)
              ++chPtr2;
          }
          stringList.Add(new string(chPtr3, 0, (int) (chPtr4 - chPtr3)));
        }
      }
      return stringList;
    }

    public static unsafe List<string> SplitValue(string value, int offset)
    {
      List<string> stringList = new List<string>();
      fixed (char* chPtr1 = value)
      {
        char* chPtr2 = chPtr1;
        while ((int) *chPtr2 != 0)
        {
          char* chPtr3;
          char* chPtr4;
          if ((int) *chPtr2 == 34)
          {
            char* chPtr5;
            chPtr3 = chPtr5 = chPtr2 + 1;
            while ((int) *chPtr5 != 34)
              ++chPtr5;
            char* chPtr6 = chPtr5;
            chPtr2 = (char*) ((IntPtr) chPtr6 + 2);
            chPtr4 = chPtr6;
            while ((int) *chPtr2 != 0 && (int) *chPtr2++ != 44)
              ;
          }
          else
          {
            chPtr3 = chPtr2;
            while ((int) *chPtr2 != 44 && (int) *chPtr2 != 0)
              ++chPtr2;
            chPtr4 = chPtr2;
            if ((int) *chPtr2 != 0)
              ++chPtr2;
          }
          stringList.Add(new string(chPtr3, 0, (int) (chPtr4 - chPtr3)));
        }
      }
      while (offset-- > 0)
        stringList.Insert(0, (string) null);
      return stringList;
    }

    public static void SetValue(List<string> list, int index, string value)
    {
      if (index >= list.Count)
      {
        int num = index - list.Count;
        while (num-- > 0)
          list.Add((string) null);
        list.Add(value);
      }
      else
        list[index] = value;
    }

    public static string ToStringList(object value)
    {
      if (value is string)
        return value as string;
      if (value is IEnumerable<string>)
      {
        string str1 = "";
        foreach (string str2 in value as IEnumerable<string>)
          str1 = !string.IsNullOrEmpty(str2) ? (!str2.StartsWith("\"") ? str1 + (str2.Contains(",") ? "\"" + str2 + "\"" : str2) + "," : str1 + str2 + ",") : str1 + ",";
        return str1.TrimEnd(',');
      }
      if (value is HabPropertiesCollection)
        return (value as HabPropertiesCollection).ToString(true);
      return value.ToString() + "";
    }
  }
}
