using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.WSA;
//#if ENABLE_WINMD_SUPPORT
#if ENABLE_WINMD_SUPPORT
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.UI.Popups;
#endif

public class ConnectBluetooth : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.WSA.Application.InvokeOnUIThread(Connect, true);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    async void Connect()
    {
#if ENABLE_WINMD_SUPPORT
        
        Scan();
        string deviceId = "70:3e:97:ca:07:12-0c:43:14:f4:69:c7";
        var bluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(deviceId);

        if (bluetoothLeDevice != null)
        {
            // Get the GATT services
            var servicesResult = await bluetoothLeDevice.GetGattServicesAsync();
            if (servicesResult.Status == GattCommunicationStatus.Success)
            {
                var services = servicesResult.Services;
                Console.WriteLine("Found GATT services:");
                DebugPanel.bluetoothStatus = "Connected.";
                foreach (var service in services)
                {
                    Console.WriteLine($"Service UUID: {service.Uuid}");

                    // Get the characteristics for each service
                    var characteristicsResult = await service.GetCharacteristicsAsync();
                    if (characteristicsResult.Status == GattCommunicationStatus.Success)
                    {
                        var characteristics = characteristicsResult.Characteristics;
                        foreach (var characteristic in characteristics)
                        {
                            Console.WriteLine($"Characteristic UUID: {characteristic.Uuid}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Failed to get characteristics for service {service.Uuid}");
                    }
                }
            }
            else
            {
                DebugPanel.bluetoothStatus = "Failed to get GATT services";
                Console.WriteLine("Failed to get GATT services.");
            }
        }
        else
        {
            Console.WriteLine("Failed to connect to microcontroller.");
            DebugPanel.bluetoothStatus = "Failed to connect to microcontroller.";
        }
#else
        Debug.Log("UWP APIs not supported!");
        DebugPanel.bluetoothStatus = "UWP APIs not supported!";
#endif
    }
#if ENABLE_WINMD_SUPPORT
    async void Scan()
    {
        string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" };

        DeviceWatcher deviceWatcher =
                    DeviceInformation.CreateWatcher(
                            BluetoothLEDevice.GetDeviceSelectorFromPairingState(false),
                            requestedProperties,
                            DeviceInformationKind.AssociationEndpoint);

        // Register event handlers before starting the watcher.
        // Added, Updated and Removed are required to get all nearby devices
        deviceWatcher.Added += DeviceWatcher_Added;
        deviceWatcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
        deviceWatcher.Stopped += DeviceWatcher_Stopped;

        // Start the watcher.
        
        try
        {
            DebugPanel.scanStatus = "Starting Scan";
            deviceWatcher.Start();
        }
        catch (Exception ex)
        {
            DebugPanel.scanStatus = ex.Message;
            // Handle the error accordingly
        }

    }

    /*async void CheckBluetoothPermissions()
    {
        var accessStatus = new BluetoothDevice();
        if (accessStatus == null)
        {
            // Bluetooth is not available on this device
            DebugPanel.bluetoothStatus = "Bluetooth not available.";
            return;
        }
        if (accessStatus != BluetoothAccessStatus.Allowed)
        {
            DebugPanel.bluetoothStatus = "Bluetooth access denied.";
            var dialog = new MessageDialog("Bluetooth access is required for this app to function properly. Please enable Bluetooth access in the Settings.");
            await dialog.ShowAsync();
        }
    }*/

    void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
    {
        DebugPanel.foundDevices += args.Name + "," + args.Id + "---";
    }

    void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args)
    {
        DebugPanel.scanStatus = "Enumeration Complete";
    }

    void DeviceWatcher_Stopped(DeviceWatcher sender, object args)
    {
        DebugPanel.scanStatus = "Scan Stopped";
        // You may handle any cleanup tasks here
    }
#endif

}
