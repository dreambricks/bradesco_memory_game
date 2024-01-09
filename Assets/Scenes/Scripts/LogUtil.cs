using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LogUtil : MonoBehaviour
{

    public static void SendLog(StatusEnum status, string timerPoints)
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "data_logs");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        DataLog data = new DataLog();

        data.status = status.ToString();
        data.timerPoints = timerPoints;

        string formattedDateTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");

        data.timePlayed = formattedDateTime;


        string json = JsonUtility.ToJson(data);

        string fileName = string.Format("{0}_datalog.json", data.timePlayed.Replace("-", "").Replace("T", "_").Replace(":", "").Replace("Z", ""));

        string filePath = Path.Combine(folderPath, fileName);
        Debug.Log(filePath);

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.Write(json);
        }

    }

    public static void SendLog(StatusEnum status)
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "data_logs");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        DataLog data = new DataLog();

        data.status = status.ToString();

        string formattedDateTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");

        data.timePlayed = formattedDateTime;


        string json = JsonUtility.ToJson(data);

        string fileName = string.Format("{0}_datalog.json", data.timePlayed.Replace("-", "").Replace("T", "_").Replace(":", "").Replace("Z", ""));

        string filePath = Path.Combine(folderPath, fileName);
        Debug.Log(filePath);

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.Write(json);
        }

    }

}
