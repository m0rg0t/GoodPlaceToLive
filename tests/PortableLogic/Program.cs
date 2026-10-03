using System;
using System.Collections.Generic;
using GoodPlaceToLive.Common;
using GoodPlaceToLive.Models;
class Program
{
    static int checks, failures;
    static void Check(bool ok, string label) { checks++; if (!ok) { failures++; Console.WriteLine("FAIL: " + label); } }
    static void Main()
    {
        var item = new BasePlaceItem();
        var names = new List<string>();
        item.PropertyChanged += (sender, args) => names.Add(args.PropertyName);
        item.ContractSum = 120;
        Check(names.Contains("ContractSumString"), "contract amount updates formatted binding");
        Check(names.Contains("PlaceCoefficient") && names.Contains("PlaceCoefficientString"), "contract amount updates coefficient bindings");
        names.Clear(); item.Distance = 3;
        Check(names.Contains("Distance"), "distance notification");
        Check(names.Contains("PlaceCoefficient") && names.Contains("PlaceCoefficientString"), "distance updates coefficient bindings");
        Check(item.PlaceCoefficient == 40, "coefficient uses current distance");
        item.Distance = 0; Check(item.PlaceCoefficient == 120, "zero-distance coefficient behavior retained");
        bool finite = true, zero = true;
        for (int n = -900; n <= 900; n++) {
            double lat = n / 10.0;
            double d = DistanceHelper.Distance(lat, 10, lat, 10, 'K');
            finite &= !Double.IsNaN(d) && !Double.IsInfinity(d);
            zero &= d == 0;
        }
        Check(finite, "identical synthetic coordinates never yield NaN");
        Check(zero, "identical synthetic coordinates return zero");
        double km = DistanceHelper.Distance(0, 0, 0, 1, 'K');
        Check(Math.Abs(km - 111.18957696) < .001, "one-degree equatorial distance retains conversion");
        double mi = DistanceHelper.Distance(0, 0, 0, 1, 'M');
        Check(Math.Abs(km / mi - 1.609344) < 1e-10, "kilometer conversion");
        Check(Math.Abs(DistanceHelper.Distance(0,0,0,1,'N') / mi - .8684) < 1e-10, "nautical conversion");
        Check(Math.Abs(DistanceHelper.Distance(10,20,-30,40,'K') - DistanceHelper.Distance(-30,40,10,20,'K')) < 1e-8, "symmetric distance");
        Check(!Double.IsNaN(DistanceHelper.Distance(10,20,-10,200,'K')), "antipodal distance finite");
        Check(Double.IsNaN(DistanceHelper.Distance(Double.NaN,0,1,1,'K')), "nonfinite input is not silently converted to zero");
        Console.WriteLine(checks + " checks; " + failures + " failures");
        Environment.ExitCode = failures == 0 ? 0 : 1;
    }
}
