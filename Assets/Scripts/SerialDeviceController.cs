using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Text;
using UnityEngine.Events;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;

public class SerialDeviceController : MonoBehaviour
{
    #region Definition/Settings/Variables

    // --- Definition ---
    [System.Serializable]
    public struct ValueListener
    {
        [Header("Key/Value")]
        public string label;

        [Utils.ReadOnly]
        public float value;

        [Header("Formating")]
        public float valueOffset;
        public Vector2Int deadZoneRange;
        public float normalizeFactor;

        [Header("Events")]
        public Events.EventWithFloat onUpdate;

        [Header("Info")]
        [Utils.ReadOnly]
        public long lastUpdateTick;
    }

    // --- Settings ---
    public string comPortString = "COM7";
    public int frequence = 9600;
    public int threadTimeout = 10;
    public int processReadQueueInterval = 5;
    public int processWriteQueueInterval = 5;
    public bool debug = false;

    public ValueListener[] valueListeners;

    // --- Variables ---
    private SerialPort sp;
    private Thread serialReadThread;
    private ConcurrentQueue<string> serialPortReadQueue = new ConcurrentQueue<string>();
    private ConcurrentQueue<string> serialPortWriteQueue = new ConcurrentQueue<string>();

    private long lastSystemTimeTick;

    private string lastWriteEntry = "";

    // --- Properties ---
    public int SerialPortReadQueueSize => serialPortReadQueue.Count;
    public int SerialPortWriteQueueSize => serialPortWriteQueue.Count;

    public ValueListener[] ValueListeners => valueListeners;

    #endregion

    #region Init/Deinit

    private void Start()
    {
        // Open Serial Port
        Open();
    }

    private void OnApplicationQuit()
    {
        // Close Serial Port
        Close();
    }

    #endregion

    #region Loop

    private void FixedUpdate()
    {
        if(serialPortReadQueue.Count > 0)
        {
            for(int i = 0; i < processReadQueueInterval; i++)
            {
                if(serialPortReadQueue.TryDequeue(out string output))
                    UpdateValues(output);

                if(serialPortReadQueue.Count == 0)
                    break;
            }
        }
        
        lastSystemTimeTick = DateTime.Now.Ticks;
    }

    #endregion

    #region Serial Port

    public void Open()
    {
        // Open
        sp = new SerialPort(comPortString, frequence, Parity.None, 8, StopBits.One);
        sp.ReadTimeout = threadTimeout;
        sp.Open();

        if(sp.IsOpen)
            Debug.Log("Serial port is open!");

        // Start Sync Read
        serialReadThread = new Thread(SerialPortThread);
        serialReadThread.Name = name;
        serialReadThread.Start();
    }

    public void Close()
    {
        sp.Close();

        serialReadThread.Interrupt();
        serialReadThread.Join();
    }

    #endregion

    #region Threading

    private void SerialPortThread()
    {
        do
        {
            // --- Read ---
            ReadThread();

            // --- Write ---
            WriteThread();

            Thread.Sleep(threadTimeout);
        }
        while(sp.IsOpen);
    }

    private void ReadThread()
    {
        string dataString = null;

        try
        {
            dataString = sp.ReadLine();
        }
        catch(TimeoutException)
        {
            dataString = null;
        }

        if(!string.IsNullOrEmpty(dataString))
            serialPortReadQueue.Enqueue(dataString);
    }

    private void WriteThread()
    {
        if(serialPortWriteQueue.Count == 0)
            return;

        for(int i = 0; i < processWriteQueueInterval; i++)
        {
            if(serialPortWriteQueue.TryDequeue(out string output))
            {
                try
                {
                    sp.WriteLine(output);
                }
                catch(TimeoutException)
                {
                    
                }
            }
            
            if(serialPortWriteQueue.Count == 0)
                break;
        }
    }

    #endregion

    #region Read

    private void UpdateValues(string dataString)
    {
        if(FilterValue(dataString, out ValueListener valueListener))
        {
            if(debug)
                Debug.LogFormat("{0}: {1}", valueListener.label, valueListener.value);

            InvokeValueEvent(valueListener);
        }  
    }

    private bool FilterValue(string line, out ValueListener _valueListener)
    {
        _valueListener = new ValueListener();

        if(line.IndexOf(":") < 0)
            return false;
        
        int seperatorIndex = line.IndexOf(':');

        string keyString = line.Substring(0, seperatorIndex);
        string valueString = line.Substring(seperatorIndex + 1, line.Length - seperatorIndex - 1);

        if(!string.IsNullOrEmpty(keyString) && !string.IsNullOrEmpty(valueString))
        {
            // --- Check Listener ---
            if(!FindValueListener(keyString, out int valueListenerIndex))
                return false;

            ValueListener valueListener = valueListeners[valueListenerIndex];

            // --- Filter Value ---
            float value = System.Int32.Parse(valueString);

            // Value Offset
            if(Mathf.Abs(valueListener.valueOffset) > 0)
                value += valueListener.valueOffset;

            if(valueListener.deadZoneRange.y > 0)
            {
                // Deadzone
                if(value < valueListener.deadZoneRange.x)
                    value = 0;
                else if(value > valueListener.deadZoneRange.y)
                    value = valueListener.deadZoneRange.y;
            }

            // Normalize
            if(valueListener.normalizeFactor > 0)
                value = (value / valueListener.normalizeFactor);

            // --- Set Value ---
            valueListener.value = value;
            valueListener.lastUpdateTick = DateTime.Now.Ticks;
            _valueListener = valueListener;

            valueListeners[valueListenerIndex] = valueListener;

            return true;
        }

        return false;
    }

    private bool FindValueListener(string label, ref ValueListener valueListener)
    {
        for(int i = 0; i < valueListeners.Length; i++)
        {
            if(label == valueListeners[i].label)
            {
                valueListener = valueListeners[i];
                return true;
            }
        }

        return false;
    }

    private bool FindValueListener(string label, out int index)
    {
        for(int i = 0; i < valueListeners.Length; i++)
        {
            if(label == valueListeners[i].label)
            {
                index = i;
                return true;
            }
        }

        index = -1;
        return false;
    }

    private void InvokeValueEvent(ValueListener valueListener)
    {
        if(valueListener.onUpdate != null)
            valueListener.onUpdate.Invoke(valueListener.value);
    }

    #endregion

    #region Write

    public void WriteValueOnPercentage(string pinLabel)
    {
        ValueListener valueListener = new ValueListener();

        if(FindValueListener(pinLabel, ref valueListener))
            WriteValue(1, valueListener.value * 255f);
    }

    public void WriteValue(int index, float value)
    {
        WriteValue(string.Format("OUT:{0}:{1}", index, value));
    }

    public void WriteValue(string keyValuePair)
    {
        if(!sp.IsOpen)
            return;

        if(string.IsNullOrEmpty(keyValuePair))
            return;

        if(debug)
            Debug.Log("WriteValue: " + keyValuePair);

        serialPortWriteQueue.Enqueue(keyValuePair);
        lastWriteEntry = keyValuePair;
    }

    #endregion

    #region Utils

    public bool GetFirstReadQueueMessage(out string output)
    {
        return serialPortReadQueue.TryPeek(out output);
    }

    public bool GetFirstWriteQueueMessage(out string output)
    {
        output = lastWriteEntry;
        return true; //;serialPortWriteQueue.TryPeek(out output);
    }

    #endregion
}