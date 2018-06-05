
using System;

namespace Samarium.PluginFramework.Logger {

	using Config;

	using System.ComponentModel;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
    using System.Threading;
	using System.Threading.Tasks;

	/// <summary>
	/// Logger.
	/// </summary>
	public sealed class Logger: INotifyPropertyChanged, IDisposable {

		readonly static object Lock = "U iz lawked  (╯°□°) ╯ ┻━┻ ";

		Logger(string name, string logFilePath) {
			LoggerName = name;
            if (logFilePath is null)
                LogFilePath = SystemConfig?.GetString("log_directory");
            else
                LogFilePath = logFilePath;
            PrintToConsole = SystemConfig?.GetBool("log_to_console") ?? true;
            PrintToFile = SystemConfig?.GetBool("log_to_file") ?? true;
            if (SystemConfig != null) {
                SystemConfig.ConfigSet += ConfigParser_ConfigSet;
                SystemConfig.ConfigsLoaded += ConfigParser_ConfigsLoaded;
            }
		}

		/// <summary>
		/// The color trace.
		/// </summary>
		public const ConsoleColor COLOR_TRACE = ConsoleColor.DarkMagenta;
		/// <summary>
		/// The color debug.
		/// </summary>
		public const ConsoleColor COLOR_DEBUG = ConsoleColor.Cyan;
		/// <summary>
		/// The color info.
		/// </summary>
		public const ConsoleColor COLOR_INFO = ConsoleColor.Blue;
		/// <summary>
		/// The color warn.
		/// </summary>
		public const ConsoleColor COLOR_WARN = ConsoleColor.Yellow;
		/// <summary>
		/// The color error.
		/// </summary>
		public const ConsoleColor COLOR_ERROR = ConsoleColor.DarkYellow;
		/// <summary>
		/// The color fatal.
		/// </summary>
		public const ConsoleColor COLOR_FATAL = ConsoleColor.Red;
        /// <summary>
        /// Colour definition for 'OK' logs
        /// </summary>
        public const ConsoleColor COLOR_OK = ConsoleColor.Green;
		/// <summary>
		/// The def background.
		/// </summary>
		public const ConsoleColor DEF_BACKGROUND = ConsoleColor.Black;
		/// <summary>
		/// The def foreground.
		/// </summary>
		public const ConsoleColor DEF_FOREGROUND = ConsoleColor.White;

		#region Singleton
		static Logger() {
			instances = new Dictionary<string, Logger>();
		}

		static Dictionary<string, Logger> instances;

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <returns>The instance.</returns>
		/// <param name="name">Name.</param>
		public static Logger GetInstance(string name) {
            if (instances.TryGetValue(name, out var logger))
                return logger;

            throw new ArgumentNullException(nameof(name), "Could not find logger!");
		}

		/// <summary>
		/// Creates the instance.
		/// </summary>
		/// <returns>The instance.</returns>
		/// <param name="name">Name.</param>
		/// <param name="logFilePath">Log file path.</param>
		public static Logger CreateInstance(string name, string logFilePath) {
			if (instances?.ContainsKey(name) == true) {
				Logger logger = null;
				instances?.TryGetValue(name, out logger);
				logger.LogFilePath = logFilePath;
				TotalIndicies = TotalIndicies + 1;
				logger.CurrentInstanceIndex = TotalIndicies;
				instances?.Remove(name);
				instances?.Add(name, logger);
				return logger;
			} else {
				var logger = new Logger(name, logFilePath);
				instances?.Add(name, logger);
				return logger;
			}
		}
		#endregion

		#region INotifyPropertyChanged
		/// <summary>
		/// Occurs when property changed.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		void OnPropertyChanged(string property) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
		}
		#endregion

		#region IDisposable
		/// <summary>
		/// Releases all resource used by the <see cref="T:Samarium.PluginFramework.Logger.Logger"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="T:Samarium.PluginFramework.Logger.Logger"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="T:Samarium.PluginFramework.Logger.Logger"/> in an unusable state.
		/// After calling <see cref="Dispose"/>, you must release all references to the
		/// <see cref="T:Samarium.PluginFramework.Logger.Logger"/> so the garbage collector can reclaim the memory that the
		/// <see cref="T:Samarium.PluginFramework.Logger.Logger"/> was occupying.</remarks>
		public void Dispose() {
			try {
				instances.Remove(LoggerName);
				FlushBuffer();
				LogBuffers.Remove(LogBuffers[CurrentInstanceIndex]);
			} catch (Exception) {
				Fatal("Error while disposing of logger!");
			}
		}
        #endregion

        #region Config events
        private void ConfigParser_ConfigSet(IConfig sender, string key) {
            if (key.ToLowerInvariant() == "log_to_console")
                PrintToConsole = sender.GetBool(key);
            else if (key.ToLowerInvariant() == "log_to_file")
                PrintToFile = sender.GetBool(key);
        }

        private void ConfigParser_ConfigsLoaded(IConfig sender) {
            PrintToConsole = sender.GetBool("log_to_console");
            PrintToFile = sender.GetBool("log_to_file");
        }
        #endregion

        #region Properties
        string logFilePath = "";
		/// <summary>
		/// Gets or sets the log file path.
		/// </summary>
		/// <value>The log file path.</value>
		public string LogFilePath {
			get { return logFilePath; }
			set {
				if (string.IsNullOrEmpty(value)) return;
				logFilePath = value;
				if (!Directory.Exists(logFilePath))
					Directory.CreateDirectory(logFilePath);

				LogFile = new FileInfo(Path.Combine(value, string.Join(".", LoggerName, "log")));
                if (!LogFile.Exists)
                    LogFile.Open(FileMode.CreateNew).Close();
                else if (SystemConfig?.GetBool("truncate_logs") == true)
                    LogFile.Open(FileMode.Truncate).Close();

				OnPropertyChanged(nameof(LogFilePath));
			}
		}

		/// <summary>
		/// Gets or sets the log file.
		/// </summary>
		/// <value>The log file.</value>
		public FileInfo LogFile { get; set; }

		/// <summary>
		/// Gets the name of the logger.
		/// </summary>
		/// <value>The name of the logger.</value>
		public string LoggerName { get; }

		bool printToConsole = true;
		/// <summary>
		/// Gets or sets the print to console.
		/// </summary>
		/// <value>The print to console.</value>
		public bool PrintToConsole {
			get { return printToConsole; }
			set { printToConsole = value; OnPropertyChanged(nameof(PrintToConsole)); }
		}

		bool printToFile = true;
		/// <summary>
		/// Gets or sets the print to file.
		/// </summary>
		/// <value>The print to file.</value>
		public bool PrintToFile {
			get => printToFile;
			set { printToFile = value; OnPropertyChanged(nameof(PrintToFile)); }
		}

		internal int CurrentInstanceIndex { get; set; }

		internal static int TotalIndicies { get; set; }

		internal static List<StringBuilder> LogBuffers { get; set; } = new List<StringBuilder>();

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="T:Samarium.PluginFramework.Logger.Logger"/> logger alive.
		/// </summary>
		/// <value><c>true</c> if logger alive; otherwise, <c>false</c>.</value>
		public static bool LoggerAlive { get; set; } = true;

		/// <summary>
		/// Gets the system config.
		/// </summary>
		/// <value>The system config.</value>
		static IConfig SystemConfig { get; set; }
		#endregion

		#region Methods
		/// <summary>
		/// Flushes this logger instance's output buffer.
		/// </summary>
		public void FlushBuffer() {
			try {
                var sWriter = default(StreamWriter);

                // Determine whether log file is viable for deletion or not
                if (LogFile.CreationTime <= DateTime.Now.AddDays(-7) || LogFile.Length >= 1073741824L) {
                    LogFile.CreationTime = DateTime.Now;
                    LogFile.Open(FileMode.Truncate);
                }

				sWriter = File.AppendText(logFilePath);

                using (sWriter) {
                    sWriter.Write(LogBuffers[CurrentInstanceIndex]);
                    sWriter.Flush();
                }
			} catch (Exception) {
				GetInstance("Samarium").Fatal($"Could not flush buffer for { LoggerName }!");
			}
		}

		/// <summary>
		/// Flushes this logger instance's output buffer asynchronously.
		/// </summary>
		/// <returns></returns>
		public async Task FlushBufferAsync() {
			await Task.Delay(60000);
			await Task.Run(() => {
				var file = File.AppendText(logFilePath);
				file.Write(LogBuffers[CurrentInstanceIndex]);
				file.Flush();
				file.Close();

			});
		}

		/// <summary>
		/// Log the specified level, msg and newLine.
		/// </summary>
		/// <param name="level">Level.</param>
		/// <param name="msg">Message.</param>
		/// <param name="newLine">If set to <c>true</c> new line.</param>
		public void Log(LogLevel level, string msg, bool newLine = true) {
			lock (Lock) {
#if !DEBUG
			if (level == LogLevel.Debug || level == LogLevel.Trace)
				return;
#endif

                // If logs aren't supposed to be printed, no point in going through all the logic here.
                // Just return from the method and all is good.
                // 
                // UNLESS the logging level is FATAL or ERROR. If either if true, the message must be passed on to the console AND
                // logfile.
                if ((!PrintToConsole && (level != LogLevel.Fatal || level != LogLevel.Error)) && !PrintToFile)
                    return;

                var date = DateTime.Now;

                string output01 = $"[{ LoggerName }] [{ date.ToShortDateString() }] [{ date.ToShortTimeString() }]",
                        output02 = $"{ level }",
                        output03 = $"{ msg }";

                if (level == LogLevel.Error || level == LogLevel.Fatal) {
                    if (PrintToConsole) {
                        Console.Write(output01);
                        Console.Write(" [");
                        switch (level) {
                            case LogLevel.Error:
                                Console.ForegroundColor = COLOR_ERROR;
                                break;
                            case LogLevel.Fatal:
                                Console.ForegroundColor = COLOR_FATAL;
                                break;
                        }
                        Console.Write(output02);
                        Console.ForegroundColor = DEF_FOREGROUND;
                        Console.Write("] ");
                        Console.Write(output03);
                        if (newLine)
                            Console.WriteLine();
                    }
                } else {
                    if (PrintToConsole || level == LogLevel.None) {
                        Console.Write(output01);
                        if (level != LogLevel.None) {
                            Console.Write(" [");
                            switch (level) {
                                case LogLevel.Debug:
                                    Console.ForegroundColor = COLOR_DEBUG;
                                    break;
                                case LogLevel.Info:
                                    Console.ForegroundColor = COLOR_INFO;
                                    break;
                                case LogLevel.Trace:
                                    Console.ForegroundColor = COLOR_TRACE;
                                    break;
                                case LogLevel.Warn:
                                    Console.ForegroundColor = COLOR_WARN;
                                    break;
                                case LogLevel.OK:
                                    Console.ForegroundColor = COLOR_OK;
                                    break;
                                case LogLevel.None:
                                    Console.ForegroundColor = DEF_FOREGROUND;
                                    break;
                            }
                            Console.Write(output02);
                            Console.ForegroundColor = DEF_FOREGROUND;
                            Console.Write("] ");
                        } else Console.Write(' ');
                        if (level == LogLevel.Debug)
                            Console.ForegroundColor = COLOR_DEBUG;
                        Console.Write(output03);
                        if (level == LogLevel.Debug)
                            Console.ForegroundColor = DEF_FOREGROUND;
                        if (newLine)
                            Console.WriteLine();
                    }
                }

				if (PrintToFile && level != LogLevel.None) {
                    var sWriter = default(StreamWriter);

                    // Determine whether log file is viable for deletion or not
                    if (LogFile.CreationTime <= DateTime.Now.AddDays(-7) || LogFile.Length >= 1073741824L) {
                        LogFile.CreationTime = DateTime.Now;
#pragma warning disable CS0642
                        using (LogFile.Open(FileMode.Truncate)) ;
#pragma warning restore CS0642
                    }

                    sWriter = LogFile.AppendText();
                    using (sWriter) {
                        sWriter.WriteLine($"{ output01 } [{ output02 }] { output03 }");
                        sWriter.Flush();
					}
				}
            }
		}

		/// <summary>
		/// Trace the specified msg and newLine.
		/// </summary>
		/// <param name="msg">Message.</param>
		/// <param name="newLine">If set to <c>true</c> new line.</param>
		public void Trace(string msg, bool newLine = true) => Log(LogLevel.Trace, msg, newLine);

        public void Trace(string format, params object[] args) => Trace(string.Format(format, args));

		/// <summary>
		/// Debug the specified msg and newLine.
		/// </summary>
		/// <param name="msg">Message.</param>
		/// <param name="newLine">If set to <c>true</c> new line.</param>
		public void Debug(string msg, bool newLine = true) => Log(LogLevel.Debug, msg, newLine);

        public void Debug(string format, params object[] args) => Debug(string.Format(format, args));

		/// <summary>
		/// Info the specified msg and newLine.
		/// </summary>
		/// <param name="msg">Message.</param>
		/// <param name="newLine">If set to <c>true</c> new line.</param>
		public void Info(string msg, bool newLine = true) => Log(LogLevel.Info, msg, newLine);

        public void Info(string format, params object[] args) => Info(string.Format(format, args));

		/// <summary>
		/// Warn the specified msg and newLine.
		/// </summary>
		/// <param name="msg">Message.</param>
		/// <param name="newLine">If set to <c>true</c> new line.</param>
		public void Warn(string msg, bool newLine = true) => Log(LogLevel.Warn, msg, newLine);

        public void Warn(string format, params object[] args) => Warn(string.Format(format, args));

		/// <summary>
		/// Error the specified msg and newLine.
		/// </summary>
		/// <param name="msg">Message.</param>
		/// <param name="newLine">If set to <c>true</c> new line.</param>
		public void Error(string msg, bool newLine = true) => Log(LogLevel.Error, msg, newLine);

        public void Error(string format, params object[] args) => Error(string.Format(format, args));

		/// <summary>
		/// Fatal the specified msg and newLine.
		/// </summary>
		/// <param name="msg">Message.</param>
		/// <param name="newLine">If set to <c>true</c> new line.</param>
		public void Fatal(string msg, bool newLine = true) => Log(LogLevel.Fatal, msg, newLine);

        public void Fatal(string format, params object[] args) => Fatal(string.Format(format, args));

        /// <summary>
        /// OK
        /// Logs a message with the log level 'OK'
        /// </summary>
        /// <param name="msg">The log message</param>
        /// <param name="newLine">Set to <code>true</code> to print a new line</param>
        public void Ok(string msg, bool newLine = true) => Log(LogLevel.OK, msg, newLine);

        public void Ok(string format, params object[] args) => Ok(string.Format(format, args));

        public void Output(string format, params object[] args) => Log(LogLevel.None, string.Format(format, args));

        public void Output(string msg) => Log(LogLevel.None, msg);

        /// <summary>
        /// Sets the config object for this instance.
        /// If the config has already been set, the new instance will be ignored to prevent catastrophic failure.
        /// </summary>
        /// <param name="cfg">The new config object.</param>
        public Logger SetConfig(IConfig cfg) {
            SystemConfig = SystemConfig ?? cfg;
            return this;
        }
		#endregion

	}

}


