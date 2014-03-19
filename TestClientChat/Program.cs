using Microsoft.AspNet.SignalR.Client;
using NUnit.Framework;
using ServerBingo.Models;
using ServerBingo.ModelsView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestClientChat
{
    [TestFixture]
    class Program
    {
        

        [Test]
        static void Main(string[] args)
        {
            Console.WriteLine("Starting client  http://localhost:8090/");

            var hubConnection = new HubConnection("http://localhost:8090/");
            hubConnection.TraceLevel = TraceLevels.StateChanges;
            hubConnection.TraceWriter = Console.Out;
            IHubProxy myHubProxy = hubConnection.CreateHubProxy("BingoHub");

            myHubProxy.On<string, string>("send", (name, message) => Console.Write("Recieved Send: " + name + ": " + message + "\n"));
            myHubProxy.On<Bingousuario>("devolverInfoUsuario", (bingoUsuario) => Console.WriteLine("Recieved devolverInfo: " + bingoUsuario.Alias));
            //myHubProxy.On("heartbeat", () => Console.Write("Recieved heartbeat \n"));
            //myHubProxy.On<HelloModel>("sendHelloObject", hello => Console.Write("Recieved sendHelloObject {0}, {1} \n", hello.Molly, hello.Age));

            hubConnection.Start().Wait(10000);

            UsuarioConexion usuarioConexion = new UsuarioConexion();

            usuarioConexion.Alias = "Jaime5";
            usuarioConexion.Ip = "192.168.0.1";
            usuarioConexion.Macaddress = "000000000000";

            myHubProxy.Invoke("conectarUsurio", usuarioConexion).ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        Console.WriteLine("!!! There was an error opening the connection:{0} \n", task.Exception.GetBaseException());
                    }
                }).Wait();

            while (true)
            {
                string key = Console.ReadLine();
                if (key.ToUpper() == "U")
                {
                    Bingousuario bingousuario = myHubProxy.Invoke<Bingousuario>("DevolverUsuario", "Jaime1").Result;

                    if (!(bingousuario == null))
                        Console.WriteLine("Alias: {0}.", bingousuario.Alias);
                    else
                        Console.WriteLine("No devolvio nada");

                    myHubProxy.Invoke("sendUsuario", "Jaime2").ContinueWith(task =>
                        {
                            if (task.IsFaulted)
                            {
                                Console.WriteLine("!!! There was an error opening the connection:{0} \n", task.Exception.GetBaseException());
                            }
                        }).Wait();

                }
                if (key.ToUpper() == "W")
                {
                    myHubProxy.Invoke("addMessage", "client message", " sent from console client").ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            Console.WriteLine("!!! There was an error opening the connection:{0} \n", task.Exception.GetBaseException());
                        }

                    }).Wait();
                    Console.WriteLine("Client Sending addMessage to server\n");
                }
                if (key.ToUpper() == "E")
                {
                    myHubProxy.Invoke("Heartbeat").ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            Console.WriteLine("There was an error opening the connection:{0}", task.Exception.GetBaseException());
                        }

                    }).Wait();
                    Console.WriteLine("client heartbeat sent to server\n");
                }
                if (key.ToUpper() == "R")
                {
                    HelloModel hello = new HelloModel { Age = 10, Molly = "clientMessage" };
                    myHubProxy.Invoke<HelloModel>("SendHelloObject", hello).ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            Console.WriteLine("There was an error opening the connection:{0}", task.Exception.GetBaseException());
                        }

                    }).Wait();
                    Console.WriteLine("client sendHelloObject sent to server\n");
                }
                if (key.ToUpper() == "C")
                {
                    
                    break;
                }
            }
        }

        private static void hubConnection_Closed()
        {

            throw new NotImplementedException();
        }

    }
}
