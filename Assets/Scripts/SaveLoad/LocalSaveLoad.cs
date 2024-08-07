using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class LocalSaveLoad
{
    public static void Save<T>(T data, string fileName) where T : class
    {
        string path = $"{Application.persistentDataPath}/{fileName}";

        BinaryFormatter bf = new BinaryFormatter();
        using FileStream f_stream = new FileStream(path, FileMode.Create);

        bf.Serialize(f_stream, data);
        f_stream.Close();
    }

    public static T Load<T>(string fileName) where T : class
    {
        string path = $"{Application.persistentDataPath}/{fileName}";
        if(File.Exists(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            using FileStream f_stream = new FileStream(path, FileMode.Open);
            if(f_stream.Length == 0)
            {
                f_stream.Close();
                return null;
            }
            T value = bf.Deserialize(f_stream) as T;
            f_stream.Close();
            return value;
        }

        return null;
    }
}
