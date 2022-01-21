using System;
using System.Collections.Generic;
using System.IO;

namespace RTGA.GoogleAuthenticationCheck.Testing
{
    public abstract class TestedConnection<T>
    {
        public Lazy<T> ConnectionObject { get; private set; }
        public ConnectionStatus Status { get; private set; }
        public List<string> Log { get; private set; }

        public TestedConnection()
        {
            Status = ConnectionStatus.Uninitialized;
            Log = new List<string>();

            ConnectionObject = new Lazy<T>(() =>
            {
                Status = ConnectionStatus.Validating;
                if(ValidateInput())
                {
                    Status = ConnectionStatus.Connecting;
                    T connectionObject = Connect();
                    Status = ConnectionStatus.Testing;
                    if(TestConnection(connectionObject))
                    {
                        Status = ConnectionStatus.Succeeded;
                        return connectionObject;
                    }
                    else
                    {
                        Status = ConnectionStatus.Failed;
                    }
                }
                else
                {
                    Status = ConnectionStatus.Invalid;
                }

                return DefaultConnectionObject;
            });

            Status = ConnectionStatus.Initialized;
        }

        protected virtual T DefaultConnectionObject => default(T);

        protected abstract bool ValidateInput();
        protected abstract T Connect();
        protected abstract bool TestConnection(T connectionObject);
    }

    public enum ConnectionStatus
    {
        Uninitialized,
        Initialized,
        Validating,
        Invalid,
        Connecting,
        Testing,
        Failed,
        Succeeded,
    }
}