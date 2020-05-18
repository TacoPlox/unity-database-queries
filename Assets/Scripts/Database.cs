using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.Serialization.Json;
using System.IO;

[System.Serializable]
public class Player
{
    public string _id;

    public string name;
}

public class Database : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetPlayers());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator GetPlayers()
    {
        //Crear petición GET
        UnityWebRequest www = UnityWebRequest.Get("http://localhost:3001/players/");
        // UnityWebRequest www = UnityWebRequest.Get("http://127.0.0.1:3001/players/");

        //Hacer petición y esperar a que responda el servidor
        yield return www.SendWebRequest();

        //Manejar casos de error
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            //Imprimir el contenido de la respuesta del servidor
            Debug.Log(www.downloadHandler.text);

            //Crear serializer para leer conjuntos de Player
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(Player[]));
            

            //Manejar automáticamente abrir/cerrar el stream de datos utilizando "using"
            using (var stream = GenerateStreamFromString(www.downloadHandler.text))
            {

                //Transformar JSON en Player[]
                Player[] players = (Player[])jsonSerializer.ReadObject(stream);

                //Iterar cada player dentro de players
                foreach (var player in players)
                {
                    //Imprimir el name de cada player
                    Debug.Log($"player.name = {player.name}");
                }
            }

        }
    }

    /// <summary>
    /// Create a Stream from a given string
    /// </summary>
    /// <param name="s">string to transform into a stream</param>
    /// <returns>MemoryStream created from a string</returns>
    public static Stream GenerateStreamFromString(string s)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }
}
