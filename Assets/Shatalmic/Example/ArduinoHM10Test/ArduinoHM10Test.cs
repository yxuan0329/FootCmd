/* This is an example to show how to connect to 2 HM-10 devices
 * that are connected together via their serial pins and send data
 * back and forth between them.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

public class ArduinoHM10Test : MonoBehaviour
{
	public string BLEMessage; // get data through bluetooth connection
	public string DeviceName = "Adafruit Bluefruit LE";
	public string ServiceUUID = "FFE0";
	public string Characteristic = "FFE1";

	public Text HM10_Status;
	public Text BluetoothStatus;

    Thread myThread;
	bool startThread = false;

	public enum States
	{
		None,
		Scan,
		Connect,
		RequestMTU,
		Subscribe,
		Unsubscribe,
		Disconnect,
		Communication,
	}

	private bool _workingFoundDevice = true;
	public bool _connected = false;
	private float _timeout = 0f;
	public States _state = States.None;
	private bool _foundID = false;
	private string _hm10; // this is our hm10 device

	public void ToUnSubscribe()
	{
		SetState (States.Unsubscribe, 0.1f);
	}

	void Reset ()
	{
		_workingFoundDevice = false;	// used to guard against trying to connect to a second device while still connecting to the first
		_connected = false;
		_timeout = 0f;
		_state = States.None;
		_foundID = false;
		_hm10 = null;
	}

	void SetState (States newState, float timeout)
	{
		_state = newState;
		_timeout = timeout;
	}

	public void StartProcess (bool isInitial = true)
	{
		BluetoothStatus.text = "Initializing...";

		Reset ();

		if(isInitial)
		{
			BluetoothLEHardwareInterface.Initialize (true, false, () => {
				
				SetState (States.Scan, 0.1f);
				BluetoothStatus.text = "Initialized";

			}, (error) => {
				
				BluetoothLEHardwareInterface.Log ("Error: " + error);
			});
		}
		else
		{
			SetState (States.Scan, 0.1f);
			BluetoothStatus.text = "Initialized";
		}
	}

	// Use this for initialization
	void Start ()
	{
		HM10_Status.text = "";
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (_timeout > 0f)
		{
			_timeout -= Time.deltaTime;
			if (_timeout <= 0f)
			{
				_timeout = 0f;

				switch (_state)
				{
				case States.None:
					break;

				case States.Scan:
					BluetoothStatus.text = "Scanning for Adafruit devices...";

					BluetoothLEHardwareInterface.ScanForPeripheralsWithServices (null, (address, name) => {

						if (name.Contains (DeviceName))
						{
							_workingFoundDevice = true;
							BluetoothLEHardwareInterface.StopScan ();
							BluetoothStatus.text = "";
							_hm10 = address; // add it to the list and set to connect to it

							HM10_Status.text = "Found Adafruit";

							SetState (States.Connect, 0.5f);

							_workingFoundDevice = false;
						}

					}, null, false, false);
					break;

				case States.Connect:
					// set these flags
					_foundID = false;

					HM10_Status.text = "Connecting to Adafruit";

					// note that the first parameter is the address, not the name. I have not fixed this because
					// of backwards compatiblity.
					// also note that I am note using the first 2 callbacks. If you are not looking for specific characteristics you can use one of
					// the first 2, but keep in mind that the device will enumerate everything and so you will want to have a timeout
					// large enough that it will be finished enumerating before you try to subscribe or do any other operations.
					BluetoothLEHardwareInterface.ConnectToPeripheral (_hm10, null, null, (address, serviceUUID, characteristicUUID) => {
						
						_connected = true;
						HM10_Status.text = "Connected to Adafruit";
						SetState (States.RequestMTU, 2f);
					}, (disconnectedAddress) => {
						BluetoothLEHardwareInterface.Log ("Device disconnected: " + disconnectedAddress);
						HM10_Status.text = "Disconnected";
					});
					break;

					case States.RequestMTU:
						HM10_Status.text = "Requesting MTU";

						BluetoothLEHardwareInterface.RequestMtu(_hm10, 185, (address, newMTU) =>
						{
							HM10_Status.text = "MTU set to " + newMTU.ToString();

							SetState(States.Subscribe, 0.1f);
						});
						break;

					case States.Subscribe:

						if(!startThread)
						{
							HM10_Status.text = "Subscribing to Adafruit";
							startThread = true;
						}

						BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress (_hm10, ServiceUUID, Characteristic, null, (address, characteristicUUID, bytes) => {
							BLEMessage = Encoding.UTF8.GetString (bytes);
							HM10_Status.text = "Received Serial: " + BLEMessage;
						});

						break;
					
					case States.Communication:
						BluetoothLEHardwareInterface.ReadCharacteristic (_hm10, ServiceUUID, Characteristic, (characteristicUUID, bytes) => {
							HM10_Status.text = "Get Message: " + Encoding.UTF8.GetString (bytes);
						});

						break;

				case States.Unsubscribe:
					BluetoothLEHardwareInterface.UnSubscribeCharacteristic (_hm10, ServiceUUID, Characteristic, null);
					SetState (States.Disconnect, 1f);
					break;

				case States.Disconnect:
					if (_connected)
					{
						BluetoothLEHardwareInterface.DisconnectPeripheral (_hm10, (address) => {
							BluetoothLEHardwareInterface.DeInitialize (() => {
								
								_connected = false;
								_state = States.None;
							});
						});
					}
					else
					{
						BluetoothLEHardwareInterface.DeInitialize (() => {
							
							_state = States.None;
						});
					}
					
					break;
				}
			}
		}
	}

	string FullUUID (string uuid)
	{
		return "0000" + uuid + "-0000-1000-8000-00805F9B34FB";
	}
	
	bool IsEqual(string uuid1, string uuid2)
	{
		if (uuid1.Length == 4)
			uuid1 = FullUUID (uuid1);
		if (uuid2.Length == 4)
			uuid2 = FullUUID (uuid2);

		return (uuid1.ToUpper().Equals(uuid2.ToUpper()));
	}

	void SendString(string value)
	{
		var data = Encoding.UTF8.GetBytes (value);
		// notice that the 6th parameter is false. this is because the HM10 doesn't support withResponse writing to its characteristic.
		// some devices do support this setting and it is prefered when they do so that you can know for sure the data was received by 
		// the device
		BluetoothLEHardwareInterface.WriteCharacteristic (_hm10, ServiceUUID, Characteristic, data, data.Length, false, (characteristicUUID) => {
			HM10_Status.text = "Write String Succeeded";
			BluetoothLEHardwareInterface.Log ("Write Succeeded");
		});
	}

	void SendByte (byte value)
	{
		byte[] data = new byte[] { value };
		// notice that the 6th parameter is false. this is because the HM10 doesn't support withResponse writing to its characteristic.
		// some devices do support this setting and it is prefered when they do so that you can know for sure the data was received by 
		// the device
		BluetoothLEHardwareInterface.WriteCharacteristic (_hm10, ServiceUUID, Characteristic, data, data.Length, false, (characteristicUUID) => {
			HM10_Status.text = "Write Byte Succeeded";
			BluetoothLEHardwareInterface.Log ("Write Succeeded");
		});
	}
}
