using System.Net;
using System.Net.Sockets;
using System.Text;

var clients = new List<Socket>();
var port = 5001;
var cancellationToken = new CancellationTokenSource();
var ipEndPoint = new IPEndPoint(IPAddress.Any, port);

using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

socket.Bind(ipEndPoint);
socket.Listen(10);

Console.WriteLine("Waiting for clients...");

while (true)
{
	Socket clientSocket = socket.Accept();
	clients.Add(clientSocket);

	var clientTask = Task.Factory.StartNew(
		() => Handle(clientSocket), cancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
}

async Task Handle(Socket clientSocket)
{
	var buffer = new byte[1024];

	while (true)
	{
		var receivedBytes = await clientSocket.ReceiveAsync(buffer);
		
		if (receivedBytes == 0)
			break;

		var message = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
		
		Console.WriteLine($"New message received: {message}");
		Broadcast(message, clientSocket);
	}

	clients.Remove(clientSocket);

	clientSocket.Shutdown(SocketShutdown.Both);
	clientSocket.Close();
}

void Broadcast(string message, Socket sender)
{
	var buffer = Encoding.UTF8.GetBytes(message);

	foreach (var client in clients)
	{
		if (client != sender)
			client.Send(buffer);
	}
}
