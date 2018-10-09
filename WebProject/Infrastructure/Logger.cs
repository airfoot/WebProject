using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
using log4net.Config;



namespace WebProject.Infrastructure
{
    public class Logger: ILogger
    {
        private log4net.ILog _logger = log4net.LogManager.GetLogger("WebProject.Logger");

        public void Info(string message)
        {
            _logger.Info(message);
        }
        public void Info(string message, Exception exception)
        {
            _logger.Info(message, exception);
        }
        public void Error(string message)
        {
            _logger.Error(message);
        }
        public void Error(string message, Exception exception)
        {
            _logger.Error(message, exception);
        }
        public void Warn(string message)
        {
            _logger.Warn(message);
        }
        public void Warn(string message, Exception exception)
        {
            _logger.Warn(message, exception);
        }
        public void Debug(string message, Exception exception)
        {
            _logger.Debug(message, exception);
        }
        public void Debug(string message)
        {
            _logger.Debug(message);
        }
        public void Fatal(string message)
        {
            _logger.Fatal(message);
        }
        public void Fatal(string message, Exception exception)
        {
            _logger.Fatal(message, exception);
        }
    }
}