﻿using System;
using System.Collections.Generic;
using Unipi.Nancy.MinPlusAlgebra;
using Unipi.Nancy.NetworkCalculus;
using Unipi.Nancy.Numerics;
using Unipi.Nancy.Tests.MinPlusAlgebra.Curves;
using Xunit;
using Xunit.Abstractions;

namespace Unipi.Nancy.Tests.MinPlusAlgebra.Curves;

public class CurvePseudoInverseInvolutiveProperties
{
    private readonly ITestOutputHelper output;

    public CurvePseudoInverseInvolutiveProperties(ITestOutputHelper output)
    {
        this.output = output;
    }

    public static IEnumerable<object[]> LeftContinuousFunctions()
    {
        foreach (var objArray in CurveUpperPseudoInverse.LeftContinuousTestCases())
        {
            var f = (Curve) objArray[0];
            yield return new object[] { f };
        }
    }

    [Theory]
    [MemberData(nameof(LeftContinuousFunctions))]
    public void LpiOfUpiOfLeftContinuous(Curve f)
    {
        Assert.True(f.IsLeftContinuous);
        var upi = f.UpperPseudoInverse();
        var lpi = upi.LowerPseudoInverse();

        #if false
            // Output code for plot-debugging
            output.WriteLine($"var f = {f.ToCodeString()};");
            output.WriteLine($"var upi = {upi.ToCodeString()};");
            output.WriteLine($"var lpi = {lpi.ToCodeString()};");
        #endif

        Assert.True(Curve.Equivalent(f, lpi));
    }

    public static IEnumerable<object[]> RightContinuousFunctions()
    {
        foreach (var objArray in CurveUpperPseudoInverse.RightContinuousTestCases())
        {
            var f = (Curve) objArray[0];
            yield return new object[] { f };
        }
    }

    [Theory]
    [MemberData(nameof(RightContinuousFunctions))]
    public void UpiOfLpiOfRightContinuous(Curve f)
    {
        Assert.True(f.IsRightContinuous);
        var lpi = f.LowerPseudoInverse();
        var upi = lpi.UpperPseudoInverse();

        #if false
            // Output code for plot-debugging
            output.WriteLine($"var f = {f.ToCodeString()};");
            output.WriteLine($"var upi = {upi.ToCodeString()};");
            output.WriteLine($"var lpi = {lpi.ToCodeString()};");
        #endif

        Assert.True(Curve.Equivalent(f, upi));
    }
}