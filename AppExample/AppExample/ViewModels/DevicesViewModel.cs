using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace AppExample.ViewModels
{
   public class DevicesViewModel : ViewModelBase
    {
        #region Attributes

        IAdapter _adapter;
        IBluetoothLE _ble;
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
            _ble.StateChanged += (s, e) => {

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
        public ICommand ScanDevicesCommand => _scanDevicesCommand ?? (_scanDevicesCommand = new Command(async () => {

            Debug.WriteLine("Scan Devices...");
            IsRefreshing = true;
            SelectionMode = SelectionMode.None;
            try
            {
                _adapter.ScanTimeout = 5000;
                _adapter.ScanMode = ScanMode.Balanced;
                DeviceCollection.Clear();
                _adapter.DeviceDiscovered += (s, a) => {
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
        public ICommand DeviceSelectedCommand => _deviceSelectedCommand ?? (_deviceSelectedCommand = new Command(async (deviceselected) => {
            await _adapter.StopScanningForDevicesAsync();


            Debug.WriteLine("Device Selected");
            var device = deviceselected as IDevice;
            try
            {
                await _adapter.ConnectToDeviceAsync(device);
      
                await page.DisplayAlert("Conectado", $"Status {device.State}", "Ok");

                //var service = await device.GetServiceAsync(Guid.Parse("ffe0ecd2-3d16-4f8d-90de-e89e7fc396a5"));

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }


        }));

        private ICommand _desconnectCommand;
        public ICommand DesconnectCommand => _desconnectCommand ?? (_desconnectCommand = new Command(async (item) => {
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
