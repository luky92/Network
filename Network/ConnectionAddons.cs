﻿using Network.Interfaces;
using Network.Logging;
using Network.Packets;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Network
{
    /// <summary>
    /// Partial class which implements additional features for the <see cref="Connection"/> class.
    /// </summary>
    public abstract partial class Connection : IPacketHandler
    {
        #region Variables

        /// <summary>
        /// Backing field for <see cref="Logger"/>.
        /// </summary>
        private NetworkLog logger;

        /// <summary>
        /// Backing field for <see cref="IsWindows"/>. Caches the value one for later use.
        /// </summary>
        /// <remarks>
        /// Since an assembly cannot be transferred across an OS during runtime, this is a variable that can be set
        /// upon instantiation and it is valid for the lifetime of this <see cref="Connection"/> instance.
        /// </remarks>
        private static readonly bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        /// <summary>
        /// Backing field for <see cref="IsMAC"/>. Caches the value one for later use.
        /// </summary>
        /// <remarks>
        /// Since an assembly cannot be transferred across an OS during runtime, this is a variable that can be set
        /// upon instantiation and it is valid for the lifetime of this <see cref="Connection"/> instance.
        /// </remarks>
        private static readonly bool isOsx = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        /// <summary>
        /// Backing field for <see cref="IsLinux"/>. Caches the value one for later use.
        /// </summary>
        /// <remarks>
        /// Since an assembly cannot be transferred across an OS during runtime, this is a variable that can be set
        /// upon instantiation and it is valid for the lifetime of this <see cref="Connection"/> instance.
        /// </remarks>
        private static readonly bool isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        /// <summary>
        /// Determins whether the running client has windows xp or higher installed.
        /// </summary>
        private static readonly bool isXpOrHigher = (Environment.OSVersion.Platform == PlatformID.Win32NT) &&
            ((Environment.OSVersion.Version.Major > 5) || ((Environment.OSVersion.Version.Major == 5) &&
            (Environment.OSVersion.Version.Minor >= 1)));

        #endregion Variables

        #region Properties

        /// <summary>
        /// The <see cref="NetworkLog"/> instance to which information should be logged.
        /// </summary>
        internal NetworkLog Logger
        {
            get { return logger; }
        }

        /// <summary>
        /// Whether the executing assembly is running on OSX.
        /// </summary>
        /// <remarks>
        /// Since an assembly cannot be transferred across an OS during runtime, this is a variable that can be set
        /// upon instantiation and it is valid for the lifetime of this <see cref="Connection"/> instance.
        /// </remarks>
        internal static bool IsMAC => isOsx;

        /// <summary>
        /// Whether the executing assembly is running on Linux.
        /// </summary>
        /// <remarks>
        /// Since an assembly cannot be transferred across an OS during runtime, this is a variable that can be set
        /// upon instantiation and it is valid for the lifetime of this <see cref="Connection"/> instance.
        /// </remarks>
        internal static bool IsLinux => isLinux;

        /// <summary>
        /// Whether the executing assembly is running on Windows.
        /// </summary>
        /// <remarks>
        /// Since an assembly cannot be transferred across an OS during runtime, this is a variable that can be set
        /// upon instantiation and it is valid for the lifetime of this <see cref="Connection"/> instance.
        /// </remarks>
        internal static bool IsWindows => isWindows;

        /// <summary>
        /// Whether the executing assembly is running on Windows.
        /// and the operating system is higher or equal to Windows XP.
        /// </summary>
        /// <value><c>true</c> if this instance is windows and xp or higher; otherwise, <c>false</c>.</value>
        internal static bool IsXpOrHigher => IsWindows && isXpOrHigher;

        /// <summary>
        /// Whether the <see cref="Connection"/> instance should automatically log information to the <see cref="Logger"/>s
        /// output <see cref="Stream"/>s.
        /// </summary>
        public bool EnableLogging
        {
            get { return logger.EnableLogging; }
            set { logger.EnableLogging = value; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Initialises this <see cref="Connection"/> instance's addons, setting up all required variables.
        /// </summary>
        partial void InitAddons()
        {
            logger = new NetworkLog(this);
        }

        /// <summary>
        /// Logs events, exceptions and messages into the given stream. To disable logging into a previous provided stream, call
        /// this method again and provide a null reference as stream. Stream hot swapping is supported.
        /// </summary>
        /// <param name="stream">The stream to log into.</param>
        public void LogIntoStream(Stream stream) => logger.SetOutputStream(stream);

        /// <summary>
        /// Sends the given <see cref="RawData"/> packet to the network.
        /// </summary>
        /// <param name="rawData">The <see cref="RawData"/> packet to send to the network.</param>
        public void SendRawData(RawData rawData) => Send(rawData);

        /// <summary>
        /// Sends the given raw, serialised primitive to the network.
        /// </summary>
        /// <param name="key">
        /// The <see cref="string"/> key which identifies the raw data <see cref="PacketReceivedHandler{P}"/> to use for the data.
        /// </param>
        /// <param name="data">The serialised raw primitive, as a <see cref="byte"/> array.</param>
        public void SendRawData(string key, byte[] data)
        {
            if (data == null)
            {
                logger.Log("Can't send a null reference data byte array", new ArgumentException(), Enums.LogLevel.Information);
                return;
            }

            Send(new RawData(key, data));
        }

        #endregion Methods
    }
}