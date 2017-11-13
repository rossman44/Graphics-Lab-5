using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatmullRomCurveInterpolation : MonoBehaviour {
	
	const int NumberOfPoints = 8;
	Vector3[] controlPoints;
	
	const int MinX = -5;
	const int MinY = -5;
	const int MinZ = 0;

	const int MaxX = 5;
	const int MaxY = 5;
	const int MaxZ = 5;
	
	double time = 0;
	const double DT = 0.01;

	double longestSpline = 0;
	double[] segmentLength = new double[8];

	List<Vector3>[] positions = new List<Vector3>[8];
	
	/* Returns a point on a cubic Catmull-Rom/Blended Parabolas curve
	 * u is a scalar value from 0 to 1
	 * segment_number indicates which 4 points to use for interpolation
	 */
	Vector3 ComputePointOnCatmullRomCurve(double u, int segmentNumber)
	{
		Vector3 point = new Vector3();

		int p1 = (int)u;
		int p2 = (p1 + 1) % NumberOfPoints;
		int p3 = (p2 + 1) % NumberOfPoints;
		int p0;
		if (p1 >= 1) {
			p0 = p1 - 1;
		} else {
			p0 = NumberOfPoints - 1;
		}

		print (u);
		print (p1);

		double t = u - (int)u;


		double tt = t * t;
		double ttt = t * t * t;

		double q1 = -ttt + 2.0 * tt - t;
		double q2 = 3.0 * ttt - 5.0 * tt + 2.0;
		double q3 = -3.0f * ttt + 4.0f * tt + t;
		double q4 = ttt - tt;

		float fq1 = (float)q1;
		float fq2 = (float)q2;
		float fq3 = (float)q3;
		float fq4 = (float)q4;


		//print (u);
		//print (segmentNumber);

		point.x = 0.5f * (controlPoints [p0].x * fq1 + controlPoints [p1].x * fq2 + controlPoints [p2].x * fq3 + controlPoints [p3].x * fq4);
		point.y = 0.5f * (controlPoints [p0].y * fq1 + controlPoints [p1].y * fq2 + controlPoints [p2].y * fq3 + controlPoints [p3].y * fq4);
		point.z = 0.5f * (controlPoints [p0].z * fq1 + controlPoints [p1].z * fq2 + controlPoints [p2].z * fq3 + controlPoints [p3].z * fq4);

		//print (segmentLength [segmentNumber]);
		//point.x = point.x * ((float)(segmentLength [segmentNumber] % NumberOfPoints) / (float)longestSpline);
		//point.y = point.y * ((float)(segmentLength [segmentNumber] % NumberOfPoints) / (float)longestSpline);
		//point.z = point.z * ((float)(segmentLength [segmentNumber] % NumberOfPoints) / (float)longestSpline);

		// TODO - compute and return a point as a Vector3		
		// Hint: Points on segment number 0 start at controlPoints[0] and end at controlPoints[1]
		//		 Points on segment number 1 start at controlPoints[1] and end at controlPoints[2]
		//		 etc...
		
		return point;
	}

	Vector3 DirectionGeneration (double u, int segmentNumber) {
		Vector3 direction = new Vector3();

		u = u - (int)u;

		double t = u;
		double tt = u * u;
		double ttt = u * u * u;

		double qq1 = -3.0f * tt + 4.0f * t - 1f;
		double qq2 = 9.0f * tt - 10.0f + t;
		double qq3 = -9.0f * tt + 8.0f * t + 1.0f;
		double qq4 = 3.0f * tt - 2.0f * t;

		float fqq1 = (float)qq1;
		float fqq2 = (float)qq2;
		float fqq3 = (float)qq3;
		float fqq4 = (float)qq4;

		direction.x = 0.5f * (controlPoints [segmentNumber % NumberOfPoints].x * fqq1 + controlPoints [(segmentNumber + 1) % NumberOfPoints].x * fqq2 + controlPoints [(segmentNumber + 2) % NumberOfPoints].x * fqq3 + controlPoints [(segmentNumber + 3) % NumberOfPoints].x * fqq4);
		direction.y = 0.5f * (controlPoints [segmentNumber % NumberOfPoints].y * fqq1 + controlPoints [(segmentNumber + 1) % NumberOfPoints].y * fqq2 + controlPoints [(segmentNumber + 2) % NumberOfPoints].y * fqq3 + controlPoints [(segmentNumber + 3) % NumberOfPoints].y * fqq4);
		direction.z = 0.5f * (controlPoints [segmentNumber % NumberOfPoints].z * fqq1 + controlPoints [(segmentNumber + 1) % NumberOfPoints].z * fqq2 + controlPoints [(segmentNumber + 2) % NumberOfPoints].z * fqq3 + controlPoints [(segmentNumber + 3) % NumberOfPoints].z * fqq4);


		return direction;
	}
	
	void GenerateControlPointGeometry()
	{
		for(int i = 0; i < NumberOfPoints; i++)
		{
			GameObject tempcube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			tempcube.transform.localScale -= new Vector3(0.8f,0.8f,0.8f);
			tempcube.transform.position = controlPoints[i];
		}	
	}

	float CalculateSegmentLength(int segmentNumber) {
		float segLength = 0.0f;
		float count = 0f;
		double pos = 0.005;
		Vector3 targetPoint = new Vector3();
		double t = time - (int)time;
		//Vector3 currentPoint = ComputePointOnCatmullRomCurve ((double)segmentNumber + 0.1, segmentNumber);
		//print (segmentNumber);
		Vector3 currentPoint = controlPoints[segmentNumber];

		while (count < 1.0f) {
			targetPoint = ComputePointOnCatmullRomCurve (pos, (segmentNumber) % NumberOfPoints);
			//targetPoint = controlPoints[(pos + 0.005) % NumberOfPoints];
			segLength = segLength + Mathf.Sqrt ((targetPoint.x - currentPoint.x) * (targetPoint.x - currentPoint.x)
			+ (targetPoint.y - currentPoint.y) * (targetPoint.y - currentPoint.y)
			+ (targetPoint.z - currentPoint.z) * (targetPoint.z - currentPoint.z));
			//print (segLength);
			count = count + 0.005f;
			pos = pos + 0.005;
			currentPoint = targetPoint;
		}
		//print (segLength);
		return segLength;

	}

	float normalizeDistance(float position) {
		int count = 0;
		while ((double)position > segmentLength [count]) {
			position = position - (float)segmentLength [count];
			count = count + 1;
		}

		return (float)count + (position / (float)segmentLength[count]);
	}

	// Use this for initialization
	void Start () {

		controlPoints = new Vector3[NumberOfPoints];

		//start works as intended
		// set points randomly...



//		controlPoints[0] = new Vector3(0,0,0);
//		for(int i = 1; i < NumberOfPoints; i++)
//		{
//			controlPoints[i] = new Vector3(Random.Range(MinX,MaxX),Random.Range(MinY,MaxY),Random.Range(MinZ,MaxZ));
//
//		}
			
		controlPoints[0] = new Vector3(0,0,0);
		controlPoints[1] = new Vector3(2f,1f,0);
		controlPoints[2] = new Vector3(3f,3f,0);
		controlPoints[3] = new Vector3(2f,4f,0);
		controlPoints[4] = new Vector3(0f,5f,0);
		controlPoints[5] = new Vector3(-2f,4f,0);
		controlPoints[6] = new Vector3(-3f,3f,0);
		controlPoints[7] = new Vector3(-2f,1f,0);
//

//		controlPoints[0] = new Vector3(0,0,0);
//		controlPoints[1] = new Vector3(0f,0,0);
//		controlPoints[2] = new Vector3(2f,0,0);
//		controlPoints[3] = new Vector3(3f,0,0);
//		controlPoints[4] = new Vector3(4f,0,0);
//		controlPoints[5] = new Vector3(5f,0,0);
//		controlPoints[6] = new Vector3(6f,0,0);
//		controlPoints[7] = new Vector3(6f,0,0);

		for (int j = 0; j < NumberOfPoints; j++) {
			segmentLength[j] = CalculateSegmentLength (j);
			//print (segmentLength [j]);
			if (segmentLength [j] > longestSpline) {
				longestSpline = segmentLength [j];
			}
			//print (longestSpline);
		}

		positions[0] = new List<Vector3>();
		positions[1] = new List<Vector3>();
		positions[2] = new List<Vector3>();
		positions[3] = new List<Vector3>();
		positions[4] = new List<Vector3>();
		positions[5] = new List<Vector3>();
		positions[6] = new List<Vector3>();
		positions[7] = new List<Vector3>();

		for (double i = 0; i < 8; i += 0.01) {
			int truncate = (int)i;
			print("Trying to do it: " + i);
			Vector3 temp = new Vector3();
			temp = ComputePointOnCatmullRomCurve (i, (int)i);

			GameObject tempcube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			tempcube.transform.localScale -= new Vector3(0.4f,0.4f,0.4f);
			tempcube.transform.position = temp;

			positions[truncate].Add(temp);
		}

		GenerateControlPointGeometry();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 temp = new Vector3();
		Vector3 temp2 = new Vector3 ();

		time += DT;
			
		// TODO - use time to determine values for u and segment_number in this function call
		//int u = (int)time;
		//comment

		//can't normalize when finding point, so multiply transform

//		if ((int)time < 6 && (int)time > 0) {
//			temp = ComputePointOnCatmullRomCurve(time, (int)time);
//			temp2 = DirectionGeneration(time, (int)time);
//		}

		temp = ComputePointOnCatmullRomCurve (time, (int)time);
		temp2 = DirectionGeneration(time, (int)time);

		//Attempt to normalize temp vector

		transform.position = temp;
		transform.rotation = Quaternion.Euler(transform.rotation.x  + temp2.x, transform.rotation.y  + temp2.y, transform.rotation.z  + temp2.z);
	}
}
