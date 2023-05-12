// This script is created by Digvijaysinh Gohil.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpiroGraph : MonoBehaviour {
	private bool shouldDraw = false;
	private float lastDrawTime;
	private List<Vector3> graphPoints;

	[SerializeField] private LineRenderer lineRenderer;
	[SerializeField] private LineRenderer lineRendererVisuals;

	[Space, SerializeField] private Transform outerCircle;
	[SerializeField] private Transform innerCircle;
	[SerializeField] private Transform drawPoint;

	[Space, Header("Spirograph controls")]
	public float outerRadius = 5;
	public float innerRadius = 3;
	public float drawPointRadius = 1;
	[Range(10f, 1000f)] public float outerCircleRotationSpeed = 5;
	private float innerCircleRotationSpeed;

	[Space, Header("Spirograph optimisation")]
	public float maxGraphPoints = 5000;
	[Range(0.01f, 1f)] public float graphPointDistanceThreshold = .05f;
	[Range(0.005f, 1f)] public float drawInterval = .009f;

	private void Awake() {
		graphPoints = new List<Vector3>();
		InitialisePoints();
	}

	private void OnValidate() {
		shouldDraw = false;
		outerCircle.gameObject.SetActive(true);
		InitialisePoints();
		ClearGraph();
	}

	private void Update() {
		if (shouldDraw)
			return;

		RotatePoints();
		AddPointToGraph(drawPoint.position);
	}

	private void LateUpdate() {
		if (shouldDraw)
			return;
		UpdateLine();
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere( transform.position, outerRadius);
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere( innerCircle.position, innerRadius);
		Gizmos.color = Color.magenta;
		float drawSphereRadius = 0.1f;
		Gizmos.DrawSphere(drawPoint.position, drawSphereRadius);
	}

	private void InitialisePoints() {
		innerCircleRotationSpeed = outerCircleRotationSpeed * outerRadius / innerRadius;
		innerCircle.localPosition = new Vector3(outerRadius - innerRadius, 0, 0);
		drawPoint.localPosition = new Vector3(drawPointRadius, 0, 0);

		UpdateLine();
	}

	private void UpdateLine() {
		lineRendererVisuals.positionCount = 3;
		lineRendererVisuals.SetPosition(0, transform.position);
		lineRendererVisuals.SetPosition(1, innerCircle.position);
		lineRendererVisuals.SetPosition(2, drawPoint.position);
	}

	private void RotatePoints() {
		outerCircle.Rotate(transform.forward, outerCircleRotationSpeed * Time.deltaTime);
		innerCircle.Rotate(-transform.forward, innerCircleRotationSpeed * Time.deltaTime);
	}

	private void AddPointToGraph(Vector3 pointToDraw) {
		if (Time.time - lastDrawTime >= drawInterval) {
			lastDrawTime = Time.time;
			if (!graphPoints.Contains(pointToDraw)) {
				if (graphPoints.Count > 0 && Vector3.Distance(pointToDraw, graphPoints[graphPoints.Count - 1]) > graphPointDistanceThreshold) {
					graphPoints.Add(pointToDraw);
				}
				else {
					graphPoints.Add(pointToDraw);
				}
				if (graphPoints.Count > maxGraphPoints) {
					ClearLineVisuals();
				}
			}

			DrawSpiroGraph();
		}
	}

	private void ClearGraph() {
		graphPoints?.Clear();
		lineRenderer.positionCount = 0;
	}

	private void DrawSpiroGraph() {
		lineRenderer.positionCount = graphPoints.Count;
		for (int i = 0; i < graphPoints.Count; i++) {
			lineRenderer.SetPosition(i, graphPoints[i]);
		}
	}

	public void ClearLineVisuals() {
		shouldDraw = true;
		lineRendererVisuals.positionCount = 0;
		outerCircle.gameObject.SetActive(false);
	}
}

[CanEditMultipleObjects, CustomEditor(typeof(SpiroGraph))]
public class SpiroGraphEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		SpiroGraph controller = target as SpiroGraph;
		EditorGUILayout.Space(5);
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Stop")) {
			controller.ClearLineVisuals();
		}
		GUILayout.EndHorizontal();
	}
}