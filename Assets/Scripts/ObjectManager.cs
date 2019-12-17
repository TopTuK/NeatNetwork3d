using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ObjectManager : MonoBehaviour
{
	private enum NodeType
	{
		BIAS = 0,
		INPUT,
		HIDDEN,
		OUTPUT
	};
	
	private class NodeObj
	{
		public NodeObj(NodeType objType, Node obj)
		{
			NodeObjType = objType;
			Obj = obj;
		}
		
		public NodeType NodeObjType { get; }
		public Node Obj { get; }
	}
	
	private const int SEGMENT_COUNT = 50;
	
	[SerializeField] private Transform BiasNodePrefab;
	[SerializeField] private Transform InputNodePrefab;
	[SerializeField] private Transform OutputNodePrefab;
	[SerializeField] private Transform HiddenNodePrefab;
	
	[SerializeField] private Text FitnessText;
	[SerializeField] private Text ErrorText;
	
	[SerializeField] private Text ObjTypeText;
	[SerializeField] private Text ObjIdxText;
	[SerializeField] private Text ActivationText;
	
	private Dictionary<Transform, NodeObj> _objList;
	private Dictionary<int, Transform> _nodeList;
	private List<GameObject> _lineList;
	
	private readonly Color BIASCOLOR = Color.cyan;
	private readonly Color INPUTCOLOR = Color.white;
	private readonly Color HIDDENCOLOR = Color.blue;
	private readonly Color OUTPUTCOLOR = Color.yellow;
	
	private readonly Color SOURCECOLOR = Color.red;
	private readonly Color CHILDCOLOR = Color.black;
	
    // Start is called before the first frame update
    void Start()
    {
		_lineList = new List<GameObject>();
		_objList = new Dictionary<Transform, NodeObj>();
		_nodeList = new Dictionary<int, Transform>();
		
		int x = 0, y = 0, z = 0;
		
		var network = NetworkManager.Network;
		FitnessText.text = string.Format("Fitness: {0}", network.Fitness);
		ErrorText.text = string.Format("Error: {0}", network.Error);
		
		// Creat Bias Nodes
		x = 14; y = 28;
		foreach(var biasNode in NetworkManager.BiasNodes)
		{
			var node = Instantiate(BiasNodePrefab, new Vector3((float) x * 1.5f, (float) y * 1.5f, (float) z), Quaternion.identity);
			SetNodeColor(node, BIASCOLOR);
			
			_objList.Add(node, new NodeObj(NodeType.BIAS, biasNode));
			_nodeList.Add(biasNode.Idx, node);
		}
		
        // Create Input nodes
		x = 0; y = 0; z = 0;
		foreach(var inputNode in NetworkManager.InputNodes)
		{
			var node = Instantiate(InputNodePrefab, new Vector3((float) x * 1.5f, (float) y * 1.5f, (float) z), Quaternion.identity);
			SetNodeColor(node, INPUTCOLOR);
			
			_objList.Add(node, new NodeObj(NodeType.INPUT, inputNode));
			_nodeList.Add(inputNode.Idx, node);
			
			if((x != 0) && (x == 27))
			{
				x = 0;
				y++;
			}
			else x++;
		}
		
		// Create output nodes
		x = 10; y = 21; z = 60;
		int outNum = 0;
		foreach(var outputNode in NetworkManager.OutputNodes)
		{
			var node = Instantiate(OutputNodePrefab, new Vector3((float) x * 1.5f, (float) y, (float) z), Quaternion.identity);
			SetNodeColor(node, OUTPUTCOLOR);
			
			AddTextToObject(node, outNum.ToString(), 3);
			outNum++;
			
			_objList.Add(node, new NodeObj(NodeType.OUTPUT, outputNode));
			_nodeList.Add(outputNode.Idx, node);
			
			x++;
		}
		
		// Creat hidden nodes
		if(NetworkManager.HiddenNodes != null)
		{
			var hiddenLength = NetworkManager.HiddenNodes.Length;
			if(hiddenLength <= 10)
			{
				int offset = 1;
				x = 0; y = 21; z = 30;
				
				foreach(var hiddenNode in NetworkManager.HiddenNodes)
				{
					var node = Instantiate(HiddenNodePrefab, new Vector3((float) x * 4.0f + 16.0f, (float) y, (float) z), Quaternion.identity);
					SetNodeColor(node, HIDDENCOLOR);
					
					offset = (offset % 2) == 0 ? 1 : 2;
					AddTextToObject(node, ((ActivationNode) hiddenNode).ActivationName, offset);
										
					_objList.Add(node, new NodeObj(NodeType.HIDDEN, hiddenNode));
					_nodeList.Add(hiddenNode.Idx, node);
					
					if((x != 0) && (x == 5))
					{
						x = 0;
						y++;
					}
					else x++;
				}
			}
			else if(hiddenLength <= 100)
			{
				// WALL
				int rowNum = hiddenLength / 10;
				int offset = 2;
				x = 0; y = 18; z = 30;
				
				foreach(var hiddenNode in NetworkManager.HiddenNodes)
				{
					var node = Instantiate(HiddenNodePrefab, new Vector3((float) x * 4.0f + 16.0f, (float) y, (float) z), Quaternion.identity);
					SetNodeColor(node, HIDDENCOLOR);
					
					offset = (offset == 2) ? 3 : 2;
					AddTextToObject(node, hiddenNode.ActivationName, offset);
					
					_objList.Add(node, new NodeObj(NodeType.HIDDEN, hiddenNode));
					_nodeList.Add(hiddenNode.Idx, node);
					
					if((x != 0) && (x == 9))
					{
						x = 0;
						y += 4;
					}
					else x++;
				}
			}
			else
			{
				int offset = 1;
				
				var circleSpeed = 0f;
				var forwardSpeed = 1f;
				var circleSize = 5f;
				var circleGrowSpeed = 0.5f;
				
				float xPos = 0.0f, yPos = 0.0f, zPos = 30f;
				
				foreach(var hiddenNode in NetworkManager.HiddenNodes)
				{
					var node = Instantiate(HiddenNodePrefab, new Vector3(xPos + 16.0f, yPos + 14.0f, zPos), Quaternion.identity);
					SetNodeColor(node, HIDDENCOLOR);
					
					xPos = Mathf.Sin(circleSpeed) * circleSize;
					yPos = Mathf.Cos(circleSpeed) * circleSize;
					
					circleSpeed += 15.0f;
					circleSize += circleGrowSpeed;
					
					AddTextToObject(node, hiddenNode.ActivationName, offset);
					
					_objList.Add(node, new NodeObj(NodeType.HIDDEN, hiddenNode));
					_nodeList.Add(hiddenNode.Idx, node);
				}
			}
		}
    }
	
	void SetNodeColor(Transform node, Color color)
	{
		var nodeRender = node.GetComponent<Renderer>();
		nodeRender.material.SetColor("_Color", color);
	}
	
	public void ResetDetails()
	{
		ObjIdxText.text = "Object Idx: -1";
		ObjTypeText.text = "Object type: none";
		ActivationText.text = "Activation name: null";
		
		ResetLines();
	}
	
	void ResetLines()
	{
		foreach(var go in _lineList)
		{
			Destroy(go);
		}
		_lineList.Clear();
		
		foreach(var node in _objList)
		{
			switch(node.Value.NodeObjType)
			{
				case NodeType.BIAS:
					SetNodeColor(node.Key, BIASCOLOR);
					break;
				case NodeType.INPUT:
					SetNodeColor(node.Key, INPUTCOLOR);
					break;
				case NodeType.HIDDEN:
					SetNodeColor(node.Key, HIDDENCOLOR);
					break;
				case NodeType.OUTPUT:
					SetNodeColor(node.Key, OUTPUTCOLOR);
					break;
			}
		}
	}
	
	public void SelectObject(Transform obj)
	{
		ResetDetails();
		
		var node = _objList[obj];
		if(node != null)
		{
			ObjIdxText.text = string.Format("Object Idx: {0}", node.Obj.Idx);
			ActivationText.text = "Activation name: null";
			
			switch(node.NodeObjType)
			{
				case NodeType.BIAS:
					ObjTypeText.text = "Object type: BIAS";
					break;
				case NodeType.INPUT:
					ObjTypeText.text = "Object type: INPUT";
					break;
				case NodeType.HIDDEN:
					ObjTypeText.text = "Object type: HIDDEN";
					ActivationText.text = string.Format("Activation name: {0}", ((ActivationNode)node.Obj).ActivationName);
					break;
				case NodeType.OUTPUT:
					ObjTypeText.text = "Object type: OUTPUT";
					break;
			}
			
			DrawObjectConnections(obj, node);
		}
		else
		{
			ObjIdxText.text = "Object Idx: -1";
			ObjTypeText.text = "Object type: none";
			ActivationText.text = "Activation name: null";
			
			ResetLines();
		}
	}
	
	public void SelectAll()
	{
		ResetLines();
		
		foreach(var connection in NetworkManager.Connections)
		{
			var sourceNode = _nodeList[connection.SourceIdx];
			
			if(connection.SourceIdx == 0) // Source node is Bias
			{
				foreach(var target in connection.OutputLinks)
				{
					var targetNode = _nodeList[target.TargetIdx];
					
					if(target.TargetIdx <= 794) // Bias -> HiddenNode
					{
						DrawQuadraticBezierCurve(sourceNode.position, targetNode.position, Color.yellow);
					}
					else // Bias -> Output
					{
						DrawCubicBezierCurve(sourceNode.position, targetNode.position, Color.blue);
					}
				}
			}
			else if((connection.SourceIdx > 0) && (connection.SourceIdx <= 784)) // Input node
			{
				foreach(var target in connection.OutputLinks)
				{
					var targetNode = _nodeList[target.TargetIdx];
					
					if(target.TargetIdx > 794) // Input -> HiddenNode
					{
						DrawLinearBezierCurve(sourceNode.position, targetNode.position, Color.red);
					}
					else // Input -> Output
					{
						DrawCubicBezierCurve(sourceNode.position, targetNode.position, Color.green);
					}
				}
			}
			else
			{
				foreach(var target in connection.OutputLinks)
				{
					var targetNode = _nodeList[target.TargetIdx];
					
					if(target.TargetIdx > 794) // HiddenNode -> HiddenNode
					{
						DrawLinearBezierCurve(sourceNode.position, targetNode.position, Color.black);
					}
					else // Input -> Output
					{
						DrawCubicBezierCurve(sourceNode.position, targetNode.position, Color.white);
					}
				}
			}
		}
	}
	
	public void SelectBias()
	{
		ResetLines();
		
		var node =_objList.FirstOrDefault(n => n.Value.NodeObjType == NodeType.BIAS);
		if(node.Key != null)
		{
			SelectObject(node.Key);
		}
	}
	
	public void SelectHidden()
	{
		ResetLines();
		
		var hiddenNodes = _objList
			.Where(n => n.Value.NodeObjType == NodeType.HIDDEN)
			.Select(n => n)
			.ToList();
			
		foreach(var hiddenNode in hiddenNodes)
		{
			var nodeIdx = hiddenNode.Value.Obj.Idx;
			
			var connection = NetworkManager.Connections.FirstOrDefault(c => c.SourceIdx == nodeIdx);
			if(connection != null)
			{
				foreach(var link in connection.InputLinks)
				{
					var node = _nodeList[link.TargetIdx];
					DrawLinearBezierCurve(hiddenNode.Key.position, node.position, Color.red);
				}
				
				foreach(var link in connection.OutputLinks)
				{
					var node = _nodeList[link.TargetIdx];
					DrawLinearBezierCurve(hiddenNode.Key.position, node.position, Color.blue);
				}
			}
		}
	}
	
	public void SelectInput()
	{
		ResetLines();
		
		var inputNodes = _objList
			.Where(n => n.Value.NodeObjType == NodeType.INPUT)
			.Select(n => n)
			.ToList();
			
		foreach(var inputNode in inputNodes)
		{
			var nodeIdx = inputNode.Value.Obj.Idx;
			
			var connection = NetworkManager.Connections.FirstOrDefault(c => c.SourceIdx == nodeIdx);
			if(connection != null)
			{
				foreach(var link in connection.InputLinks)
				{
					var node = _nodeList[link.TargetIdx];
					DrawLinearBezierCurve(inputNode.Key.position, node.position, Color.red);
				}
				
				foreach(var link in connection.OutputLinks)
				{
					var node = _nodeList[link.TargetIdx];
					DrawLinearBezierCurve(inputNode.Key.position, node.position, Color.blue);
				}
			}
		}
	}
	
	void DrawObjectConnections(Transform obj, NodeObj nodeObj)
	{
		SetNodeColor(obj, SOURCECOLOR);
		var nodeConnections = Array.Find(NetworkManager.Connections, c => c.SourceIdx == nodeObj.Obj.Idx);
		
		if(nodeConnections != null)
		{
			var objPosition = obj.position;
			
			if((nodeConnections.InputLinks != null) && (nodeConnections.InputLinks.Length > 0))
			{
				foreach(var link in nodeConnections.InputLinks)
				{
					var node = _objList.FirstOrDefault(n => n.Value.Obj.Idx == link.TargetIdx).Key;
					SetNodeColor(node, CHILDCOLOR);
					DrawLinearBezierCurve(objPosition, node.position, Color.blue);
				}
			}
			
			if((nodeConnections.OutputLinks != null) && (nodeConnections.OutputLinks.Length > 0))
			{
				foreach(var link in nodeConnections.OutputLinks)
				{
					var node = _objList.FirstOrDefault(n => n.Value.Obj.Idx == link.TargetIdx).Key;
					SetNodeColor(node, CHILDCOLOR);
					DrawLinearBezierCurve(objPosition, node.position, Color.red);
				}
			}
		}
	}
	
	void DrawLinearBezierCurve(Vector3 startPoint, Vector3 endPoint, Color color)
	{
		var go = new GameObject();
		var lineRenderer = go.AddComponent<LineRenderer>();
		
		_lineList.Add(go);
		
		lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.01f;
		
		lineRenderer.startColor = color;
		lineRenderer.endColor = color;
		
        lineRenderer.positionCount = SEGMENT_COUNT;
		
		lineRenderer.SetPosition(0, startPoint);
		for(int i = 1; i < SEGMENT_COUNT; i++)
		{
			float t = i / (float) SEGMENT_COUNT;
			Vector3 p = startPoint + t * (endPoint - startPoint);
			
			lineRenderer.SetPosition(i, p);
		}
		
		lineRenderer.SetPosition(49, endPoint);
	}
	
	void DrawQuadraticBezierCurve(Vector3 startPoint, Vector3 endPoint, Color color)
	{
		var go = new GameObject();
		var lineRenderer = go.AddComponent<LineRenderer>();
		
		_lineList.Add(go);
		
		lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.05f;
        lineRenderer.positionCount = SEGMENT_COUNT;
		
		lineRenderer.startColor = color;
		lineRenderer.endColor = color;
		
		Vector3 p1 = startPoint * 1.5f;

		lineRenderer.SetPosition(0, startPoint);
		for(int i = 1; i < SEGMENT_COUNT; i++)
		{
			float t = i / (float) SEGMENT_COUNT;
			
			float t1 = (1 - t) * (1 - t);
			float t2 = 2 * (1 - t) * t;
			float t3 = t*t;
			
			Vector3 p = t1*startPoint + t2*p1 + t3*endPoint;
			lineRenderer.SetPosition(i, p);
		}
		
		lineRenderer.SetPosition(49, endPoint);		
	}
	
	void DrawCubicBezierCurve(Vector3 startPoint, Vector3 endPoint, Color color)
	{
		var go = new GameObject();
		var lineRenderer = go.AddComponent<LineRenderer>();
		
		_lineList.Add(go);
		
		lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.05f;
        lineRenderer.positionCount = SEGMENT_COUNT;
		
		lineRenderer.startColor = color;
		lineRenderer.endColor = color;
		
		Vector3 p1 = startPoint;
		p1.x *= 1.3f;
		
		Vector3 p2 = startPoint;
		p2.x *= 1.7f;
		
		lineRenderer.SetPosition(0, startPoint);
		for(int i = 1; i < SEGMENT_COUNT; i++)
		{
			float t = i / (float) SEGMENT_COUNT;
			Vector3 p = CalculateCubicBezierPoint(t, startPoint, p1, p2, endPoint);
			
			lineRenderer.SetPosition(i, p);
		}
		lineRenderer.SetPosition(49, endPoint);
	}
	
	Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t; // (1 - t)^1
		float uu = u * u; // (1 - t)^2
        float uuu = uu * u; // (1 - t)^3
		
        float tt = t * t; // t^2
        float ttt = tt * t; // t^3
        
        Vector3 p = uuu * p0; // (1-t)^3 * p0
        p += 3 * uu * t * p1; // 3 *(1-t)^2*t * p1
        p += 3 * u * tt * p2; // 3 *(1-t)*t^2 * p2
        p += ttt * p3; // (1-t)^3 * p3
        
        return p;
    }
	
	void AddTextToObject(Transform obj, string text, int textOffset = 2)
	{
		GameObject textObj = new GameObject();
		
		// Make block to be parent of this gameobject
		textObj.transform.parent = obj;
		textObj.name = "Text Holder";

		// Create TextMesh and modify its properties
		TextMesh textMesh = textObj.AddComponent<TextMesh>();
		textMesh.text = text;
		textMesh.characterSize = 1;

		// Set postion of the TextMesh with offset
		textMesh.anchor = TextAnchor.MiddleCenter;
		textMesh.alignment = TextAlignment.Center;
		textMesh.transform.position = new Vector3(obj.position.x,  obj.position.y + textOffset, obj.position.z);
	}
}
