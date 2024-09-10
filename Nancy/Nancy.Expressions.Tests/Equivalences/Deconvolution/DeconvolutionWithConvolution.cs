﻿using JetBrains.Annotations;
using Xunit;
using Unipi.Nancy.MinPlusAlgebra;
using Unipi.Nancy.NetworkCalculus;

namespace Unipi.Nancy.Expressions.Tests.Equivalences.Deconvolution;


[TestSubject(typeof(Nancy.Expressions.Equivalences.DeconvolutionWithConvolution))]
public class DeconvolutionWithConvolution
{

    [Fact]
    public void ApplyEquivalence_DeconvolutionWithConvolution()
    {
        Curve f = new RateLatencyServiceCurve(1, 2);
        Curve g = new RateLatencyServiceCurve(2, 4);
        Curve h = new RateLatencyServiceCurve(3, 5);

        var e = Expressions.Deconvolution(
            f,
            Expressions.Convolution(g, h)
        );

        var eq = e.ApplyEquivalence(new Nancy.Expressions.Equivalences.DeconvolutionWithConvolution());
        
        Assert.True(e.Equivalent(eq));
        Assert.False(e == eq);
        Assert.Equal("(f ⊘ h) ⊘ g", eq.ToUnicodeString());
    }
}