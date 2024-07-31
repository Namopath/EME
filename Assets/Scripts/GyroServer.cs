using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using System.Collections;

public class GyroServer : MonoBehaviour
{
    Thread thread;
    public int connectionPort = 25002;
    TcpListener server;
    bool running;
    public GameObject scoreObj;
    private int intScore = 0;
    private TMP_Text tmpText;
    TcpClient client;
    Queue scoreQueue = new Queue();
    private object queueLock = new object();
    private String prevData;
    public GameObject hrWarn;
   

    void Start()
    {
        StartServer();
        print("Gyroscope server is running");
        if (scoreObj != null)
        {
            try
            {
                tmpText = scoreObj.GetComponentInChildren<TMP_Text>();
                tmpText.text = "Score: 0";
                print(tmpText.text);
            }
            catch (Exception e)
            {
                print("Error fetching score from obj:" + e);
            }
        }
        else
        {
            print("No score object detected");
        }
    }

    void OnDestroy()
    {
        StopServer();
        print("Server stopped on destroyed");
    }

    void StopServer()
    {
        if (running)
        {
            running = false;
            server.Stop();
            if (thread != null && thread.IsAlive)
            {
                thread.Abort();
            }
        }
    }

    void StartServer()
    {
        if (!running)
        {
            running = true;
            thread = new Thread(GetData);
            thread.Start();
        }
    }

    void GetData()
    {
        try
        {
            server = new TcpListener(IPAddress.Any, connectionPort);
            server.Start();
            print("Server started on port " + connectionPort);

            client = server.AcceptTcpClient();
            running = true;

            while (client.Connected)
            {
                Connection();
            }
            
            server.Stop();
        }
        catch (Exception e)
        {
            print("Error with Gyro server: " + e.Message);
        }
    }

    void Connection()
    {
        try
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[client.ReceiveBufferSize];
            int bytesRead = stream.Read(buffer, 0, client.ReceiveBufferSize);

            string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            // print("Data: " + data);
            print("Gyro received: " + dataReceived);

            lock (queueLock){
                scoreQueue.Enqueue(dataReceived);
            }

            // Send a response back to the client
            string response = "Acknowledged";
            byte[] responseBytes = Encoding.UTF8.GetBytes(response);
            stream.Write(responseBytes, 0, responseBytes.Length);

        }
        catch (Exception e)
        {
            print("Error reading incoming messages: " + e);
        }
    }
    void Update(){ 
        
        lock(queueLock){
            while (scoreQueue.Count > 0){
                String incomingData = scoreQueue.Dequeue().ToString();

                if(incomingData == "1" && intScore < 20){
                    intScore += 1;
                    tmpText.text = "Score:" + intScore.ToString();
                } else{
                    hrWarn.SetActive(true);
                }
            }
        }

    }

    // private struct Score{
    //     public int queue;
    // }
}
