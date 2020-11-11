using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;
using PrinterSetup;

namespace PrinterSetup
{
	public class SpoolerHelper
	{
		

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		private struct MONITOR_INFO_2
		{
			public string pName;
			public string pEnvironment;


			public string pDLLName;
		}

		private enum PrinterAccess
		{
			ServerAdmin = 1,
			ServerEnum,
			PrinterAdmin = 4,
			PrinterUse = 8,
			JobAdmin = 16,
			JobRead = 32,
			StandardRightsRequired = 983040,
			PrinterAllAccess = 983052
		}

		private struct PrinterDefaults
		{
			public IntPtr pDataType;

			public IntPtr pDevMode;

			public SpoolerHelper.PrinterAccess DesiredAccess;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct PortData
		{
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
			public string sztPortName;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		private struct DRIVER_INFO_3
		{
			public uint cVersion;

			[MarshalAs(UnmanagedType.LPTStr)]
			public string pName;

			[MarshalAs(UnmanagedType.LPTStr)]
			public string pEnvironment;

			[MarshalAs(UnmanagedType.LPTStr)]
			public string pDriverPath;

			[MarshalAs(UnmanagedType.LPTStr)]
			public string pDataFile;

			[MarshalAs(UnmanagedType.LPTStr)]
			public string pConfigFile;

			[MarshalAs(UnmanagedType.LPTStr)]
			public string pHelpFile;

			[MarshalAs(UnmanagedType.LPTStr)]
			public string pDependentFiles;

			[MarshalAs(UnmanagedType.LPTStr)]
			public string pMonitorName;

			[MarshalAs(UnmanagedType.LPTStr)]
			public string pDefaultDataType;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		private struct PRINTER_INFO_2
		{
			public string pServerName;

			public string pPrinterName;

			public string pShareName;

			public string pPortName;

			public string pDriverName;

			public string pComment;

			public string pLocation;

			public IntPtr pDevMode;

			public string pSepFile;

			public string pPrintProcessor;

			public string pDatatype;

			public string pParameters;

			public IntPtr pSecurityDescriptor;

			public uint Attributes;

			public uint Priority;

			public uint DefaultPriority;

			public uint StartTime;

			public uint UntilTime;

			public uint Status;

			public uint cJobs;

			public uint AveragePPM;
		}

		public class GenericResult
		{
			private string _method;

			public bool Success
			{
				get;
				set;
			}

			public string Message
			{
				get;
				set;
			}

			public Exception Exception
			{
				get;
				set;
			}

			public string Method
			{
				get
				{
					return this._method;
				}
			}

			public GenericResult(string method)
			{
				this.Success = false;
				this.Message = string.Empty;
				this.Exception = null;
				this._method = method;
			}
		}

		private const int MAX_PORTNAME_LEN = 64;

		private const int MAX_NETWORKNAME_LEN = 49;

		private const int MAX_SNMP_COMMUNITY_STR_LEN = 33;

		private const int MAX_QUEUENAME_LEN = 33;

		private const int MAX_IPADDR_STR_LEN = 16;

		private const int RESERVED_BYTE_ARRAY_SIZE = 540;

		[DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int AddMonitor(string pName, uint Level, ref SpoolerHelper.MONITOR_INFO_2 pMonitors);

		[DllImport("winspool.drv", SetLastError = true)]
		private static extern bool OpenPrinter(string printerName, out IntPtr phPrinter, ref SpoolerHelper.PrinterDefaults printerDefaults);

		[DllImport("winspool.drv", SetLastError = true)]
		private static extern bool ClosePrinter(IntPtr phPrinter);

		[DllImport("winspool.drv", CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern bool XcvDataW(IntPtr hXcv, string pszDataName, IntPtr pInputData, uint cbInputData, out IntPtr pOutputData, uint cbOutputData, out uint pcbOutputNeeded, out uint pdwStatus);

		[DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int AddPrinterDriver(string pName, uint Level, ref SpoolerHelper.DRIVER_INFO_3 pDriverInfo);

		[DllImport("winspool.drv", CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern bool GetPrinterDriverDirectory(StringBuilder pName, StringBuilder pEnv, int Level, [Out] StringBuilder outPath, int bufferSize, ref int Bytes);

		[DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int AddPrinter(string pName, uint Level, [In] ref SpoolerHelper.PRINTER_INFO_2 pPrinter);

		public SpoolerHelper.GenericResult AddPrinterMonitor(string monitorName)
		{
			SpoolerHelper.GenericResult retVal = new SpoolerHelper.GenericResult("AddPrinterMonitor");
			SpoolerHelper.MONITOR_INFO_2 mi2 = default(SpoolerHelper.MONITOR_INFO_2);
			mi2.pName = monitorName;
			mi2.pEnvironment = null;
			mi2.pDLLName = "mfilemon.dll";
			try
			{
				bool flag = SpoolerHelper.AddMonitor(null, 2u, ref mi2) == 0;
				if (flag)
				{
					retVal.Exception = new Win32Exception(Marshal.GetLastWin32Error());
					retVal.Message = retVal.Exception.Message;
				}
			}
			catch (Exception ex)
			{
				retVal.Exception = ex;
				retVal.Message = retVal.Exception.Message;
			}
			bool flag2 = string.IsNullOrEmpty(retVal.Message);
			if (flag2)
			{
				retVal.Success = true;
			}
			return retVal;
		}

		public SpoolerHelper.GenericResult AddPrinterPort(string portName, string portType)
		{
			SpoolerHelper.GenericResult retVal = new SpoolerHelper.GenericResult("AddPrinterPort");
			SpoolerHelper.PrinterDefaults defaults = new SpoolerHelper.PrinterDefaults
			{
				DesiredAccess = SpoolerHelper.PrinterAccess.ServerAdmin
			};
			try
			{
				IntPtr printerHandle;
				bool flag = !SpoolerHelper.OpenPrinter(",XcvMonitor " + portType, out printerHandle, ref defaults);
				if (flag)
				{
					throw new Exception("Could not open printer for the monitor port " + portType + "!");
				}
				try
				{
					SpoolerHelper.PortData portData = new SpoolerHelper.PortData
					{
						sztPortName = portName
					};
					uint size = (uint)Marshal.SizeOf(portData);
					IntPtr pointer = Marshal.AllocHGlobal((int)size);
					Marshal.StructureToPtr(portData, pointer, true);
					try
					{
						IntPtr outputData;
						uint outputNeeded;
						uint status;
						bool flag2 = !SpoolerHelper.XcvDataW(printerHandle, "AddPort", pointer, size, out outputData, 0u, out outputNeeded, out status);
						if (flag2)
						{
							retVal.Message = status.ToString();
						}
					}
					catch (Exception ex)
					{
						retVal.Exception = ex;
						retVal.Message = retVal.Exception.Message;
					}
					finally
					{
						Marshal.FreeHGlobal(pointer);
					}
				}
				catch (Exception ex2)
				{
					retVal.Exception = ex2;
					retVal.Message = retVal.Exception.Message;
				}
				finally
				{
					SpoolerHelper.ClosePrinter(printerHandle);
				}
			}
			catch (Exception ex3)
			{
				retVal.Exception = ex3;
				retVal.Message = retVal.Exception.Message;
			}
			bool flag3 = string.IsNullOrEmpty(retVal.Message);
			if (flag3)
			{
				retVal.Success = true;
			}
			return retVal;
		}

		public SpoolerHelper.GenericResult GetPrinterDirectory()
		{
			SpoolerHelper.GenericResult retVal = new SpoolerHelper.GenericResult("GetPrinterDirectory");
			StringBuilder str = new StringBuilder(1024);
			int i = 0;
			SpoolerHelper.GetPrinterDriverDirectory(null, null, 1, str, 1024, ref i);
			try
			{
				SpoolerHelper.GetPrinterDriverDirectory(null, null, 1, str, 1024, ref i);
				retVal.Success = true;
				retVal.Message = str.ToString();
			}
			catch (Exception ex)
			{
				retVal.Exception = ex;
				retVal.Message = retVal.Exception.Message;
			}
			return retVal;
		}

		public SpoolerHelper.GenericResult AddPrinterDriver(string driverName, string driverPath, string dataPath, string configPath, string helpPath)
		{
			SpoolerHelper.GenericResult retVal = new SpoolerHelper.GenericResult("AddPrinterDriver");
			SpoolerHelper.DRIVER_INFO_3 di = default(SpoolerHelper.DRIVER_INFO_3);
			di.cVersion = 3u;
			di.pName = driverName;
			di.pEnvironment = null;
			di.pDriverPath = driverPath;
			di.pDataFile = dataPath;
			di.pConfigFile = configPath;
			di.pHelpFile = helpPath;
			di.pDependentFiles = "";
			di.pMonitorName = null;
			di.pDefaultDataType = "RAW";
			try
			{
				bool flag = SpoolerHelper.AddPrinterDriver(null, 3u, ref di) == 0;
				if (flag)
				{
					retVal.Exception = new Win32Exception(Marshal.GetLastWin32Error());
					retVal.Message = retVal.Exception.Message;
				}
			}
			catch (Exception ex)
			{
				retVal.Exception = ex;
				retVal.Message = retVal.Exception.Message;
			}
			bool flag2 = string.IsNullOrEmpty(retVal.Message);
			if (flag2)
			{
				retVal.Success = true;
			}
			return retVal;
		}

		public SpoolerHelper.GenericResult AddPrinter(string printerName, string portName, string driverName)
		{
			SpoolerHelper.GenericResult retVal = new SpoolerHelper.GenericResult("AddPrinter");
			SpoolerHelper.PRINTER_INFO_2 pi = default(SpoolerHelper.PRINTER_INFO_2);
			pi.pServerName = null;
			pi.pPrinterName = printerName;
			pi.pShareName = "";
			pi.pPortName = portName;
			pi.pDriverName = driverName;
			pi.pComment = "Cubemak Labs Printer";
			pi.pLocation = "";
			pi.pDevMode = new IntPtr(0);
			pi.pSepFile = "";
			pi.pPrintProcessor = "WinPrint";
			pi.pDatatype = "RAW";
			pi.pParameters = "";
			pi.pSecurityDescriptor = new IntPtr(0);
			try
			{
				bool flag = SpoolerHelper.AddPrinter(null, 2u, ref pi) == 0;
				if (flag)
				{
					retVal.Exception = new Win32Exception(Marshal.GetLastWin32Error());
					retVal.Message = retVal.Exception.Message;
				}
			}
			catch (Exception ex)
			{
				retVal.Exception = ex;
				retVal.Message = retVal.Exception.Message;
			}
			bool flag2 = string.IsNullOrEmpty(retVal.Message);
			if (flag2)
			{
				retVal.Success = true;
			}
			return retVal;
		}

		public SpoolerHelper.GenericResult ConfigureVirtualPort(string monitorName, string portName, string key)
		{
			//string installationpath = "";
			MainForm form = new MainForm();
			string installationpath = form.installationpath;

			SpoolerHelper.GenericResult retVal = new SpoolerHelper.GenericResult("ConfigureVirtualPort");
			try
			{
				//string outputPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\OperaVirtualPrinter\\jobs";
				string outputPath = installationpath;
				string filePattern = "%Y%m%d_%H%n%s_%j.pdf";
				string userCommand = string.Empty;
				string execPath = string.Empty;
				string keyName = string.Format("SYSTEM\\CurrentControlSet\\Control\\Print\\Monitors\\{0}\\{1}", monitorName, portName);
				Registry.LocalMachine.CreateSubKey(keyName);
				RegistryKey regKey = Registry.LocalMachine.OpenSubKey(keyName, true);
				regKey.SetValue("OutputPath", outputPath, RegistryValueKind.String);
				regKey.SetValue("FilePattern", filePattern, RegistryValueKind.String);
				regKey.SetValue("Overwrite", 0, RegistryValueKind.DWord);
				regKey.SetValue("UserCommand", userCommand, RegistryValueKind.String);
				regKey.SetValue("ExecPath", execPath, RegistryValueKind.String);
				regKey.SetValue("WaitTermination", 0, RegistryValueKind.DWord);
				regKey.SetValue("PipeData", 0, RegistryValueKind.DWord);
				regKey.Close();
				retVal.Success = true;
			}
			catch (Exception ex)
			{
				retVal.Exception = ex;
				retVal.Message = retVal.Exception.Message;
			}
			return retVal;
		}

		public SpoolerHelper.GenericResult RestartSpoolService()
		{
			SpoolerHelper.GenericResult retVal = new SpoolerHelper.GenericResult("RestartSpoolService");
			try
			{
				ServiceController sc = new ServiceController("Spooler");
				bool flag = sc.Status != ServiceControllerStatus.Stopped || sc.Status != ServiceControllerStatus.StopPending;
				if (flag)
				{
					sc.Stop();
				}
				sc.WaitForStatus(ServiceControllerStatus.Stopped);
				sc.Start();
				retVal.Success = true;
			}
			catch (Exception ex)
			{
				retVal.Exception = ex;
				retVal.Message = retVal.Exception.Message;
			}
			return retVal;
		}

		public SpoolerHelper.GenericResult AddVPrinter(string printerName, string key)
		{
			SpoolerHelper.GenericResult retVal = new SpoolerHelper.GenericResult("AddVPrinter");
			try
			{
				string portName = string.Format("{0}", printerName);
				string driverFileName = "PSCRIPT5.DLL";
				string dataFileName = "OperaVirtual.PPD";
				string configFileName = "PS5UI.DLL";
				string helpFileName = "PSCRIPT.HLP";
				string driverPath = "C:\\WINDOWS\\system32\\spool\\drivers\\w32x86\\PSCRIPT5.DLL";
				string dataPath = "C:\\WINDOWS\\system32\\spool\\drivers\\w32x86\\OperaVirtual.PPD";
				string configPath = "C:\\WINDOWS\\system32\\spool\\drivers\\w32x86\\PS5UI.DLL";
				string helpPath = "C:\\WINDOWS\\system32\\spool\\drivers\\w32x86\\PSCRIPT.HLP";
				SpoolerHelper.GenericResult printerDriverPath = this.GetPrinterDirectory();
				bool success = printerDriverPath.Success;
				if (success)
				{
					driverPath = string.Format("{0}\\{1}", printerDriverPath.Message, driverFileName);
					dataPath = string.Format("{0}\\{1}", printerDriverPath.Message, dataFileName);
					configPath = string.Format("{0}\\{1}", printerDriverPath.Message, configFileName);
					helpPath = string.Format("{0}\\{1}", printerDriverPath.Message, helpFileName);
				}
				SpoolerHelper.GenericResult printerMonitorResult = this.AddPrinterMonitor(printerName);
				bool flag = !printerMonitorResult.Success;
				if (flag)
				{
					bool flag2 = printerMonitorResult.Message.ToLower() != "the specified print monitor has already been installed";
					if (flag2)
					{
						throw printerMonitorResult.Exception;
					}
				}
				SpoolerHelper.GenericResult printerPortResult = this.AddPrinterPort(portName, printerName);
				bool flag3 = !printerPortResult.Success;
				if (flag3)
				{
					throw printerPortResult.Exception;
				}
				SpoolerHelper.GenericResult printerDriverResult = this.AddPrinterDriver(printerName, driverPath, dataPath, configPath, helpPath);
				bool flag4 = !printerDriverResult.Success;
				if (flag4)
				{
					throw printerDriverResult.Exception;
				}
				SpoolerHelper.GenericResult printerResult = this.AddPrinter(printerName, portName, printerName);
				bool flag5 = !printerResult.Success;
				if (flag5)
				{
					throw printerResult.Exception;
				}
				SpoolerHelper.GenericResult configResult = this.ConfigureVirtualPort(printerName, portName, key);
				bool flag6 = !configResult.Success;
				if (flag6)
				{
					throw configResult.Exception;
				}
				SpoolerHelper.GenericResult restartSpoolResult = this.RestartSpoolService();
				bool flag7 = !restartSpoolResult.Success;
				if (flag7)
				{
					throw restartSpoolResult.Exception;
				}
				retVal.Success = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format("{0}", ex));
				retVal.Exception = ex;
				retVal.Message = retVal.Exception.Message;
			}
			return retVal;
		}
	}
}
