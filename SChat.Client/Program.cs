using System.Net;
using System.Net.Sockets;
using System.Text;

var host = SetValueOrDefault(
	"192.168.1.176", 
	$"Enter host (default: 192.168.1.176): ");

var port = int.Parse(SetValueOrDefault(
	5001.ToString(), 
	$"Enter port (default: 5001): "));

var deviceName = SetValueOrDefault(
	Guid.NewGuid().ToString().Substring(0, 7), 
	$"Enter your name (default: random GUID): ");

var stopWord = "exit";
var cancellationToken = new CancellationTokenSource();
var ipEndPoint = new IPEndPoint(IPAddress.Parse(host!), port);

using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

try
{
	await socket.ConnectAsync(ipEndPoint);
	Console.WriteLine($"Successfully connected to {host} (port: {port}). Enter 'exit' to close.");
}
catch (Exception ex)
{
	Console.WriteLine($"Failed to connect to server. Error message: {ex.Message}");
	return;
}

var readingTask = Task.Factory.StartNew(
	StartReading, cancellationToken.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

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
			Print(message, ConsoleColor.DarkBlue);
		}
	}
}

async Task StartWriting()
{
	var input = string.Empty;
	
	while (true)
	{
		input = deviceName + ": " + Console.ReadLine();

		Console.SetCursorPosition(0, Console.GetCursorPosition().Top - 1);
		Print(input, ConsoleColor.DarkRed);
		
		var buffer = Encoding.UTF8.GetBytes(input!);

		if (input == stopWord)
			break;

		try
		{
			await socket.SendAsync(buffer);
		}
		catch(Exception ex)
		{
			Console.WriteLine($"Error while sending a message: {ex.Message}");
		}
	}
}

string SetValueOrDefault(string defaultValue, string message)
{
	Console.Write(message);
	var newValue = Console.ReadLine();
	return string.IsNullOrEmpty(newValue) ? defaultValue : newValue;
}

void Print(string message, ConsoleColor backgroundColor)
{
	var parts = message.Split(':', StringSplitOptions.TrimEntries);

	var defaultColor = Console.BackgroundColor;

	Console.BackgroundColor = backgroundColor;
	Console.Write(parts[0]);
	Console.BackgroundColor = defaultColor;

	Console.WriteLine($": {parts[1]}");
}
