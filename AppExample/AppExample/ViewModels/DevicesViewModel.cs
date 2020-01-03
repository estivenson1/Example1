using AppExample.Models;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AppExample.ViewModels
{
    public class DevicesViewModel : ViewModelBase
    {
        #region Attributes

        IAdapter _adapter;
        IBluetoothLE _ble;


        public ICharacteristic _characteristic { get; private set; }
        private readonly IDevice _connectedDevice;
        #endregion


        #region Properties

        SelectionMode _selectionMode;
        public SelectionMode SelectionMode
        {
            get => _selectionMode;
            set
            {
                _selectionMode = value;
                OnPropertyChanged("SelectionMode");
            }
        }

        bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                _isRefreshing = value;
                OnPropertyChanged("IsRefreshing");
            }
        }

        ObservableCollection<IDevice> _deviceCollection = new ObservableCollection<IDevice>();
        public ObservableCollection<IDevice> DeviceCollection
        {
            get => _deviceCollection;
            set
            {
                _deviceCollection = value;
                OnPropertyChanged("DeviceCollection");
            }
        }



        IDevice _device;
        public IDevice DeviceCurrent
        {
            get => _device;
            set
            {
                _device = value;
                OnPropertyChanged("DeviceCurrent");
            }
        }

        IService _iService;
        public IService ServiceCurrent
        {
            get => _iService;
            set
            {
                _iService = value;
                OnPropertyChanged("ServiceCurrent");
            }
        }


        ICharacteristic _iCharacteristic;
        public ICharacteristic CharacteristicCurrent
        {
            get => _iCharacteristic;
            set
            {
                _iCharacteristic = value;
                OnPropertyChanged("CharacteristicCurrent");
            }
        }

        //private Guid _previousGuid;  
        //public Guid PreviousGuid
        //{
        //    get => _previousGuid;
        //    set
        //    {
        //        _previousGuid = value;
        //        _settings.AddOrUpdateValue("lastguid", _previousGuid.ToString());
        //        RaisePropertyChanged();
        //        RaisePropertyChanged(() => ConnectToPreviousCommand);
        //    }
        //}
        #endregion


        #region Constructors
        public DevicesViewModel(Page page) : base(page)
        {
            Title = "Dispositivos ";

            _ble = CrossBluetoothLE.Current;
            _adapter = CrossBluetoothLE.Current.Adapter;
            SelectionMode = SelectionMode.None;

            Debug.WriteLine("Scan Devices...");

            //También puede escuchar los cambios de estado. Para que pueda reaccionar si el usuario enciende / apaga bluetooth en su teléfono inteligente.
            _ble.StateChanged += (s, e) =>
            {

                try
                {

                    switch (_ble.State)
                    {
                        case BluetoothState.On:
                            page.DisplayAlert("Bluetooth Activado ", "Escaneando...", "OK");
                            ScanDevicesCommand.Execute(null);


                            break;
                        case BluetoothState.Off:
                            page.DisplayAlert("Bluetooth Desactivado", $"Activa tu bluetooth en configuraciones", "OK");
                            //DeviceCollection.Clear();
                            SelectionMode = SelectionMode.None;

                            break;
                    }

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }


            };

            page.Appearing += Page_Appearing;

        }
        #endregion


        #region Methods	
        private void Page_Appearing(object sender, EventArgs e)
        {
            if (_ble.State == BluetoothState.On)
            {
                ScanDevicesCommand.Execute(null);
            }
            else
            {
                page.DisplayAlert("Bluetooth Desactivado", $"Activa tu bluetooth en configuraciones", "OK");

            }
        }
        #endregion


        #region Commands
        private ICommand _scanDevicesCommand;
        public ICommand ScanDevicesCommand => _scanDevicesCommand ?? (_scanDevicesCommand = new Command(async () =>
        {

            Debug.WriteLine("Scan Devices...");
            IsRefreshing = true;
            SelectionMode = SelectionMode.None;
            try
            {
                _adapter.ScanTimeout = 5000;
                _adapter.ScanMode = ScanMode.Balanced;
                DeviceCollection.Clear();
                _adapter.DeviceDiscovered += (s, a) =>
                {
                    Debug.WriteLine(a.Device);

                    if (!DeviceCollection.Contains(a.Device))
                    {

                        DeviceCollection.Add(a.Device);
                    }

                };

                await _adapter.StartScanningForDevicesAsync();
                SelectionMode = SelectionMode.Single;
                IsRefreshing = false;
                Debug.WriteLine("Scanned");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }));

        private ICommand _deviceSelectedCommand;
        public ICommand DeviceSelectedCommand => _deviceSelectedCommand ?? (_deviceSelectedCommand = new Command(async (deviceselected) =>
       {
           await _adapter.StopScanningForDevicesAsync();


           Debug.WriteLine("Device Selected");
           DeviceCurrent = (deviceselected as IDevice);



            //MemberwiseClone()

            try
           {
               var cancellationToken = new System.Threading.CancellationTokenSource();
               var parameters = new Plugin.BLE.Abstractions.ConnectParameters(forceBleTransport: true);
               await _adapter.ConnectToDeviceAsync(DeviceCurrent, parameters, cancellationToken.Token);



               var services = await _device.GetServicesAsync();
               var Service = await _device.GetServiceAsync(DeviceCurrent.Id);


               List<ICharacteristic> Characteristics = new List<ICharacteristic>();

               foreach (var item in services)
               {
                   var cara = await item.GetCharacteristicsAsync();

                   Characteristics.AddRange(cara);

                    //if (_uuid.ToString() == "e7810a71-73ae-499d-8c15-faa9aef0c3f2")
                    //{

                    string str = item.Id.ToString();
               }


               this._characteristic = Characteristics.Where(x => x.Uuid == "bef8d6c9-9c21-4c9e-b632-bd58c1009f9f").FirstOrDefault();

               if (_characteristic.CanWrite)
               {
                   byte[] bytes = Encoding.UTF8.GetBytes("Hola Mundo despues de un siglo ya se puede imprimir" + System.Environment.NewLine);
                   var cancellationToken2 = new System.Threading.CancellationToken();

                    //_characteristic.WriteType = Plugin.BLE.Abstractions.CharacteristicWriteType.WithoutResponse;

                    //var result =await _characteristic.WriteAsync(bytes, new System.Threading.CancellationToken(true)).ConfigureAwait(false);

                    await _characteristic.WriteAsync(bytes);

                   _characteristic.ValueUpdated += (s, e) =>
                   {
                       Debug.WriteLine("New value: {0}", e.Characteristic.Value);
                   };
                   await _characteristic.StartUpdatesAsync();



               }


                //var characteristic = await service.GetCharacteristicAsync(Guid.Parse("d8de624e-140f-4a22-8594-e2216b84a5f2"));

                //await _adapter.ConnectToDeviceAsync(device);

                await page.DisplayAlert("Conectado", $"Status {DeviceCurrent.State}", "Ok");

                //var service = await device.GetServiceAsync(Guid.Parse("ffe0ecd2-3d16-4f8d-90de-e89e7fc396a5"));

            }
           catch (Exception ex)
           {
               Debug.WriteLine(ex);
           }


       }));

        private ICommand _desconnectCommand;
        public ICommand DesconnectCommand => _desconnectCommand ?? (_desconnectCommand = new Command(async (item) =>
        {
            var device = item as IDevice;

            try
            {

                if (device.State == Plugin.BLE.Abstractions.DeviceState.Disconnected)
                {
                    throw new System.ArgumentException("Dispositivo no puede estar desconectado", "devicel.state");

                }
                await _adapter.DisconnectDeviceAsync(device);
                await page.DisplayAlert("Desconectado", $"Status {device.State}", "Ok");

            }
            catch (Exception ex)
            {
                await page.DisplayAlert("El Dispositivo aun no ha sido conectado", $"Status {device.State}", "Ok");

                Debug.WriteLine(ex);
            }


        }));

        private ICommand _imprimirCommand;
        public ICommand ImprimirCommand => _imprimirCommand ?? (_imprimirCommand = new Command(async () =>
        {

            //var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);

            try
            {
                //System.Threading.CancellationTokenSource tokenSource = new System.Threading.CancellationTokenSource();
                //await _adapter.ConnectToKnownDeviceAsync(Guid.Parse("00000000-0000-0000-0000-dc0d30098222"));
                //var device = await _adapter.ConnectToKnownDeviceAsync(Guid.Parse("00000000-0000-0000-0000-dc0d30098222"), new Plugin.BLE.Abstractions.ConnectParameters(autoConnect: true, forceBleTransport: false), tokenSource.Token);


       


                if (DeviceCurrent == null)
                {
                    var cancellationToken = new System.Threading.CancellationTokenSource();
                    var parameters = new Plugin.BLE.Abstractions.ConnectParameters(forceBleTransport: true);
                    DeviceCurrent = await _adapter.ConnectToKnownDeviceAsync(Guid.Parse("00000000-0000-0000-0000-dc0d30098222"), parameters, cancellationToken.Token);
                    ServiceCurrent = await DeviceCurrent.GetServiceAsync(Guid.Parse("e7810a71-73ae-499d-8c15-faa9aef0c3f2"));
                    CharacteristicCurrent = await ServiceCurrent.GetCharacteristicAsync(Guid.Parse("bef8d6c9-9c21-4c9e-b632-bd58c1009f9f"));
                }
                else if (DeviceCurrent.State == Plugin.BLE.Abstractions.DeviceState.Disconnected)
                {
                    var cancellationToken = new System.Threading.CancellationTokenSource();
                    var parameters = new Plugin.BLE.Abstractions.ConnectParameters(forceBleTransport: true);
                    DeviceCurrent = await _adapter.ConnectToKnownDeviceAsync(Guid.Parse("00000000-0000-0000-0000-dc0d30098222"), parameters, cancellationToken.Token);
                    ServiceCurrent = await DeviceCurrent.GetServiceAsync(Guid.Parse("e7810a71-73ae-499d-8c15-faa9aef0c3f2"));
                    CharacteristicCurrent = await ServiceCurrent.GetCharacteristicAsync(Guid.Parse("bef8d6c9-9c21-4c9e-b632-bd58c1009f9f"));
                }
                else if (ServiceCurrent == null)
                {
                    ServiceCurrent = await DeviceCurrent.GetServiceAsync(Guid.Parse("e7810a71-73ae-499d-8c15-faa9aef0c3f2"));
                    CharacteristicCurrent = await ServiceCurrent.GetCharacteristicAsync(Guid.Parse("bef8d6c9-9c21-4c9e-b632-bd58c1009f9f"));
                }


                byte[] bytes = Encoding.UTF8.GetBytes("Hola Mundo despues de un siglo ya se puede imprimir" + System.Environment.NewLine);

                await CharacteristicCurrent.WriteAsync(bytes);
                //await CharacteristicCurrent.StartUpdatesAsync();


                //MemberwiseClone
            }
            catch (Plugin.BLE.Abstractions.Exceptions.DeviceConnectionException e)
            {
                // ... could not connect to device
                string ms = e.Message;
            }



        }));
        #endregion
    }
}

#region Attributes

#endregion


#region Properties

#endregion


#region Constructors

#endregion


#region Methods	

#endregion


#region Commands

#endregion



//private async void ScanButton_Clicked(object sender, EventArgs e)
//{
//    ScanButton.IsEnabled = false;
//    var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);

//    if (status != PermissionStatus.Granted)
//    {
//        if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
//        {
//            await DisplayAlert("Need location", "App needs location permission", "OK");
//        }

//        var status1 = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Location });

//        var loca = status1.FirstOrDefault(x => x.Key == Permission.Location);
//        if (loca.Value != null)
//            if (loca.Value == PermissionStatus.Granted) status = PermissionStatus.Granted;
//    }

//    if (status != PermissionStatus.Granted)
//    {
//        await DisplayAlert("Need location", "App need location permission", "OK");
//        return;
//    }

//    _gattDevices.Clear();
//    await _adapter.StartScanningForDevicesAsync();
//    listView.ItemsSource = _gattDevices.ToArray();
//    ScanButton.IsEnabled = true;
//}