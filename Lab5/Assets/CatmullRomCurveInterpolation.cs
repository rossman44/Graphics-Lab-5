using UnityEngine;
using System.Collections;

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
	
	/* Returns a point on a cubic Catmull-Rom/Blended Parabolas curve
	 * u is a scalar value from 0 to 1
	 * segment_number indicates which 4 points to use for interpolation
	 */
	Vector3 ComputePointOnCatmullRomCurve(double u, int segmentNumber)
	{
		Vector3 point = new Vector3();

		u = u - (int)u;

		double tt = u * u;
		double ttt = u * u * u;

		double q1 = -ttt + 2.0 * tt - u;
		double q2 = 3.0 * ttt - 5.0 * tt + 2.0;
		double q3 = -3.0f * ttt + 4.0f * tt + u;
		double q4 = ttt - tt;

		float fq1 = (float)q1;
		float fq2 = (float)q2;
		float fq3 = (float)q3;
		float fq4 = (float)q4;


		point.x = 0.5f * controlPoints [segmentNumber % NumberOfPoints].x * fq1 + controlPoints [(segmentNumber + 1) % NumberOfPoints].x * fq2 + controlPoints [(segmentNumber + 2) % NumberOfPoints].x * fq3 + controlPoints [(segmentNumber + 3) % NumberOfPoints].x * fq4;
		point.y = 0.5f * controlPoints [segmentNumber % NumberOfPoints].y * fq1 + controlPoints [(segmentNumber + 1) % NumberOfPoints].y * fq2 + controlPoints [(segmentNumber + 2) % NumberOfPoints].y * fq3 + controlPoints [(segmentNumber + 3) % NumberOfPoints].y * fq4;
		point.z = 0.5f * controlPoints [segmentNumber % NumberOfPoints].z * fq1 + controlPoints [(segmentNumber + 1) % NumberOfPoints].z * fq2 + controlPoints [(segmentNumber + 2) % NumberOfPoints].z * fq3 + controlPoints [(segmentNumber + 3) % NumberOfPoints].z * fq4;

		//print (segmentLength [segmentNumber]);
		point.x = point.x * ((float)(segmentLength [segmentNumber]) / (float)longestSpline);
		point.y = point.y * ((float)(segmentLength [segmentNumber]) / (float)longestSpline);
		point.z = point.z * ((float)(segmentLength [segmentNumber]) / (float)longestSpline);


		// TODO - compute and return a point as a Vector3		
		// Hint: Points on segment number 0 start at controlPoints[0] and end at controlPoints[1]
		//		 Points on segment number 1 start at controlPoints[1] and end at controlPoints[2]
		//		 etc...
		
		return point;
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
		Vector3 targetPoint = new Vector3();
		double t = time - (int)time;
		Vector3 currentPoint = ComputePointOnCatmullRomCurve (time - t, segmentNumber);

		while (count < 1.0f) {
			targetPoint = ComputePointOnCatmullRomCurve (time, segmentNumber);
			segLength = segLength + Mathf.Sqrt ((targetPoint.x - currentPoint.x) * (targetPoint.x - currentPoint.x)
			+ (targetPoint.y - currentPoint.y) * (targetPoint.y - currentPoint.y)
			+ (targetPoint.z - currentPoint.z) * (targetPoint.z - currentPoint.z));
			print (segLength);
			count += 0.005f;
			currentPoint = targetPoint;
		}

		return segLength;

	}

	float normaizeDistance(float position) {
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

		// set points randomly...
		controlPoints[0] = new Vector3(0,0,0);
		for(int i = 1; i < NumberOfPoints; i++)
		{
			controlPoints[i] = new Vector3(Random.Range(MinX,MaxX),Random.Range(MinY,MaxY),Random.Range(MinZ,MaxZ));
			segmentLength[i] = CalculateSegmentLength (i);
			if (segmentLength [i] > longestSpline) {
				longestSpline = segmentLength [i];
			}
		}



//		controlPoints[0] = new Vector3(0,0,0);
//		controlPoints[1] = new Vector3(0.5f,0,1);
//		controlPoints[2] = new Vector3(1f,0,1.50f);
//		controlPoints[3] = new Vector3(1f,0,1.75f);
//		controlPoints[4] = new Vector3(1.80f,0,2.4f);
//		controlPoints[5] = new Vector3(2.70f,0,3.00f);
//		controlPoints[6] = new Vector3(3.50f,0,4.00f);
//		controlPoints[7] = new Vector3(4.20f,0,5.00f);

		
		GenerateControlPointGeometry();
	}
	
	// Update is called once per frame
	void Update () {
		
		time += DT;
			
		// TODO - use time to determine values for u and segment_number in this function call
		//int u = (int)time;
		//comment

		
		Vector3 temp = ComputePointOnCatmullRomCurve(time, (int)time);
		transform.position = temp;
	}
}
