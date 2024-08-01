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
// using UnityEditor.PackageManager;
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
    public int qtype = 1;
    int txtIndex;

    public GameObject hrWarn;

    Dictionary<int, string> qDict = new Dictionary<int, string>() {
        {1, "What is the tallest mountain in the world"},
        {2, "Why is seawater salty"},
        {3, "Why do we see rainbows"},
        {4, "Why do leaves change colors during the fall"},
        {5, "How can exercise benefit you?"},
        {6, "How can lack of sleep affect health"},
        {7, "Why is hydration important"},
        {8, "Why is the sky blue"},
    };
    Dictionary<int, string> ansDict = new Dictionary<int, string>() {
        {1, "Mount everest"},
        {2, "Dissolved rocks and minerals in the water"},
        {3, "Refraction of light on raindrops"},
        {4, "Lack of Chlorophyll"},
        {5, "Improves physical health and physique"},
        {6, "Causes damage to the immune and neural system"},
        {7, "Helps balance chemical processes"},
        {8, "Scattering of blue light"},
    };
    Dictionary<int, string> wrongDict1 = new Dictionary<int, string>() {
        {1, "Mont Blanc"},
        {2, "Fish waste products"},
        {3, "Good luck"},
        {4, "Ice and frost"},
        {5, "Damages muscle"},
        {6, "Make you more energetic"},
        {7, "To cure diseases"},
        {8, "Water vapor"},
    };
    Dictionary<int, string> wrongDict2 = new Dictionary<int, string>() {
        {1, "Himalayans"},
        {2, "Volcanic activity"},
        {3, "Sunlight"},
        {4, "Lack of water"},
        {5, "Causes respiratory problems"},
        {6, "Causes damage to the nutrition"},
        {7, "Helps make you pee"},
        {8, "The ocean"},
    };

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
                        if(qtype == 1){
                            questionTxt.text = ChangeMathQuestion();
                        }
                        else if(qtype ==2){
                            questionTxt.text = ChangeTxtQuestion();
                        }
                } catch(Exception e){
                        print("Error fetching text for question: " + e);
                }
        }
        if(ans1 != null && ans2 != null && ans3 != null){
             try{
                        ans1Text = ans1.GetComponent<TMP_Text>();
                        ans2Text = ans2.GetComponent<TMP_Text>();
                        ans3Text = ans3.GetComponent<TMP_Text>();
                        if(qtype == 1){
                            ChangeMathAns(operation);
                        }
                        else if(qtype == 2){
                            ChangeTxtQuestion();
                        }
                        
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
                        if(intScore < 20){
                         intScore += 1;   
                        }else{
                            hrWarn.SetActive(true);
                        }
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
                        if(intScore < 20){
                         intScore += 1;   
                        }else{
                            hrWarn.SetActive(true);
                        }
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
                        if(intScore < 20){
                         intScore += 1;   
                        }else{
                            hrWarn.SetActive(true);
                        }
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

        if(qtype == 1){
            questionTxt.text = ChangeMathQuestion();
            correctAns = rnd.Next(1,4);
            ChangeMathAns(operation); 
        }
        else if(qtype == 2){
            questionTxt.text = ChangeTxtQuestion();
            correctAns = rnd.Next(1,4);
            ChangeTxtAns();
        }
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
    
    String ChangeMathQuestion(){
        num1 = rnd.Next(0, 100);
        num2 = rnd.Next(0, 100);
        int index = rnd.Next(0,3);
        operation = ops[index];
        String q = num1.ToString() + operation + num2.ToString();
        return q;
    }
    void ChangeMathAns(String operation){
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

    public void ChangeqType(){
        if(qtype == 1){
            qtype = 2;
        }
        else if(qtype == 2){
            qtype = 1;
        }
    }

    String ChangeTxtQuestion(){
        txtIndex = rnd.Next(1,8);
        String q = qDict[txtIndex];
        return q;
    }
    
    void ChangeTxtAns(){
        if(correctAns == 1){
            ans1Text.text = ansDict[txtIndex];
            ans2Text.text = wrongDict1[txtIndex];
            ans3Text.text = wrongDict2[txtIndex];
        }
        if(correctAns == 2){
            ans2Text.text = ansDict[txtIndex];
            ans1Text.text = wrongDict1[txtIndex];
            ans3Text.text = wrongDict2[txtIndex];
        }
        if(correctAns == 3){
            ans3Text.text = ansDict[txtIndex];
            ans1Text.text = wrongDict1[txtIndex];
            ans2Text.text = wrongDict2[txtIndex];
        }
    }
}
