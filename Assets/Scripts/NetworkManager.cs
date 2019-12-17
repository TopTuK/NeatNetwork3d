using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ConnectionLink
{
	public int TargetIdx;
	public float Weight;
}

[Serializable]
public class Connection
{
	public int SourceIdx;
	public ConnectionLink[] InputLinks;
	public ConnectionLink[] OutputLinks;
}

[Serializable]
public class ActivationNode : Node
{
	public string ActivationName;
}

[Serializable]
public class Node
{
	public int Idx;
}

[Serializable]
public class NeatNetwork
{
	public string Fitness;
	public string Error;
	
	public int BiasNodesCount;
	public int InputNodesCount;
	public int HiddenNodesCount;
	public int OutputNodesCount;
	
	public Node[] BiasNodes;
	public Node[] InputNodes;
	public Node[] OutputNodes;
	public ActivationNode[] HiddenNodes;
	
	public Connection[] Connections;
}

public static class NetworkManager
{
	private static NeatNetwork _network = null;

	public static bool LoadNetwork(string fileName)
	{
		Debug.Log(string.Format("Reading network file: {0}", fileName));
		bool result = false;
		
		StreamReader reader = new StreamReader(fileName);
		try
		{
			string json = reader.ReadToEnd();
			_network = JsonUtility.FromJson<NeatNetwork>(json);
			
			Debug.Log(string.Format("Fitness: {0}", _network.Fitness));
			Debug.Log(string.Format("Error: {0}", _network.Error));
			
			Debug.Log(string.Format("Bias Nodes Count: {0}", _network.BiasNodesCount));
			Debug.Log(string.Format("Input Nodes Count: {0}", _network.InputNodesCount));
			Debug.Log(string.Format("Output Nodes Count: {0}", _network.OutputNodesCount));
			Debug.Log(string.Format("Hidden Nodes Count: {0}", _network.HiddenNodesCount));
			Debug.Log(string.Format("Connections Count: {0}", _network.Connections.Length));
			
			result = true;
		}
		catch
		{
			result = false;
		}
		finally
		{
			reader.Dispose();
		}
		
		return result;
	}
	
	public static NeatNetwork Network => _network;
	
	public static Node[] BiasNodes => _network.BiasNodes;
	public static Node[] InputNodes => _network.InputNodes;
	public static Node[] OutputNodes => _network.OutputNodes;
	public static ActivationNode[] HiddenNodes => _network.HiddenNodes;
	
	public static Connection[] Connections => _network.Connections;
}
