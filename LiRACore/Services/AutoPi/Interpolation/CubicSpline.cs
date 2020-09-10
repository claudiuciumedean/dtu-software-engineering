//
// Author: Ryan Seghers
//
// Copyright (C) 2013-2014 Ryan Seghers
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the irrevocable, perpetual, worldwide, and royalty-free
// rights to use, copy, modify, merge, publish, distribute, sublicense, 
// display, perform, create derivative works from and/or sell copies of 
// the Software, both in source and object code form, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using LiRACore.Models.RawData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LiRACore.Services.AutoPi.Interpolation
{
    /// <summary>
    /// Cubic spline interpolation.
    /// Call Fit (or use the corrector constructor) to compute spline coefficients, then Eval to evaluate the spline at other X coordinates.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is implemented based on the wikipedia article:
    /// http://en.wikipedia.org/wiki/Spline_interpolation
    /// I'm not sure I have the right to include a copy of the article so the equation numbers referenced in 
    /// comments will end up being wrong at some point.
    /// </para>
    /// <para>
    /// This is not optimized, and is not MT safe.
    /// This can extrapolate off the ends of the splines.
    /// You must provide points in X sort order.
    /// </para>
    /// </remarks>
    public class CubicSpline
	{
		#region Fields

		// N-1 spline coefficients for N points
		private float?[] a;
		private float?[] b;

		// Save the original x and y for Eval
		private float[] xOrig;
		private float?[] yOrig;

		#endregion

		#region Ctor

		/// <summary>
		/// Default ctor.
		/// </summary>
		public CubicSpline()
		{
		}

		/// <summary>
		/// Construct and call Fit.
		/// </summary>
		/// <param name="x">Input. X coordinates to fit.</param>
		/// <param name="y">Input. Y coordinates to fit.</param>
		/// <param name="startSlope">Optional slope constraint for the first point. Single.NaN means no constraint.</param>
		/// <param name="endSlope">Optional slope constraint for the final point. Single.NaN means no constraint.</param>
		/// <param name="debug">Turn on console output. Default is false.</param>
		public CubicSpline(float[] x, float?[] y, float startSlope = float.NaN, float endSlope = float.NaN, bool debug = false)
		{
			Fit(x, y, startSlope, endSlope, debug);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Throws if Fit has not been called.
		/// </summary>
		private void CheckAlreadyFitted()
		{
			if (a == null) throw new Exception("Fit must be called before you can evaluate.");
		}

		private int _lastIndex = 0;

		/// <summary>
		/// Find where in xOrig the specified x falls, by simultaneous traverse.
		/// This allows xs to be less than x[0] and/or greater than x[n-1]. So allows extrapolation.
		/// This keeps state, so requires that x be sorted and xs called in ascending order, and is not multi-thread safe.
        /// notice: example n=5 (5 actual coordinates) we have coordinates indices (0,1,2,3,4), and splines indices (0,1,2,3)
		/// </summary>

		private int GetNextXIndex(float x)
		{
            //extra line for xs which are less than x[0] 
            if (x < 0)
            {
               return _lastIndex = 0;
            }

            else
            {
                if (x < xOrig[_lastIndex])

                {
                    //return _lastIndex = 0;
                    throw new ArgumentException("The X values to evaluate must be sorted.");
                }


                while ((_lastIndex < xOrig.Length - 2) && (x > xOrig[_lastIndex + 1]))
                {
                    _lastIndex++;
                }

                return _lastIndex;
            }
            
		}

		/// <summary>
		/// Evaluate the specified x value using the specified spline.
		/// </summary>
		/// <param name="x">The x value.</param>
		/// <param name="j">Which spline to use.</param>
		/// <param name="debug">Turn on console output. Default is false.</param>
		/// <returns>The y value.</returns>
		private float? EvalSpline(float x, int j, bool debug = false)
		{
			float? dx = xOrig[j + 1] - xOrig[j];
            // changed for xs less than 0
			float? t = Math.Abs (x - xOrig[j]) / dx;
			float? y = (1 - t) * yOrig[j] + t * yOrig[j + 1] + t * (1 - t) * (a[j] * (1 - t) + b[j] * t); // equation 9
			if (debug) Console.WriteLine("xs = {0}, j = {1}, t = {2}", x, j, t);
			return y;
		}

		#endregion

		#region Fit*

		/// <summary>
		/// Fit x,y and then eval at points xs and return the corresponding y's.
		/// This does the "natural spline" style for ends.
		/// This can extrapolate off the ends of the splines.
		/// You must provide points in X sort order.
		/// </summary>
		/// <param name="x">Input. X coordinates to fit.</param>
		/// <param name="y">Input. Y coordinates to fit.</param>
		/// <param name="xs">Input. X coordinates to evaluate the fitted curve at.</param>
		/// <param name="startSlope">Optional slope constraint for the first point. Single.NaN means no constraint.</param>
		/// <param name="endSlope">Optional slope constraint for the final point. Single.NaN means no constraint.</param>
		/// <param name="debug">Turn on console output. Default is false.</param>
		/// <returns>The computed y values for each xs.</returns>
		//public float[] FitAndEval(float[] x, float[] y, float[] xs, float startSlope = float.NaN, float endSlope = float.NaN, bool debug = false)
		//{
		//	Fit(x, y, startSlope, endSlope, debug);
		//	return Eval(xs, debug);
		//}


        public List<Measurement> FitAndEval(float[] x, float?[] y, float[] xs, List<Measurement> interpolate_list,bool isX_Lat, float startSlope = float.NaN, float endSlope = float.NaN, bool debug = false)
        {
            Fit(x, y, startSlope, endSlope, debug);

            return Eval(xs, interpolate_list, isX_Lat, debug);
        }

        /// <summary>
        /// Compute spline coefficients for the specified x,y points.
        /// This does the "natural spline" style for ends.
        /// This can extrapolate off the ends of the splines.
        /// You must provide points in X sort order.
        /// </summary>
        /// <param name="x">Input. X coordinates to fit.</param>
        /// <param name="y">Input. Y coordinates to fit.</param>
        /// <param name="startSlope">Optional slope constraint for the first point. Single.NaN means no constraint.</param>
        /// <param name="endSlope">Optional slope constraint for the final point. Single.NaN means no constraint.</param>
        /// <param name="debug">Turn on console output. Default is false.</param>
  //      public void Fit(float[] x, float[] y,  List<Interpolated> nonIterpolated_List, List<Interpolated> interpolated_List, bool isLati,float startSlope = float.NaN, float endSlope = float.NaN, bool debug = false)
   
		//{
		//	if (Single.IsInfinity(startSlope) || Single.IsInfinity(endSlope))
		//	{
		//		throw new Exception("startSlope and endSlope cannot be infinity.");
		//	}

		//	// Save x and y for eval
		//	this.xOrig = x;
		//	this.yOrig = y;

		//	int n = x.Length;
		//	float[] r = new float[n]; // the right hand side numbers: wikipedia page overloads b

		//	TriDiagonalMatrixF m = new TriDiagonalMatrixF(n);
		//	float dx1, dx2, dy1, dy2;

		//	// First row is different (equation 16 from the article)
		//	if (float.IsNaN(startSlope))
		//	{
		//		dx1 = x[1] - x[0];
		//		m.C[0] = 1.0f / dx1;
		//		m.B[0] = 2.0f * m.C[0];
		//		r[0] = 3 * (y[1] - y[0]) / (dx1 * dx1);
		//	}
		//	else
		//	{
		//		m.B[0] = 1;
		//		r[0] = startSlope;
		//	}

  //          float yValuea;
  //          float yValuePlus;
  //          float yValueMinus;
  //          // Body rows (equation 15 from the article)
  //          for (int i = 1; i < n - 1; i++)
  //          {
  //              //y[i]
  //              yValuea = yValue(nonIterpolated_List, isLati, i);

  //              dx1 = x[i] - x[i - 1];
  //              dx2 = x[i + 1] - x[i];

  //              m.A[i] = 1.0f / dx1;
  //              m.C[i] = 1.0f / dx2;
  //              m.B[i] = 2.0f * (m.A[i] + m.C[i]);

  //              dy1 = y[i] - y[i - 1];
  //              dy2 = y[i + 1] - y[i];
  //              r[i] = 3 * (dy1 / (dx1 * dx1) + dy2 / (dx2 * dx2));
  //          }

  //          // Last row also different (equation 17 from the article)
  //          if (float.IsNaN(endSlope))
		//	{
		//		dx1 = x[n - 1] - x[n - 2];
		//		dy1 = y[n - 1] - y[n - 2];
		//		m.A[n - 1] = 1.0f / dx1;
		//		m.B[n - 1] = 2.0f * m.A[n - 1];
		//		r[n - 1] = 3 * (dy1 / (dx1 * dx1));
		//	}
		//	else
		//	{
		//		m.B[n - 1] = 1;
		//		r[n - 1] = endSlope;
		//	}

		//	if (debug) Console.WriteLine("Tri-diagonal matrix:\n{0}", m.ToDisplayString(":0.0000", "  "));
		//	if (debug) Console.WriteLine("r: {0}", ArrayUtil.ToString<float>(r));

		//	// k is the solution to the matrix
		//	float[] k = m.Solve(r);
		//	if (debug) Console.WriteLine("k = {0}", ArrayUtil.ToString<float>(k));

		//	// a and b are each spline's coefficients
		//	this.a = new float[n - 1];
		//	this.b = new float[n - 1];

		//	for (int i = 1; i < n; i++)
		//	{
		//		dx1 = x[i] - x[i - 1];
		//		dy1 = y[i] - y[i - 1];
		//		a[i - 1] = k[i - 1] * dx1 - dy1; // equation 10 from the article
		//		b[i - 1] = -k[i] * dx1 + dy1; // equation 11 from the article
		//	}

		//	if (debug) Console.WriteLine("a: {0}", ArrayUtil.ToString<float>(a));
		//	if (debug) Console.WriteLine("b: {0}", ArrayUtil.ToString<float>(b));
		//}

        private static float yValue(List<Measurement> nonIterpolated_List, bool isLati, int i)
        {
            return (isLati) ? nonIterpolated_List[i].lat.Value : nonIterpolated_List[i].lon.Value;
        }


        public void Fit(float[] x, float?[] y, float startSlope = float.NaN, float endSlope = float.NaN, bool debug = false)
        {
            if (Single.IsInfinity(startSlope) || Single.IsInfinity(endSlope))
            {
                throw new Exception("startSlope and endSlope cannot be infinity.");
            }

            // Save x and y for eval
            this.xOrig = x;
            this.yOrig = y;

            int n = x.Length;
            float?[] r = new float?[n]; // the right hand side numbers: wikipedia page overloads b

            TriDiagonalMatrixF m = new TriDiagonalMatrixF(n);
            float? dx1, dx2, dy1, dy2;

            // First row is different (equation 16 from the article)
            if (float.IsNaN(startSlope))
            {
                dx1 = x[1] - x[0];
                m.C[0] = 1.0f / dx1.Value;
                m.B[0] = 2.0f * m.C[0];
                r[0] = 3 * (y[1] - y[0]) / (dx1 * dx1);
            }
            else
            {
                m.B[0] = 1;
                r[0] = startSlope;
            }

            // Body rows (equation 15 from the article)
            for (int i = 1; i < n - 1; i++)
            {
                dx1 = x[i] - x[i - 1];
                dx2 = x[i + 1] - x[i];

                m.A[i] = 1.0f / dx1;
                m.C[i] = 1.0f / dx2;
                m.B[i] = 2.0f * (m.A[i] + m.C[i]);

                dy1 = y[i] - y[i - 1];
                dy2 = y[i + 1] - y[i];
                r[i] = 3 * (dy1 / (dx1 * dx1) + dy2 / (dx2 * dx2));
            }

            // Last row also different (equation 17 from the article)
            if (float.IsNaN(endSlope))
            {
                dx1 = x[n - 1] - x[n - 2];
                dy1 = y[n - 1] - y[n - 2];
                m.A[n - 1] = 1.0f / dx1.Value;
                m.B[n - 1] = 2.0f * m.A[n - 1];
                r[n - 1] = 3 * (dy1 / (dx1 * dx1));
            }
            else
            {
                m.B[n - 1] = 1;
                r[n - 1] = endSlope;
            }

            if (debug) Console.WriteLine("Tri-diagonal matrix:\n{0}", m.ToDisplayString(":0.0000", "  "));
            if (debug) Console.WriteLine("r: {0}", ArrayUtil.ToString<float?>(r));

            // k is the solution to the matrix
            float?[] k = m.Solve(r);
            if (debug) Console.WriteLine("k = {0}", ArrayUtil.ToString<float?>(k));

            // a and b are each spline's coefficients
            this.a = new float?[n - 1];
            this.b = new float?[n - 1];

            for (int i = 1; i < n; i++)
            {
                dx1 = x[i] - x[i - 1];
                dy1 = y[i] - y[i - 1];
                a[i - 1] = k[i - 1].Value * dx1 - dy1; // equation 10 from the article
                b[i - 1] = -k[i] * dx1 + dy1; // equation 11 from the article
            }

            if (debug) Console.WriteLine("a: {0}", ArrayUtil.ToString<float?>(a));
            if (debug) Console.WriteLine("b: {0}", ArrayUtil.ToString<float?>(b));
        }

        #endregion

        #region Eval*

        /// <summary>
        /// Evaluate the spline at the specified x coordinates.
        /// This can extrapolate off the ends of the splines.
        /// You must provide X's in ascending order.
        /// The spline must already be computed before calling this, meaning you must have already called Fit() or FitAndEval().
        /// </summary>
        /// <param name="x">Input. X coordinates to evaluate the fitted curve at.</param>
        /// <param name="debug">Turn on console output. Default is false.</param>
        /// <returns>The computed y values for each x.</returns>
        public List<Measurement> Eval(float[] x, List<Measurement> interpolated_list, bool isX_Lat, bool debug = false)
		{
            // x here is xs: points which require interpolation
			CheckAlreadyFitted();

			int n = x.Length;
			float?[] y = new float?[n];
			_lastIndex = 0; // Reset simultaneous traversal in case there are multiple calls

            try
            {
                for (int i = 0; i < n; i++)
                {
                    if(i == n-1)
                    {
                        int shah = 0;
                    }
                    // Find which spline can be used to compute this x (by simultaneous traverse)
                    int j = GetNextXIndex(x[i]);

                    // Evaluate using j'th spline
                    if (isX_Lat) // latetude needs to be calculated
                    {
                        // y[i] = EvalSpline(x[i], j, debug);
                        interpolated_list [i].lat = EvalSpline(x[i], j, debug);

                    }

                    else // longtitude needs to be calculated
                    {
                        interpolated_list[i].lon = EvalSpline(x[i], j, debug);
                    }


                 }
            }
            catch (Exception ex)
            {

                throw;
            }

			return interpolated_list;
		}

		/// <summary>
		/// Evaluate (compute) the slope of the spline at the specified x coordinates.
		/// This can extrapolate off the ends of the splines.
		/// You must provide X's in ascending order.
		/// The spline must already be computed before calling this, meaning you must have already called Fit() or FitAndEval().
		/// </summary>
		/// <param name="x">Input. X coordinates to evaluate the fitted curve at.</param>
		/// <param name="debug">Turn on console output. Default is false.</param>
		/// <returns>The computed y values for each x.</returns>
		public float?[] EvalSlope(float[] x, bool debug = false)
		{
			CheckAlreadyFitted();

			int n = x.Length;
			float?[] qPrime = new float?[n];
			_lastIndex = 0; // Reset simultaneous traversal in case there are multiple calls

			for (int i = 0; i < n; i++)
			{
				// Find which spline can be used to compute this x (by simultaneous traverse)
				int j = GetNextXIndex(x[i]);

				// Evaluate using j'th spline
				float? dx = xOrig[j + 1] - xOrig[j];
				float? dy = yOrig[j + 1] - yOrig[j];
				float? t = (x[i] - xOrig[j]) / dx;

				// From equation 5 we could also compute q' (qp) which is the slope at this x
				qPrime[i] = dy / dx
					+ (1 - 2 * t) * (a[j] * (1 - t) + b[j] * t) / dx
					+ t * (1 - t) * (b[j] - a[j]) / dx;

				if (debug) Console.WriteLine("[{0}]: xs = {1}, j = {2}, t = {3}", i, x[i], j, t);
			}

			return qPrime;
		}

		#endregion

		#region Static Methods

		/// <summary>
		/// Static all-in-one method to fit the splines and evaluate at X coordinates.
		/// </summary>
		/// <param name="x">Input. X coordinates to fit.</param>
		/// <param name="y">Input. Y coordinates to fit.</param>
		/// <param name="xs">Input. X coordinates to evaluate the fitted curve at.</param>
		/// <param name="startSlope">Optional slope constraint for the first point. Single.NaN means no constraint.</param>
		/// <param name="endSlope">Optional slope constraint for the final point. Single.NaN means no constraint.</param>
		/// <param name="debug">Turn on console output. Default is false.</param>
		/// <returns>The computed y values for each xs.</returns>
		//public static float?[] Compute(float[] x, float?[] y, float[] xs, float startSlope = float.NaN, float endSlope = float.NaN, bool debug = false)
		//{
		//	CubicSpline spline = new CubicSpline();
		//	return spline.FitAndEval(x, y, xs, startSlope, endSlope, debug);
		//}


    //    public static void FitSensorDataGeometric(List<Interpolated> nonInterpolate_list, List<Interpolated> interpolate_list , out List<Interpolated> xs, out List<Interpolated> ys,
    //float firstDx = Single.NaN, float firstDy = Single.NaN, float lastDx = Single.NaN, float lastDy = Single.NaN)
    //    {
    //        // Compute distances
    //        int n = nonInterpolate_list.Count;
    //        float[] dists = new float[n]; // cumulative distance
    //        double[] dists1 = new double[n]; // cumulative distance


    //        float[] times_cumulative = new float[n]; // cumulative time



    //        dists[0] = 0;
    //        double totalDist = 0;
    //        double totalDist1 = 0;

    //        times_cumulative[0] = (float)nonInterpolate_list[0].ts.Offset.TotalMilliseconds;
    //        for (int i = 1; i < n; i++)
    //        {
    //            float? dx = nonInterpolate_list[i].lat- nonInterpolate_list[i-1].lat;
    //            float? dy = nonInterpolate_list[i].lon - nonInterpolate_list[i - 1].lon;
    //            // float dist = (float)Math.Sqrt(dx * dx + dy * dy);

    //            double dist1 = GetDistanceBetweenPoints(nonInterpolate_list[i - 1].lat.Value , nonInterpolate_list[i - 1].lon.Value, nonInterpolate_list[i].lat.Value , nonInterpolate_list[i].lon.Value );                
    //            totalDist += dist1;
    //            dists[i] = (float)totalDist;


    //            times_cumulative[i] = (float)(nonInterpolate_list[i].ts - nonInterpolate_list[i - 1].ts).TotalMilliseconds + times_cumulative[i - 1];

    //        }


            
    //        // Create 'times' to interpolate to
    //        //float dt = totalDist / (nOutputPoints - 1);

    //        float dt1 =(float)(interpolate_list[1].ts - interpolate_list[0].ts).TotalMilliseconds;

    //        float dtn = (float)(interpolate_list[n-1].ts - interpolate_list[n-2].ts).TotalMilliseconds;


    //        float[] times_interpolated = new float[interpolate_list.Count];

    //        times_interpolated[0] = (float)(interpolate_list[0].ts - nonInterpolate_list[0].ts).TotalMilliseconds;

    //        float[] d_times = new float[interpolate_list.Count];

    //        for (int i = 1; i < interpolate_list.Count; i++)
    //        {
    //            //if (t_interpolated[i] < t[t.Length -1])
    //            //{
    //                times_interpolated[i] = (float)(interpolate_list[i].ts  - interpolate_list[i - 1].ts).TotalMilliseconds + times_interpolated[i - 1];
    //           // }

    //         //   d_times[i] = (float)(t[i-1] - t[0]).TotalMilliseconds;


    //        }

                                 
    //        // Normalize the slopes, if specified
    //        NormalizeVector(ref firstDx, ref firstDy);
    //        NormalizeVector(ref lastDx, ref lastDy);

    //        // Spline fit both x and y to times
    //        CubicSpline xSpline = new CubicSpline();
    //        float?[] xx = nonInterpolate_list.Select(x => x.lat).ToArray();
    //        bool isX_Lat = true;
    //        xs = xSpline.FitAndEval(times_cumulative, xx, times_interpolated, interpolate_list, isX_Lat, firstDx / dt1, lastDx / dtn );

    //        CubicSpline ySpline = new CubicSpline();
    //        float?[] yy = nonInterpolate_list.Select(x => x.lon).ToArray();
    //         isX_Lat = false;

    //        // float?[] ys = interpolate_list.Select(x => x.lon).ToArray();
    //        ys = ySpline.FitAndEval(times_cumulative, yy, times_interpolated, interpolate_list, isX_Lat, firstDy / dt1, lastDy / dtn);
    //    }

        public static void FitSensorDataGeometric(List<Measurement> nonInterpolate_list, List<Measurement> interpolate_list, out List<Measurement> xs, out List<Measurement> ys,
  float firstDx = Single.NaN, float firstDy = Single.NaN, float lastDx = Single.NaN, float lastDy = Single.NaN)
        {
            // Compute distances
            int n = nonInterpolate_list.Count;
            float[] dists = new float[n]; // cumulative distance
            double[] dists1 = new double[n]; // cumulative distance


            float[] times_cumulative = new float[n]; // cumulative time



            dists[0] = 0;
            double totalDist = 0;
            double totalDist1 = 0;

            // TODO: check if it should be deducted from allpoints[0].ts or below is ok
            times_cumulative[0] = (float)(nonInterpolate_list[0].TS_or_Distance - interpolate_list[0].TS_or_Distance).TotalMilliseconds;
            for (int i = 1; i < n; i++)
            {
                float? dx = nonInterpolate_list[i].lat - nonInterpolate_list[i - 1].lat;
                float? dy = nonInterpolate_list[i].lon - nonInterpolate_list[i - 1].lon;
                // float dist = (float)Math.Sqrt(dx * dx + dy * dy);

                double dist1 = GetDistanceBetweenPoints(nonInterpolate_list[i - 1].lat.Value, nonInterpolate_list[i - 1].lon.Value, nonInterpolate_list[i].lat.Value, nonInterpolate_list[i].lon.Value);
                totalDist += dist1;
                dists[i] = (float)totalDist;


                times_cumulative[i] = (float)(nonInterpolate_list[i].TS_or_Distance - nonInterpolate_list[i - 1].TS_or_Distance).TotalMilliseconds + times_cumulative[i - 1];

            }



            // Create 'times' to interpolate to
            //float dt = totalDist / (nOutputPoints - 1);

            float dt1 = (float)(interpolate_list[1].TS_or_Distance - interpolate_list[0].TS_or_Distance).TotalMilliseconds;

            float dtn = (float)(interpolate_list[n - 1].TS_or_Distance - interpolate_list[n - 2].TS_or_Distance).TotalMilliseconds;


            float[] times_interpolated = new float[interpolate_list.Count];

            times_interpolated[0] = (float)(interpolate_list[0].TS_or_Distance - nonInterpolate_list[0].TS_or_Distance).TotalMilliseconds;
           //times_interpolated[0] = 0;

            float[] d_times = new float[interpolate_list.Count];

            for (int i = 1; i < interpolate_list.Count; i++)
            {
                //if (t_interpolated[i] < t[t.Length -1])
                //{
                times_interpolated[i] = (float)(interpolate_list[i].TS_or_Distance - interpolate_list[i - 1].TS_or_Distance).TotalMilliseconds + times_interpolated[i - 1];
                // }

                //   d_times[i] = (float)(t[i-1] - t[0]).TotalMilliseconds;


            }


            // Normalize the slopes, if specified
            NormalizeVector(ref firstDx, ref firstDy);
            NormalizeVector(ref lastDx, ref lastDy);

            // Spline fit both x and y to times
            CubicSpline xSpline = new CubicSpline();
            float?[] xx = nonInterpolate_list.Select(x => x.lat).ToArray();
            bool isX_Lat = true;
            xs = xSpline.FitAndEval(times_cumulative, xx, times_interpolated, interpolate_list, isX_Lat, firstDx / dt1, lastDx / dtn);

            CubicSpline ySpline = new CubicSpline();
            float?[] yy = nonInterpolate_list.Select(x => x.lon).ToArray();
            isX_Lat = false;

            // float?[] ys = interpolate_list.Select(x => x.lon).ToArray();
            ys = ySpline.FitAndEval(times_cumulative, yy, times_interpolated, interpolate_list, isX_Lat, firstDy / dt1, lastDy / dtn);
        }



        //        public static void FitSensorDataGeometric(float[] x, float[] y, DateTimeOffset[] t, DateTimeOffset[] t_interpolated, int nOutputPoints, out float[] xs, out float[] ys,
        //float firstDx = Single.NaN, float firstDy = Single.NaN, float lastDx = Single.NaN, float lastDy = Single.NaN)
        //        {
        //            // Compute distances
        //            int n = x.Length;
        //            float[] dists = new float[n]; // cumulative distance
        //            double[] dists1 = new double[n]; // cumulative distance


        //            float[] times_cumulative = new float[n]; // cumulative time



        //            dists[0] = 0;
        //            double totalDist = 0;
        //            double totalDist1 = 0;

        //            times_cumulative[0] = (float)t[0].Offset.TotalMilliseconds;
        //            for (int i = 1; i < n; i++)
        //            {
        //                float dx = x[i] - x[i - 1];
        //                float dy = y[i] - y[i - 1];
        //                // float dist = (float)Math.Sqrt(dx * dx + dy * dy);

        //                double dist1 = GetDistanceBetweenPoints(x[i - 1], y[i - 1], x[i], y[i]);
        //                totalDist += dist1;
        //                dists[i] = (float)totalDist;


        //                times_cumulative[i] = (float)(t[i] - t[i - 1]).TotalMilliseconds + times_cumulative[i - 1];

        //            }



        //            // Create 'times' to interpolate to
        //            //float dt = totalDist / (nOutputPoints - 1);

        //            float dt1 = (float)(t_interpolated[1] - t_interpolated[0]).TotalMilliseconds;

        //            float dtn = (float)(t_interpolated[n - 1] - t_interpolated[n - 2]).TotalMilliseconds;


        //            float[] times_interpolated = new float[t_interpolated.Length];

        //            times_interpolated[0] = (float)(t_interpolated[0] - t[0]).TotalMilliseconds;

        //            float[] d_times = new float[t_interpolated.Length];

        //            for (int i = 1; i < t_interpolated.Length; i++)
        //            {
        //                //if (t_interpolated[i] < t[t.Length -1])
        //                //{
        //                times_interpolated[i] = (float)(t_interpolated[i] - t_interpolated[i - 1]).TotalMilliseconds + times_interpolated[i - 1];
        //                // }

        //                //   d_times[i] = (float)(t[i-1] - t[0]).TotalMilliseconds;


        //            }


        //            // Normalize the slopes, if specified
        //            NormalizeVector(ref firstDx, ref firstDy);
        //            NormalizeVector(ref lastDx, ref lastDy);

        //            // Spline fit both x and y to times
        //            CubicSpline xSpline = new CubicSpline();
        //            xs = xSpline.FitAndEval(times_cumulative, x, times_interpolated, firstDx / dt1, lastDx / dtn);

        //            CubicSpline ySpline = new CubicSpline();
        //            ys = ySpline.FitAndEval(times_cumulative, y, times_interpolated, firstDy / dt1, lastDy / dtn);
        //        }


        /// <summary>
        /// Fit the input x,y points using the parametric approach, so that y does not have to be an explicit
        /// function of x, meaning there does not need to be a single value of y for each x.
        /// </summary>
        /// <param name="x">Input x coordinates.</param>
        /// <param name="y">Input y coordinates.</param>
        /// <param name="nOutputPoints">How many output points to create.</param>
        /// <param name="xs">Output (interpolated) x values.</param>
        /// <param name="ys">Output (interpolated) y values.</param>
        /// <param name="firstDx">Optionally specifies the first point's slope in combination with firstDy. Together they
        /// are a vector describing the direction of the parametric spline of the starting point. The vector does
        /// not need to be normalized. If either is NaN then neither is used.</param>
        /// <param name="firstDy">See description of dx0.</param>
        /// <param name="lastDx">Optionally specifies the last point's slope in combination with lastDy. Together they
        /// are a vector describing the direction of the parametric spline of the last point. The vector does
        /// not need to be normalized. If either is NaN then neither is used.</param>
        /// <param name="lastDy">See description of dxN.</param>
        //      public static void FitParametric(float[] x, float[] y, int nOutputPoints, out float[] xs, out float[] ys,
        //          float firstDx = Single.NaN, float firstDy = Single.NaN, float lastDx = Single.NaN, float lastDy = Single.NaN)
        //{
        //	// Compute distances
        //	int n = x.Length;
        //	float[] dists = new float[n]; // cumulative distance
        //	dists[0] = 0;
        //	float totalDist = 0;


        //	for (int i = 1; i < n; i++)
        //	{
        //		float dx = x[i] - x[i - 1];
        //		float dy = y[i] - y[i - 1];
        //		float dist = (float)Math.Sqrt(dx * dx + dy * dy);
        //		totalDist += dist;
        //		dists[i] = totalDist;
        //	}



        //	// Create 'times' to interpolate to
        //	float dt = totalDist / (nOutputPoints - 1);
        //	float[] times = new float[nOutputPoints];
        //	times[0] = 0;

        //	for (int i = 1; i < nOutputPoints; i++)
        //	{
        //		times[i] = times[i - 1] + dt;
        //	}

        //          // Normalize the slopes, if specified
        //          NormalizeVector(ref firstDx, ref firstDy);
        //          NormalizeVector(ref lastDx, ref lastDy);

        //	// Spline fit both x and y to times
        //	CubicSpline xSpline = new CubicSpline();
        //	xs = xSpline.FitAndEval(dists, x, times, firstDx / dt, lastDx / dt);

        //	CubicSpline ySpline = new CubicSpline();
        //	ys = ySpline.FitAndEval(dists, y, times, firstDy / dt, lastDy / dt);
        //}

        private static void NormalizeVector(ref float dx, ref float dy)
        {
            if (!Single.IsNaN(dx) && !Single.IsNaN(dy))
            {
                float d = (float)Math.Sqrt(dx * dx + dy * dy);

                if (d > Single.Epsilon) // probably not conservative enough, but catches the (0,0) case at least
                {
                    dx = dx / d;
                    dy = dy / d;
                }
                else
                {
                    throw new ArgumentException("The input vector is too small to be normalized.");
                }
            }
            else
            {
                // In case one is NaN and not the other
                dx = dy = Single.NaN;
            }
        }


        public static double GetDistanceBetweenPoints(float lat1, float long1, float lat2, float long2)
        {
            double distance = 0;

            double dLat = (lat2 - lat1) / 180 * Math.PI;
            double dLong = (long2 - long1) / 180 * Math.PI;

                                           
                double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                            + Math.Cos(lat2) * Math.Sin(dLong / 2) * Math.Sin(dLong / 2);
                double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

                //Calculate radius of earth
                // For this you can assume any of the two points.
                double radiusE = 6378135; // Equatorial radius, in metres
                double radiusP = 6356750; // Polar Radius

                //Numerator part of function
                double nr = Math.Pow(radiusE * radiusP * Math.Cos(lat1 / 180 * Math.PI), 2);
                //Denominator part of the function
                double dr = Math.Pow(radiusE * Math.Cos(lat1 / 180 * Math.PI), 2)
                                + Math.Pow(radiusP * Math.Sin(lat1 / 180 * Math.PI), 2);
                double radius = Math.Sqrt(nr / dr);

                //Calaculate distance in metres.
                distance = radius * c;
            
                return distance;
        }


        #endregion

    }
}
