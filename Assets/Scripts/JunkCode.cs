// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System.Linq;
// using System.Threading;
// using System.Net;
// using System.Net.Sockets;
// using System.Text;
// using System;

// public class Animation_script : MonoBehaviour
// {
//     Thread thread;
//     public int connectionPort = 25501;
//     TcpListener server;
//     TcpClient client;
//     bool running;
//     public GameObject[] Body;

//     // Flag to indicate if the thread is running
//     private bool isRunning = false;

//     // Start is called before the first frame update
//     void Start()
//     {
//         print("Hello the script is running!");
//         StartServer();
//     }

//     void StartServer()
//     {
//         // Start the thread only if it's not already running
//         if (!isRunning)
//         {
//             // Set the flag to indicate that the thread is running
//             isRunning = true;
//             // Start the thread
//             thread = new Thread(GetData);
//             thread.Start();
//         }
//     }

//     void OnDestroy()
//     {
//         // Ensure that the thread is stopped when the script is destroyed
//         StopServer();
//         print("Server stopped on destroyed");
//     }

//     void OnDisable()
//     {
//         // Ensure that the thread is stopped when the script is disabled
//         StopServer();
//         print("Server stopped on disabled");
//     }

//     void StopServer()
//     {
//         // Stop the thread only if it's running
//         if (isRunning)
//         {
//             // Set the flag to indicate that the thread is not running
//             isRunning = false;
//             // Stop the thread
//             if (thread != null && thread.IsAlive)
//             {
//                 thread.Abort();
//             }
//         }
//     }

//     void GetData()
//     {
//         try
//         {
//             server = new TcpListener(IPAddress.Any, connectionPort);
//             server.Start();

//             client = server.AcceptTcpClient();
//             running = true;

//             while (running)
//             {
//                 Connection();
//             }

//             server.Stop();
//         }
//         catch (Exception e)
//         {
//             Debug.LogError("Exception caught: " + e.Message);
//         }
//     }

//     void Connection()
//     {
//         try
//         {
//             NetworkStream nwStream = client.GetStream();
//             byte[] buffer = new byte[client.ReceiveBufferSize];
//             int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

//             // Decode the bytes into a string
//             string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);

//             // Make sure we're not getting an empty string
//             if (!string.IsNullOrEmpty(dataReceived))
//             {
//                 // Convert the received string of data to the format we are using
//                 string[] points = dataReceived.Split(',');

//                 if (points.Length >= 4) // Ensure at least 4 values are received
//                 {
//                     int index = int.Parse(points[0]);
//                     if (index >= 0 && index < Body.Length)
//                     {
//                         float x = float.Parse(points[1]) / 100;
//                         float y = float.Parse(points[2]) / 100;
//                         float z = float.Parse(points[3]) / 100;

//                         Body[index].transform.localPosition = new Vector3(x, y, z);
//                     }
//                 }
//             }
//         }
//         catch (Exception e)
//         {
//             Debug.LogError("Exception caught during connection: " + e.Message);
//         }
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         // No need for this anymore
//     }
// }


//OG txt code below:

// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System.Linq;
// using System.Threading;
// using System.Net;
// using System.Net.Sockets;
// using System.Text;

// public class Animation_script : MonoBehaviour
// {
//     Thread thread;
//     public int connectionPort = 25501;
//     TcpListener server;
//     TcpClient client;
//     bool running;
//     public GameObject[] Body;
//     List<string> lines;
//     int counter = 0;
//     // Start is called before the first frame update
//     void Start()
//     {
//         lines = System.IO.File.ReadLines("Assets/Animation.txt").ToList();
//         // ThreadStart ts = new ThreadStart(GetData);
//         // thread = new Thread(ts);
//         // thread.Start();

    
//     }

//     // void GetData(){
//     //     server = new TcpListener(IPAddress.Any, connectionPort);
//     //     server.Start();

//     //     client = server.AcceptTcpClient();
//     //     while (running)
//     //     {
//     //         Connection();
//     //     }
//     //     server.Stop();
//     // }

//     // void Connection(){
//     //     NetworkStream nwStream = client.GetStream();
//     //     byte[] buffer = new byte[client.ReceiveBufferSize];
//     //     int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

//     //     // Decode the bytes into a string
//     //     string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        
//     //     // Make sure we're not getting an empty string
//     //     //dataReceived.Trim();
//     //     if (dataReceived != null && dataReceived != "")
//     //     {
//     //         // Convert the received string of data to the format we are using
//     //         position = ParseData(dataReceived);
//     //         nwStream.Write(buffer, 0, bytesRead);
//     //     }
//     // }

//     // Update is called once per frame
//     void Update()
//     {
//         // print(lines[0]);
//         string[] points = lines[counter].Split(',');

//         for(int i = 0; i <= 32; i++){
//             float x = float.Parse(points[0+(i*3)])/100;
//             float y = float.Parse(points[1+(i*3)])/100;
//             float z = float.Parse(points[2+(i*3)])/100;

//             Body[i].transform.localPosition = new Vector3(x,y,z);
//         }

//         // print(points[0]);
        
//         counter += 1;

//         if (counter == lines.Count){
//             counter = 0;
//         }
//         Thread.Sleep(30);
//     }
// }
