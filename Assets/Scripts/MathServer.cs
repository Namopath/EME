using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

public class MathServer : MonoBehaviour
{
    Thread thread;
    public int connectionPort = 25003;
    TcpListener server;
    bool running;
    public GameObject question;
    public GameObject ans1;
    public GameObject ans2;
    public GameObject ans3;
    public GameObject scoreObj;
    private TMP_Text scoreText;
    int intScore = 0;
    private TMP_Text questionTxt;
    private TMP_Text ans1Text;
    private TMP_Text ans2Text;
    private TMP_Text ans3Text;
    TcpClient client;
    private object queueLock = new object();
    private bool isRunning = false;
    Queue selectedChoice = new Queue();
    private bool canProcess = false;
    private int num1;
    private int num2;
    System.Random rnd = new System.Random();
    private List<String> ops = new List<String> {"+", "-", "*", "/"};
    private String prevMsg = "Null";
    private int correctAns = 1;
    String operation = "+";
    int result;
    int numAns1;
    int numAns2;
    int numAns3;
    private bool changingQuestion = false;
    int qtype;
    // void Awake(){
    //     Debug.unityLogger.logEnabled = false;
    // }
    void Start(){
        StartServer();
        scoreText = scoreObj.GetComponentInChildren<TMP_Text>();
        correctAns = rnd.Next(1,3);
        if(question != null){
             try{
                        questionTxt = question.GetComponent<TMP_Text>();
                        questionTxt.text = ChangeQuestion();
                } catch(Exception e){
                        print("Error fetching text for question: " + e);
                }
        }
        if(ans1 != null && ans2 != null && ans3 != null){
             try{
                        ans1Text = ans1.GetComponent<TMP_Text>();
                        ans2Text = ans2.GetComponent<TMP_Text>();
                        ans3Text = ans3.GetComponent<TMP_Text>();
                        ChangeAns(operation);
                } catch(Exception e){
                        print("Error fetching text for question: " + e);
                }        
            }
        print("Math server started");
    }

    void Update(){
        lock(queueLock){
            while(selectedChoice.Count > 0){
                String IncomingData = selectedChoice.Dequeue().ToString();
                // print("Math incoming: " + IncomingData);
                if (IncomingData == "C1"){
                    if(prevMsg == "N" && correctAns == 1){
                        ans1Text.text = "Correct";
                        intScore += 1;
                        scoreText.text = "Score: " + intScore;
                        StartCoroutine(ChangeQuestionAfterDelay());
                    }
                    if(prevMsg == "N" && correctAns != 1){
                        ans1Text.text = "Incorrect";
                    } 
                    prevMsg = IncomingData;
                }
                if (IncomingData == "C2"){
                    if(prevMsg == "N" && correctAns == 2){
                        ans2Text.text = "Correct";
                        intScore += 1;
                        scoreText.text = "Score: " + intScore;
                        StartCoroutine(ChangeQuestionAfterDelay());
                    }
                    if(prevMsg == "N" && correctAns != 2){
                        ans2Text.text = "Incorrect";
                    }
                    prevMsg = IncomingData;
                }
                if (IncomingData == "C3"){
                    if(prevMsg == "N" && correctAns == 3){
                        ans3Text.text = "Correct";
                        intScore += 1;
                        scoreText.text = "Score: " + intScore;
                        StartCoroutine(ChangeQuestionAfterDelay());
                    }
                    if(prevMsg == "N" && correctAns != 3){
                        ans3Text.text = "Incorrect";
                    }
                    prevMsg = IncomingData;
                }
                prevMsg = IncomingData;
                print("Previous Message: " + prevMsg);
            }
        }
    }

    IEnumerator ChangeQuestionAfterDelay(){
        changingQuestion = true;
        yield return new WaitForSeconds(2f);

        questionTxt.text = ChangeQuestion();
        correctAns = rnd.Next(1,4);
        ChangeAns(operation);

        changingQuestion = false;
    }

    void StartServer(){
        if (!isRunning)
                {
                    isRunning = true;
                    thread = new Thread(GetData);
                    thread.Start();
                }
    }

    void StopServer(){
    server.Stop();
    if (isRunning){
        isRunning = false;
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
            // print("Serv")

            client = server.AcceptTcpClient();
            // client.ReceiveTimeout = 1000;
            running = true;

            SendInit("Math");

            while(client.Connected){
                Connection();
            }

            server.Stop();

        }catch(Exception e){
            print("Error: "+ e);
        }
    }

    void Connection(){
        try{
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[client.ReceiveBufferSize];
            int bytesRead = stream.Read(buffer, 0, client.ReceiveBufferSize);

            string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            lock(queueLock){
                selectedChoice.Enqueue(dataReceived);
            }
        } catch(Exception e){
            print("Error reading incoming messages: " + e);
        }
    }

    void OnDestroy(){
        StopServer();
    }

    void OnDisable(){
        StopServer();
    }

    void SendInit(String message){
        try {
            if(client != null && client.Connected){
                NetworkStream stream = client.GetStream();
                byte[] dataToSend = Encoding.UTF8.GetBytes(message);
                stream.Write(dataToSend, 0, dataToSend.Length);
                stream.Flush();
                print("Initial mode message sent");
            }
        } catch (Exception e){
            print("Error sending initial msg: " + e);
        }
    }
    
    String ChangeQuestion(){
        num1 = rnd.Next(0, 100);
        num2 = rnd.Next(0, 100);
        int index = rnd.Next(0,3);
        operation = ops[index];
        String q = num1.ToString() + operation + num2.ToString();
        return q;
    }
    void ChangeAns(String operation){
        switch (operation)
        {
            case "+":
                result = num1 + num2;
                break;
            case "-":
                result = num1 - num2;
                break;
            case "*":
                result = num1 * num2;
                break;
            default:
                Console.WriteLine("Unknown operation.");
                break;
        }
        
        if(correctAns == 1){
            ans1Text.text = result.ToString();
            numAns2 = rnd.Next(result-5, result+5);
            numAns3 = rnd.Next(result-5, result+5);
            ans2Text.text = numAns2.ToString();
            ans3Text.text = numAns3.ToString();
        }
        if(correctAns == 2){
            ans2Text.text = result.ToString();
            numAns1 = rnd.Next(result-5, result+5);
            numAns3 = rnd.Next(result-5, result+5);
            ans1Text.text = numAns1.ToString();
            ans3Text.text = numAns3.ToString();
        }
        if(correctAns == 3){
            ans3Text.text = result.ToString();
            numAns1 = rnd.Next(result-5, result+5);
            numAns2 = rnd.Next(result-5, result+5);
            ans1Text.text = numAns1.ToString();
            ans2Text.text = numAns2.ToString();
        }
    }

}
