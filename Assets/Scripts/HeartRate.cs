using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using System.Collections;

public class HeartRate : MonoBehaviour
{
    Thread thread;
     public int connectionPort = 25004;
    TcpListener server;
    bool running;
    public GameObject heartWarn;
    private int heartRate = 0;
    TcpClient client;
    Queue heartQueue = new Queue();
    private object queueLock = new object();

    void Start(){
        StartServer();
    }
    void StartServer(){
         if (!running)
        {
            running = true;
            thread = new Thread(GetData);
            thread.Start();
            heartWarn.SetActive(false);
        }
    }
    void OnDestroy(){
        StopServer();
    }
    void StopServer(){
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
    void GetData(){
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
            print("Error with HR server: " + e.Message);
        }
    }
    void Connection(){
        try
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[client.ReceiveBufferSize];
            int bytesRead = stream.Read(buffer, 0, client.ReceiveBufferSize);

            string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            // print("Data: " + data);
            print("HR received: " + dataReceived);

            lock (queueLock){
                heartQueue.Enqueue(dataReceived);
            }

            // Send a response back to the client
            // string response = "Acknowledged";
            // byte[] responseBytes = Encoding.UTF8.GetBytes(response);
            // stream.Write(responseBytes, 0, responseBytes.Length);

        }
        catch (Exception e)
        {
            print("Error reading incoming messages: " + e);
        }
    }
    void Update(){
        lock(queueLock){
            while (heartQueue.Count > 0){
                String incomingData = heartQueue.Dequeue().ToString();
                heartRate = Int32.Parse(incomingData);
                if(heartRate >= 150){
                    heartWarn.SetActive(true);
                }
            }
        }
    }

    public void HRResume(){
        heartWarn.SetActive(false);
        Time.timeScale = 1f;
    }
}
