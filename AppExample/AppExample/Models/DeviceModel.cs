using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppExample.Models
{
    public class DeviceModel : IDevice
    {
        public Guid Id => throw new NotImplementedException();

        public string Name => throw new NotImplementedException();

        public int Rssi => throw new NotImplementedException();

        public object NativeDevice => throw new NotImplementedException();

        public DeviceState State => throw new NotImplementedException();

        public IList<AdvertisementRecord> AdvertisementRecords => throw new NotImplementedException();

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<IService> GetServiceAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<IService>> GetServicesAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> RequestMtuAsync(int requestValue)
        {
            throw new NotImplementedException();
        }

        public bool UpdateConnectionInterval(ConnectionInterval interval)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateRssiAsync()
        {
            throw new NotImplementedException();
        }
    }
}
