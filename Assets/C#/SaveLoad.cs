using UnityEngine;
// next line enables use of the operating system's serialization capabilities within the script
using System.Runtime.Serialization.Formatters.Binary;
// next line, IO stands for Input/Output, and is what allows us to write to and read from
// our computer or mobile device. Allowing to create unique files and then read them.
using System.IO;

public static class SaveLoad {

    public const int SOUND_NOICE = 0;
    public const int MUSIC_NOICE = 1;
    public const string FILE_PATH = "/CrashBall.gd";

    public static float[] Noices { get; set; }

    public static bool FindSaveFile()
    {
        return File.Exists(Application.persistentDataPath + FILE_PATH);
    }

    public static void MakeSaveFile()
    {
        Noices = new float[2] { 0.6f, 0.4f };
    }

    public static void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + FILE_PATH);
        bf.Serialize(file, Noices);
        file.Close();
    }

    public static void Load()
    {
        if (File.Exists(Application.persistentDataPath + FILE_PATH))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + FILE_PATH, FileMode.Open);
            Noices = (float[])bf.Deserialize(file);
            file.Close();
        }
    }

    public static void Delete()
    {
        if (File.Exists(Application.persistentDataPath + FILE_PATH))
            File.Delete(Application.persistentDataPath + FILE_PATH);
    }
}
