﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    public class Browser
    {
        public Browser(Connection connection, Dictionary<string, object> options)
        {
            Connection = connection;
            IgnoreHTTPSErrors = options.ContainsKey("ignoreHTTPSErrors") && (bool)options["ignoreHTTPSErrors"];
            AppMode = options.ContainsKey("appMode") && (bool)options["appMode"];

            Connection.Closed += (object sender, EventArgs e) => Disconnected?.Invoke(this, new EventArgs());
            Connection.MessageReceived += Connect_MessageReceived;
        }

        #region Properties
        public Connection Connection { get; set; }
        public event EventHandler Closed;
        public event EventHandler Disconnected;
        public event EventHandler<TargetChangedArgs> TargetChanged;

        public string WebSocketEndpoint
        {
            get
            {
                return Connection.Url;
            }
        }

        public bool IgnoreHTTPSErrors { get; set; }
        public bool AppMode { get; set; }

        #endregion

        #region Public Methods

        public async Task Initialize()
        {
            dynamic args = new ExpandoObject();
            args.discover = true;
            await Connection.SendAsync("Target.setDiscoverTargets", args);
        }

        public async Task<Page> NewPageAsync()
        {
            var targetId = (int)(await Connection.SendAsync("Target.createTarget", new Dictionary<string, object>(){
        {"url", "about:blank"}
      }));
            var client = await Connection.CreateSession(targetId);
            return new Page(client, IgnoreHTTPSErrors, AppMode);
        }

        internal void ChangeTarget(TargetInfo targetInfo)
        {
            TargetChanged?.Invoke(this, new TargetChangedArgs()
            {
                TargetInfo = targetInfo
            });
        }

        public async Task<string> GetVersionAsync()
        {
            dynamic version = await Connection.SendAsync("Browser.getVersion");
            return version.product.ToString();
        }

        public void Close()
        {
            Closed?.Invoke(this, new EventArgs());
        }
        #endregion

        #region Private Methods

        public void Connect_MessageReceived(object sender, MessageEventArgs args)
        {
            switch (args.MessageID)
            {
                case "Target.targetCreated":
                    targetCreated();
                    return;

                case "Target.targetDestroyed":
                    targetDestroyed();
                    return;

                case "Target.targetInfoChanged":
                    targetInfoChanged();
                    return;
            }
        }

        private void targetInfoChanged()
        {
            throw new NotImplementedException();
        }

        private void targetDestroyed()
        {
            throw new NotImplementedException();
        }

        private void targetCreated()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
