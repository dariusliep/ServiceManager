using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ServicesOperations
{
    public static class ServiceHelper
    {
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern Boolean ChangeServiceConfig(
            IntPtr hService,
            UInt32 nServiceType,
            UInt32 nStartType,
            UInt32 nErrorControl,
            String lpBinaryPathName,
            String lpLoadOrderGroup,
            IntPtr lpdwTagId,
            [In] char[] lpDependencies,
            String lpServiceStartName,
            String lpPassword,
            String lpDisplayName);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int QueryServiceConfig(IntPtr service, IntPtr queryServiceConfig, int bufferSize, ref int bytesNeeded);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "QueryServiceConfig2W")]
        public static extern Boolean QueryServiceConfig2(IntPtr hService, UInt32 dwInfoLevel, IntPtr buffer, UInt32 cbBufSize, out UInt32 pcbBytesNeeded);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, uint dwDesiredAccess);

        [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr OpenSCManager(string machineName, string databaseName, uint dwAccess);

        [DllImport("advapi32.dll", EntryPoint = "CloseServiceHandle")]
        public static extern int CloseServiceHandle(IntPtr hSCObject);

        [DllImport("kernel32.dll")]
        static extern uint GetLastError();

        private const uint SERVICE_NO_CHANGE = 0xFFFFFFFF;
        private const uint SERVICE_QUERY_CONFIG = 0x00000001;
        private const uint SERVICE_CHANGE_CONFIG = 0x00000002;
        private const uint SC_MANAGER_ALL_ACCESS = 0x000F003F;

        private const UInt32 SERVICE_CONFIG_DESCRIPTION = 0x01;

        public static void ChangeStartMode(ServiceController svc, ServiceStartMode mode)
        {
            var scManagerHandle = OpenSCManager(null, null, SC_MANAGER_ALL_ACCESS);
            if (scManagerHandle == IntPtr.Zero)
            {
                throw new ExternalException("Open Service Manager Error");
            }

            var serviceHandle = OpenService(
                scManagerHandle,
                svc.ServiceName,
                SERVICE_QUERY_CONFIG | SERVICE_CHANGE_CONFIG);

            if (serviceHandle == IntPtr.Zero)
            {
                throw new ExternalException("Open Service Error");
            }

            var result = ChangeServiceConfig(
                serviceHandle,
                SERVICE_NO_CHANGE,
                (uint)mode,
                SERVICE_NO_CHANGE,
                null,
                null,
                IntPtr.Zero,
                null,
                null,
                null,
                null);

            if (result == false)
            {
                int nError = Marshal.GetLastWin32Error();
                var win32Exception = new Win32Exception(nError);
                throw new ExternalException("Could not change service start type: "
                    + win32Exception.Message);
            }

            CloseServiceHandle(serviceHandle);
            CloseServiceHandle(scManagerHandle);
        }

        public struct ServiceInfo
        {
            public int serviceType;
            public int startType;
            public int errorControl;
            public string binaryPathName;
            public string loadOrderGroup;
            public int tagID;
            public string dependencies;
            public string startName;
            public string displayName;
            public string serviceName;
            public string description;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct QueryServiceConfigStruct
        {
            public int serviceType;
            public int startType;
            public int errorControl;
            public IntPtr binaryPathName;
            public IntPtr loadOrderGroup;
            public int tagID;
            public IntPtr dependencies;
            public IntPtr startName;
            public IntPtr displayName;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class SERVICE_DESCRIPTION
        {
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
            public String lpDescription;
        }

        public static List<ServiceInfo> GetServicesInfo(List<string> services)
        {
            if (services.Count == 0 )
                throw new NullReferenceException("At least one service must be queried");
            IntPtr scManager = OpenSCManager(null, null, SERVICE_QUERY_CONFIG);
            if (scManager == IntPtr.Zero)
            {
                throw new Win32Exception();
            }

            List<ServiceInfo> servicesInfo = new List<ServiceInfo>();

            foreach (var serviceName in services)
            {
                IntPtr service = OpenService(scManager, serviceName, SERVICE_QUERY_CONFIG);
                if (service.ToInt32() <= 0)
                    throw new NullReferenceException();

                int bytesNeeded = 5;
                QueryServiceConfigStruct qscs = new QueryServiceConfigStruct();
                IntPtr qscPtr = Marshal.AllocCoTaskMem(0);

                int retCode = QueryServiceConfig(service, qscPtr, 0, ref bytesNeeded);
                if (retCode == 0 && bytesNeeded == 0)
                {
                    throw new Win32Exception();
                }
                else
                {
                    qscPtr = Marshal.AllocCoTaskMem(bytesNeeded);
                    retCode = QueryServiceConfig(service, qscPtr, bytesNeeded, ref bytesNeeded);
                    if (retCode == 0)
                    {
                        throw new Win32Exception();
                    }
                    qscs.binaryPathName = IntPtr.Zero;
                    qscs.dependencies = IntPtr.Zero;
                    qscs.displayName = IntPtr.Zero;
                    qscs.loadOrderGroup = IntPtr.Zero;
                    qscs.startName = IntPtr.Zero;

                    qscs = (QueryServiceConfigStruct)
                    Marshal.PtrToStructure(qscPtr,
                    new QueryServiceConfigStruct().GetType());
                }

                ServiceInfo serviceInfo = new ServiceInfo();
                serviceInfo.binaryPathName = Marshal.PtrToStringAuto(qscs.binaryPathName);
                serviceInfo.dependencies = Marshal.PtrToStringAuto(qscs.dependencies);
                serviceInfo.displayName = Marshal.PtrToStringAuto(qscs.displayName);
                serviceInfo.loadOrderGroup = Marshal.PtrToStringAuto(qscs.loadOrderGroup);
                serviceInfo.startName = Marshal.PtrToStringAuto(qscs.startName);

                serviceInfo.errorControl = qscs.errorControl;
                serviceInfo.serviceType = qscs.serviceType;
                serviceInfo.startType = qscs.startType;
                serviceInfo.tagID = qscs.tagID;
                serviceInfo.serviceName = serviceName;

                Marshal.FreeCoTaskMem(qscPtr);

                // getting description
                UInt32 dwBytesNeeded;

                // Determine the buffer size needed
                bool sucess = QueryServiceConfig2(service, SERVICE_CONFIG_DESCRIPTION, IntPtr.Zero, 0, out dwBytesNeeded);

                IntPtr ptr = Marshal.AllocHGlobal((int)dwBytesNeeded);
                sucess = QueryServiceConfig2(service, SERVICE_CONFIG_DESCRIPTION, ptr, dwBytesNeeded, out dwBytesNeeded);
                SERVICE_DESCRIPTION descriptionStruct = new SERVICE_DESCRIPTION();
                Marshal.PtrToStructure(ptr, descriptionStruct);
                Marshal.FreeHGlobal(ptr);
                serviceInfo.description = descriptionStruct.lpDescription;

                servicesInfo.Add(serviceInfo);

            }
            return servicesInfo;
        }
    }
}
