﻿using System.Collections.Generic;
using System.Linq;
using Unipi.Nancy.MinPlusAlgebra;
using Unipi.Nancy.NetworkCalculus;
using Unipi.Nancy.Numerics;
using Xunit;
using Xunit.Abstractions;

namespace Unipi.Nancy.Tests.MinPlusAlgebra.Curves;

public partial class ConvolutionIsomorphism
{
    private readonly ITestOutputHelper output;

    public ConvolutionIsomorphism(ITestOutputHelper output)
    {
        this.output = output;
    }

    public ComputationSettings noIsospeedSettings = ComputationSettings.Default() with
    {
        UseConvolutionIsospeedOptimization = false,
        UseSubAdditiveConvolutionOptimizations = false
    };

    public ComputationSettings convolutionIsospeedSettings = ComputationSettings.Default() with
    {
        UseConvolutionIsospeedOptimization = true,
        UseBySequenceConvolutionIsospeedOptimization = true,
        UseConvolutionSuperIsospeedOptimization = false,
        UseSubAdditiveConvolutionOptimizations = false
    };
    
    public ComputationSettings convolutionSuperIsospeedSettings = ComputationSettings.Default() with
    {
        UseConvolutionIsospeedOptimization = true,
        UseBySequenceConvolutionIsospeedOptimization = true,
        UseConvolutionSuperIsospeedOptimization = true,
        UseSubAdditiveConvolutionOptimizations = false
    };

    // original set of tests
    public static IEnumerable<object[]> ConvolutionsSetTestCases()
    {
        var testCases = new Curve[][]
        {
            new Curve[]
            {
                new StairCurve(3, 5),
                new StairCurve(7, 11)
            },
            new Curve[]
            {
                new StairCurve(5, 3),
                new StairCurve(11, 7)
            },
            new Curve[]
            {
                new StairCurve(5, 3),
                new StairCurve(7, 11)
            },
            new Curve[]
            {
                new StairCurve(3, new Rational(5, 11)),
                new StairCurve(16, 2)
            },
            // new Curve[]
            // { 
            //     new StairCurve(1, new Rational(23, 29)),
            //     new StairCurve(120, new Rational(17, 19))
            // },
            new Curve[]
            {
                new Curve(
                    baseSequence: new Sequence(
                        new Element[]
                        {
                            Point.Origin(),
                            new Segment(0, 1, 0, 3),
                            new Point(1, 3),
                            new Segment(1, 2, 3, 2)
                        }),
                    pseudoPeriodStart: 0,
                    pseudoPeriodHeight: 5,
                    pseudoPeriodLength: 2
                ),
                new Curve(
                    baseSequence: new Sequence(
                        new Element[]
                        {
                            Point.Origin(),
                            new Segment(0, 2, 0, 1),
                            new Point(2, 2),
                            new Segment(2, 5, 2, new Rational(1, 3))
                        }),
                    pseudoPeriodStart: 0,
                    pseudoPeriodHeight: 3,
                    pseudoPeriodLength: 5
                )
            },
            new Curve[]
            {
                new Curve(
                    baseSequence: new Sequence(
                        new Element[]
                        {
                            Point.Origin(),
                            new Segment(0, 1, 0, 3),
                            new Point(1, 3),
                            new Segment(1, 2, 3, 2)
                        }),
                    pseudoPeriodStart: 0,
                    pseudoPeriodHeight: 5,
                    pseudoPeriodLength: 2
                ),
                new Curve(
                    baseSequence: new Sequence(
                        new Element[]
                        {
                            Point.Origin(),
                            new Segment(0, 2, 0, 1),
                            new Point(2, 2),
                            new Segment(2, 5, 2, new Rational(1, 3))
                        }),
                    pseudoPeriodStart: 0,
                    pseudoPeriodHeight: 3,
                    pseudoPeriodLength: 5
                )
            },
            new Curve[]
            {
                new FlowControlCurve(4, 12, 4),
                new FlowControlCurve(3, 12, 3),
                new FlowControlCurve(4, 12, 4)
            },
            new Curve[]
            {
                new FlowControlCurve(4, 12, 4),
                new FlowControlCurve(3,12, 3)
            },
            new Curve[]
            {
                new FlowControlCurve(4, 12, 4),
                new FlowControlCurve(3,11, 3)
            },
            new Curve[]
            {
                new FlowControlCurve(5, 12, 4),
                new FlowControlCurve(3,11, 3),
                new FlowControlCurve(3, 3, 2)
            },
            new Curve[]
            {
                new FlowControlCurve(3, 3, 2),
                new FlowControlCurve(3,5, 5)
            },
            new Curve[]
            {
                new SigmaRhoArrivalCurve(3, 2),
                new RateLatencyServiceCurve(4, 2)
            },
            new Curve[]
            {
                new FlowControlCurve(4, 12, 3),
                new FlowControlCurve(3, 12, 4) *
                    new RateLatencyServiceCurve( 12, 3)
            },
            new Curve[]
            {
                new FlowControlCurve(1, 12, 3),
                new FlowControlCurve(1,12, 4) *
                    new RateLatencyServiceCurve(12, 1)
            },
            new Curve[]
            {
                new Curve(
                    baseSequence: new FlowControlCurve(3, 3, 2).Cut(0, 5 + 3),
                    pseudoPeriodStart: 5,
                    pseudoPeriodLength: 3,
                    pseudoPeriodHeight: 2
                ),
                new Curve(
                    baseSequence: new FlowControlCurve(3,5, 5).Cut(0, 10 + 3),
                    pseudoPeriodStart: 10,
                    pseudoPeriodLength: 3,
                    pseudoPeriodHeight: 5
                )
            },
            new Curve[]
            {
                new Curve(
                    baseSequence: new StairCurve(3, new Rational(5, 11)).Cut(0,2 + new Rational(5, 11)),
                    pseudoPeriodStart: 2,
                    pseudoPeriodLength: new Rational(5, 11),
                    pseudoPeriodHeight: 3
                ),
                new StairCurve(16, 2)
            }

            #if !SKIP_LONG_TESTS
            new []
            {
                new FlowControlCurve(3, 3, 2),
                new FlowControlCurve(3,5, 5),
                new FlowControlCurve(416, 835, 313)
            }
            #endif
        };

        foreach (var testCase in testCases)
        {
            yield return new object[] { testCase };
        }
    }

    // new set from IsomorphismOverInterval investigation
    public static List<Curve> ContinuousExamples = new List<Curve>()
    {
        // a
        new Curve(
            baseSequence: new Sequence(new List<Element>()
            {
                Point.Origin(),
                Segment.Zero(0,1),
                new Point(1, 0),
                new Segment(1, 3, 0, 0.5m),
                new Point(3, 1),
                new Segment(3, 4, 1, 1),
                new Point(4, 2),
                Segment.Constant(4, 5, 2)
            }),
            pseudoPeriodStart: 3,
            pseudoPeriodLength: 2,
            pseudoPeriodHeight: 1
        ),
        // b
        new Curve(
            baseSequence: new Sequence(new List<Element>()
            {
                Point.Origin(),
                new Segment(0, 1, 0, 1),
                new Point(1, 1),
                Segment.Constant(1, 3, 1),
                new Point(3, 1),
                new Segment(3,4, 1, 1)
            }),
            pseudoPeriodStart: 2,
            pseudoPeriodLength: 2,
            pseudoPeriodHeight: 1
        ),
        // b with T delayed
        new Curve(
            baseSequence: new Sequence(new List<Element>()
            {
                Point.Origin(),
                new Segment(0,1, 0, 1),
                new Point(1, 1),
                Segment.Constant(1, 3, 1),
                new Point(3, 1),
                new Segment(3,4, 1, 1),
                new Point(4, 2),
                Segment.Constant(4, 5, 2),
            }),
            pseudoPeriodStart: 3,
            pseudoPeriodLength: 2,
            pseudoPeriodHeight: 1
        ),
        // b modified to break the T = sup{} expression
        new Curve(
            baseSequence: new Sequence(new List<Element>()
            {
                Point.Origin(),
                new Segment(0, 1, 0, 1),
                new Point(1, 1),
                Segment.Constant(1, 2, 1),
                new Point(2, 1),
                new Segment(2, 3, 1, 0.25m),
                new Point(3, 1.25m),
                new Segment(3,4, 1.25m, 0.75m)
            }),
            pseudoPeriodStart: 2,
            pseudoPeriodLength: 2,
            pseudoPeriodHeight: 1
        ),
        // b modified to break the T = sup{} expression, with different slope
        new Curve(
            baseSequence: new Sequence(new List<Element>()
            {
                Point.Origin(),
                new Segment(0, 1, 0, 1),
                new Point(1, 1),
                Segment.Constant(1, 2, 1),
                new Point(2, 1),
                new Segment(2, 3, 1, 1),
                new Point(3, 2),
                new Segment(3,4, 2, 3)
            }),
            pseudoPeriodStart: 2,
            pseudoPeriodLength: 2,
            pseudoPeriodHeight: 4
        ),
        // b modified to break the T = sup{} expression, with different slope
        new Curve(
            baseSequence: new Sequence(new List<Element>()
            {
                Point.Origin(),
                new Segment(0, 1, 0, 1),
                new Point(1, 1),
                Segment.Constant(1, 2, 1),
                new Point(2, 1),
                new Segment(2, 2.5m, 1, 0.5m),
                new Point(2.5m, 1.25m),
                new Segment(2.5m,3, 1.25m, 1.5m)
            }),
            pseudoPeriodStart: 2,
            pseudoPeriodLength: 1,
            pseudoPeriodHeight: 1
        ),
        new Curve(
            baseSequence: new Sequence(new List<Element>()
            {
                Point.Origin(),
                new Segment(0,1, 0, 1),
                new Point(1, 1),
                Segment.Constant(1, 3, 1),
                new Point(3, 1),
                new Segment(3,4, 1, 1),
                new Point(4, 2),
                Segment.Constant(4, 5, 2),
            }),
            pseudoPeriodStart: 2,
            pseudoPeriodLength: 3,
            pseudoPeriodHeight: 1
        ), 
        // T = 0, f(0) > 0
        new RateLatencyServiceCurve(2, 4).UpperPseudoInverse(),
        new RateLatencyServiceCurve(5, 6).UpperPseudoInverse(),
        new Curve(
            baseSequence: new Sequence(new List<Element>()
            {
                new Point(0, 2),
                new Segment(0,1, 2, 1),
                new Point(1, 3),
                Segment.Constant(1, 3, 3)
            }),
            pseudoPeriodStart: 0,
            pseudoPeriodLength: 3,
            pseudoPeriodHeight: 1
        ),
        new Curve(
            baseSequence: new Sequence(new List<Element>()
            {
                new Point(0, 2),
                Segment.Constant(0, 2, 2),
                new Point(2, 2),
                new Segment(2,3, 2, 1),
            }),
            pseudoPeriodStart: 0,
            pseudoPeriodLength: 3,
            pseudoPeriodHeight: 1
        ),
        // T = 0, f(0) = 0
        new RateLatencyServiceCurve(2, 0).UpperPseudoInverse(),
        // T > 0, f(0) > 0
        new Curve(
            baseSequence: new Sequence(new List<Element>()
            {
                Point.Origin(),
                new Segment(0,1, 0, 1),
                new Point(1, 1),
                Segment.Constant(1, 3, 1),
                new Point(3, 1),
                new Segment(3,4, 1, 1),
                new Point(4, 2),
                Segment.Constant(4, 5, 2),
            }),
            pseudoPeriodStart: 2,
            pseudoPeriodLength: 3,
            pseudoPeriodHeight: 1
        )
        .VerticalShift(3, false),
        new Curve(
            baseSequence: new Sequence(new List<Element>()
            {
                Point.Origin(),
                Segment.Zero(0,1),
                new Point(1, 0),
                new Segment(1, 3, 0, 0.5m),
                new Point(3, 1),
                new Segment(3, 4, 1, 1),
                new Point(4, 2),
                Segment.Constant(4, 5, 2)
            }),
            pseudoPeriodStart: 3,
            pseudoPeriodLength: 2,
            pseudoPeriodHeight: 1
        )
        .VerticalShift(2, false),
    };

    public static List<Curve> LeftContinuousExamples = new List<Curve>()
    {
        new StairCurve(2, 3),
                // jumps at the start of the period
        new Curve(
            baseSequence: new Sequence(new List<Element>()
            {
                Point.Origin(),
                new Segment(0, 1, 0, 1),
                new Point(1, 1),
                new Segment(1, 2, 2, 2),
            }),
            pseudoPeriodStart: 1,
            pseudoPeriodLength: 1,
            pseudoPeriodHeight: 3
        ),
        // jumps at the start of the period, high slope
        new Curve(
            baseSequence: new Sequence(new List<Element>()
            {
                Point.Origin(),
                new Segment(0, 1, 0, 5),
                new Point(1, 5),
                new Segment(1, 2, 10, 7),
            }),
            pseudoPeriodStart: 1,
            pseudoPeriodLength: 1,
            pseudoPeriodHeight: 12
        ),
        // jumps at the start of the period, low slope
        new Curve(
            baseSequence: new Sequence(new List<Element>()
            {
                Point.Origin(),
                new Segment(0, 10, 0, 0.1m),
                new Point(10, 1),
                new Segment(10, 15, 2, 0.2m),
            }),
            pseudoPeriodStart: 10,
            pseudoPeriodLength: 5,
            pseudoPeriodHeight: 2
        ),
        // jumps and constants, staircase behavior in the period
        // T is set at the start of the first plateau
        new Curve(
            baseSequence: new Sequence(new List<Element>()
            {
                Point.Origin(),
                new Segment(0,1, 0, 1),
                new Point(1, 1),
                Segment.Constant(1, 3, 2),
            }),
            pseudoPeriodStart: 1,
            pseudoPeriodLength: 2,
            pseudoPeriodHeight: 1
        ),
        // jumps and constants, staircase behavior in the period
        // T is set in-between the first plateau
        new Curve(
            baseSequence: new Sequence(new List<Element>()
            {
                Point.Origin(),
                new Segment(0,1, 0, 1),
                new Point(1, 1),
                Segment.Constant(1, 3, 2),
                new Point(3, 2),
                Segment.Constant(3, 4, 3),
            }),
            pseudoPeriodStart: 2,
            pseudoPeriodLength: 2,
            pseudoPeriodHeight: 1
        ),
        // jumps and constants, staircase behavior in the period
        // T is set at the start of the second plateau
        new Curve(
            baseSequence: new Sequence(new List<Element>()
            {
                Point.Origin(),
                new Segment(0,1, 0, 1),
                new Point(1, 1),
                Segment.Constant(1, 3, 2),
                new Point(3, 2),
                Segment.Constant(3, 5, 3),
            }),
            pseudoPeriodStart: 3,
            pseudoPeriodLength: 2,
            pseudoPeriodHeight: 1
        ),
        // jumps and constants, staircase behavior in the period
        // the jump is at 3/4 of the period
        new Curve(
            baseSequence: new Sequence(new List<Element>()
            {
                Point.Origin(),
                new Segment(0,2, 0, 1),
                new Point(2, 2),
                Segment.Constant(2, 3.5m, 2),
                new Point(3.5m, 2),
                Segment.Constant(3.5m, 4, 3),
            }),
            pseudoPeriodStart: 2,
            pseudoPeriodLength: 2,
            pseudoPeriodHeight: 1
        ),
    };

    public static List<Curve> RightContinuousExamples = new List<Curve>()
    {
        new StairCurve(2, 3).UpperPseudoInverse(),
        // jumps and constants, staircase behavior in the period
        // T is set at the start of the first plateau
        new Curve(
            baseSequence: new Sequence(new List<Element>()
            {
                Point.Origin(),
                new Segment(0,1, 0, 1),
                new Point(1, 2),
                Segment.Constant(1, 3, 2)
            }),
            pseudoPeriodStart: 1,
            pseudoPeriodLength: 2,
            pseudoPeriodHeight: 1
        ),
        // jumps and constants, staircase behavior in the period
        // T is set in-between the first plateau
        new Curve(
            baseSequence: new Sequence(new List<Element>()
            {
                Point.Origin(),
                new Segment(0,1, 0, 1),
                new Point(1, 2),
                Segment.Constant(1, 3, 2),
                new Point(3, 3),
                Segment.Constant(3, 4, 3),
            }),
            pseudoPeriodStart: 2,
            pseudoPeriodLength: 2,
            pseudoPeriodHeight: 1
        ),
        // jumps and constants, staircase behavior in the period
        // T is set at the start of the second plateau
        new Curve(
            baseSequence: new Sequence(new List<Element>()
            {
                Point.Origin(),
                new Segment(0,1, 0, 1),
                new Point(1, 2),
                Segment.Constant(1, 3, 2),
                new Point(3, 3),
                Segment.Constant(3, 5, 3),
            }),
            pseudoPeriodStart: 3,
            pseudoPeriodLength: 2,
            pseudoPeriodHeight: 1
        ),
    };
}