using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using Unity.VisualScripting;

public class Animation_script : MonoBehaviour
{
    Thread thread;
    public int connectionPort = 25501;
    TcpListener server;
    TcpClient client;
    bool running;
    public GameObject[] Body;

    public GameObject RightForearm;
    
    // Flag to indicate if the thread is running
    private bool isRunning = false;

    // Queue to hold position data
    private Queue<PositionData> positionDataQueue = new Queue<PositionData>();

    private Queue<RotationData> rotationDataQueue = new Queue<RotationData>();
    private object queueLock = new object();

    Vector3 originalPosition = new Vector3(-1.496f, 5.683f, 1.613f);

    // Start is called before the first frame update
    void Start()
    {
        print("Hello the script is running!");
        StartServer();
    }

    void StartServer()
    {
        
        if (!isRunning)
        {
            isRunning = true;
            thread = new Thread(GetData);
            thread.Start();
        }
    }

    void OnDestroy()
    {
        // Ensure that the thread is stopped when the script is destroyed
        StopServer();
        print("Server stopped on destroyed");
    }

    void OnDisable()
    {
        // Ensure that the thread is stopped when the script is disabled
        StopServer();
        print("Server stopped on disabled");
    }

    void StopServer()
    {
        if (isRunning)
        {
            isRunning = false;
            if (thread != null && thread.IsAlive)
            {
                thread.Abort();
            }
        }
    }

    void GetData()
    {
        try
        {
            server = new TcpListener(IPAddress.Any, connectionPort);
            server.Start();

            client = server.AcceptTcpClient();
            running = true;

            while (running)
            {
                Connection();
            }

            server.Stop();
        }
        catch (Exception e)
        {
            print("Exception caught: " + e.Message);
        }
    }

    void Connection()
    {
        try
        {
            NetworkStream nwStream = client.GetStream();
            byte[] buffer = new byte[client.ReceiveBufferSize];
            int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

            // Decode the bytes into a string
            string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            // Debug.Log("Received data: " + dataReceived);

            // Make sure we're not getting an empty string
            if (!string.IsNullOrEmpty(dataReceived))
            {
                // Convert the received string of data to the format we are using
                string[] points = dataReceived.Split(',');

                if (points.Length == 4) // Ensure at least 4 values are received
                {
                    PositionData normalizedData = new PositionData
                    {
                        index = int.Parse(points[0]),
                        x = float.Parse(points[1]) / 500,
                        y = float.Parse(points[2]) / 480,
                        z = float.Parse(points[3]) / 400,
                    };

                    PositionData posData = new PositionData{
                        index = int.Parse(points[0]),
                        x = (normalizedData.x * 12) - 6.5f + originalPosition.x,
                        y = (normalizedData.y * 10) - 2f,
                        z = normalizedData.z - 1
                    };

                    lock (queueLock)
                    {
                        positionDataQueue.Enqueue(posData);
                    }
                }
                else if(points.Length <= 2){

                    RotationData rotationData = new RotationData{
                        bone = int.Parse(points[0]),
                        x_Rotation = int.Parse(points[1])
                    };

                    // lock(queueLock){
                    //     rotationDataQueue.Enqueue(rotationData);
                    // }
                } else{
                    return;
                }
            }
        }
        catch (Exception e)
        {
            print("Exception caught during connection: " + e.Message);
        }
    }

    // Update is called once per frame
    void Update()
    {
        lock (queueLock)
        {
            while (positionDataQueue.Count > 0)
            {
                PositionData posData = positionDataQueue.Dequeue();
                if (posData.index >= 0 && posData.index < Body.Length)
                {
                    Body[posData.index].transform.localPosition = new Vector3(posData.x, posData.y, posData.z);
                }
            }

            // while(rotationDataQueue.Count > 0){
            //     RotationData rotationData = rotationDataQueue.Dequeue();
            //      if(RightForearm != null && rotationData.bone == -1){
            //         Body[16].transform.localRotation = Quaternion.Euler(rotationData.x_Rotation,0,0);
            //         print("Rotation applied to bone");
            //      } else{
            //         print("Bone is null");
            //      }
            // }
        }
    }

    // Struct to hold position data
    private struct PositionData
    {
        public int index;
        public float x;
        public float y;
        public float z;
    }
    private struct RotationData{
        public int bone;
        public int x_Rotation;
    }
}

