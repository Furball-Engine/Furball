using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Furball.Engine.Engine.DevConsole;
using Microsoft.Xna.Framework;

namespace Furball.Engine.Engine.Helpers.Logger {
    public static class Logger {
        private static Queue<LoggerLine> _LoggerLines = new();
        private static List<LoggerBase>  _Loggers     = new();

        private static double _UpdateDeltaTime = 0;
        
        public static List<LoggerBase> Loggers => _Loggers;

        /// <summary>
        /// The time in seconds before each logger update, defaults to half a second
        /// </summary>
        public static double UpdateRate = 0.1d;
        
        public static async void Update(GameTime time) {
            _UpdateDeltaTime += time.ElapsedGameTime.TotalSeconds;

            if (_UpdateDeltaTime >= UpdateRate) {
                _UpdateDeltaTime = 0d;
                
                await Task.Run(
                delegate {
                    if (_LoggerLines.Count == 0) return;

                    do {
                        if (_LoggerLines.Count == 0) break;
                        LoggerLine lineToSend = _LoggerLines.Dequeue();

                        foreach (LoggerBase logger in _Loggers) {
                            if (logger.Level.Contains(lineToSend.LoggerLevel) || logger.Level.Contains(LoggerLevel.All))
                                logger.Send(lineToSend);
                        }
                    } while (_LoggerLines.Count > 0);
                }
                );
            }
        }

        public static void AddLogger(LoggerBase loggerBase) {
            loggerBase.Initialize();
            _Loggers.Add(loggerBase);
        }

        public static void RemoveLogger(LoggerBase loggerBase) {
            loggerBase.Dispose();
            _Loggers.Remove(loggerBase);
        }

        public static void RemoveLogger(Type type) {
            for (var i = 0; i < _Loggers.Count; i++) 
                if (_Loggers[i].GetType() == type) 
                    RemoveLogger(_Loggers[i]);
        }

        public static void Log(LoggerLine line) {
            line.LineData = line.LineData.Replace("\r", "");
            line.LineData = line.LineData.Replace("\n", " ");
            _LoggerLines.Enqueue(line);
        }

        public static void Log(string data, LoggerLevel level = null) {
            if (data is null) data = "";
            
            level ??= LoggerLevelUnknown.Instance;

            Log(new LoggerLine{LoggerLevel = level, LineData = data});
        }
    }
}
