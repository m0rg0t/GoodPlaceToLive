using System;
using System.Collections.Generic;
using System.Text;

namespace GoodPlaceToLive.Common
{
    public static class DistanceHelper
    {
        public static double Distance(double lat1, double lon1, double lat2, double lon2, char unit) {
          if (lat1 == lat2 && lon1 == lon2 &&
              !Double.IsInfinity(lat1) && !Double.IsInfinity(lon1)) {
            return 0;
          }
          double theta = lon1 - lon2;
          double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
          // Floating-point roundoff can move the cosine outside [-1, 1].
          dist = Math.Acos(Math.Max(-1.0, Math.Min(1.0, dist)));
          dist = rad2deg(dist);
          dist = dist * 60 * 1.1515;
          if (unit == 'K') {
            dist = dist * 1.609344;
          } else if (unit == 'N') {
            dist = dist * 0.8684;
            }
          return (dist);
        }
 
//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
//::  This function converts decimal degrees to radians             :::
//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

private static double deg2rad(double deg) {
  return (deg * Math.PI / 180.0);
}

//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
//::  This function converts radians to decimal degrees             :::
//:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
private static double rad2deg(double rad) {
  return (rad / Math.PI * 180.0);
}

    }
}
