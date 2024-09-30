using System.Net;
using System.Net.Sockets;
using System.Text;

var host = "192.168.1.176";
var port = 5001;
var deviceId = Guid.NewGuid();

var ipEndPoint = new IPEndPoint(IPAddress.Parse(host), port);

var stopWord = "exit";

var cancellationToken = new CancellationTokenSource();

using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

await socket.ConnectAsync(ipEndPoint);

Console.WriteLine($"Successfully connected to {host} (port: {port})");

var readingTask = Task.Factory.StartNew(
	StartReading, cancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
// var writingTask = Task.Factory.StartNew(
// 	StartWriting, cancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

await StartWriting();

socket.Shutdown(SocketShutdown.Both);

async Task StartReading()
{
	while(true)
	{
		var buffer = new byte[1024];
		var receivedBytes = await socket.ReceiveAsync(buffer);

		if (receivedBytes != 0)
		{
			var message = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
			Console.WriteLine(message);
		}
	}
}

async Task StartWriting()
{
	var input = string.Empty;
	
	while (true)
	{
		input = deviceId + ": " + Console.ReadLine();
		var buffer = Encoding.UTF8.GetBytes(input!);

		if (input == stopWord)
			break;

		await socket.SendAsync(buffer);
	}
}
