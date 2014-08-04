﻿
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

using FOG;

namespace FOG
{
	/// <summary>
	/// Handle all shutdown requests
	/// The windows shutdown command is used over the win32 api because it notifies the user prior
	/// </summary>
	public class ShutdownHandler {

		//Define variables
		private Boolean shutdownPending;
		private LogHandler logHandler;

		public ShutdownHandler(LogHandler logHandler) {
			this.shutdownPending = false;
			this.logHandler = logHandler;
		}
		
		//Load the ability to lock the computer from the native user32 dll
		[DllImport("user32")]
		private static extern void LockWorkStation();
		
		//List all possible shutdown types
		public enum ShutDownType {
			LogOff = 0,
			Shutdown = 1,
			Reboot = 2,
			ForcedLogOff = 4,
			ForcedShutdown = 5,
			ForcedReboot = 6,
			PowerOff = 8,
			ForcedPowerOff = 12
		}
		
		//List reasons for why the shutdown was called
		//TODO shorten this list to just what is needed, no point in defining everything
		[Flags]
		internal enum ShutdownReason : uint {
			MajorApplication = 0x00040000,
			MajorHardware = 0x00010000,
			MajorLegacyApi = 0x00070000,
			MajorOperatingSystem = 0x00020000,
			MajorOther = 0x00000000,
			MajorPower = 0x00060000,
			MajorSoftware = 0x00030000,
			MajorSystem = 0x00050000,

			MinorBlueScreen = 0x0000000F,
			MinorCordUnplugged = 0x0000000b,
			MinorDisk = 0x00000007,
			MinorEnvironment = 0x0000000c,
			MinorHardwareDriver = 0x0000000d,
			MinorHotfix = 0x00000011,
			MinorHung = 0x00000005,
			MinorInstallation = 0x00000002,
			MinorMaintenance = 0x00000001,
			MinorMMC = 0x00000019,
			MinorNetworkConnectivity = 0x00000014,
			MinorNetworkCard = 0x00000009,
			MinorOther = 0x00000000,
			MinorOtherDriver = 0x0000000e,
			MinorPowerSupply = 0x0000000a,
			MinorProcessor = 0x00000008,
			MinorReconfig = 0x00000004,
			MinorSecurity = 0x00000013,
			MinorSecurityFix = 0x00000012,
			MinorSecurityFixUninstall = 0x00000018,
			MinorServicePack = 0x00000010,
			MinorServicePackUninstall = 0x00000016,
			MinorTermSrv = 0x00000020,
			MinorUnstable = 0x00000006,
			MinorUpgrade = 0x00000003,
			MinorWMI = 0x00000015,

			FlagUserDefined = 0x40000000,
			FlagPlanned = 0x80000000
		}
		
		//List options on how to exit windows
		[Flags]
		public enum ExitWindows : uint
		{
			LogOff = 0x00,
			ShutDown = 0x01,
			Reboot = 0x02,
			PowerOff = 0x08,
			RestartApps = 0x40,
			Force = 0x04,
			ForceIfHung = 0x10,
		}
		
		public Boolean isShutdownPending() { return shutdownPending; }
		
		private void createShutdownCommand(String parameters) {
			Process.Start("shutdown", parameters);
		}
		
		public void shutdown(String comment, int seconds) {
			this.shutdownPending = true;
			
			logHandler.log(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name,
			               "Attemping to shutdown computer in " + seconds.ToString() + " seconds");
			
			createShutdownCommand("/s /c \"" + comment + "\" /t " + seconds + "/d " + ShutdownReason.MajorApplication);
		}
		
		public void restart(String comment, int seconds) {
			this.shutdownPending = true;
			
			logHandler.log(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name,
			              "Attemping to restart computer in " + seconds.ToString() + " seconds");
			
			createShutdownCommand("/r /c \"" + comment + "\" /t " + seconds + "/d " + ShutdownReason.MajorApplication);
		}		
		
		public void logOffUser(String comment, int seconds) {
			logHandler.log(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name,
			              "Attemping to log off user in " + seconds.ToString() + " seconds");
			
			createShutdownCommand("/l /c \"" + comment + "\" /t " + seconds);
		}
		
		public void hibernate(String comment, int seconds) {
			logHandler.log(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name,
			              "Attemping to hibernate computer");
			createShutdownCommand("/h" );
		}
		
		public void lockWorkStation() {
			logHandler.log(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name,
			              "Attemping to lock computer");			
			LockWorkStation();
		}
		
		public void abortShutdown() {
			logHandler.log(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name,
			              "Attemping to abort shutdown");			
			this.shutdownPending = false;
			createShutdownCommand("/a");
		}
		
	}
}