// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable ReturnValueOfPureMethodIsNotUsed

using System;

namespace RoslynRanger.Sample;

public class Examples
{
    public static void MathRoundMidpointExample()
    {
        Math.Round(1.2345); // Should trigger analyzer
        Math.Round(1.2345, 4); // Should trigger analyzer

        Math.Round(1.2345, MidpointRounding.AwayFromZero);
        Math.Round(1.2345, MidpointRounding.ToEven);
        Math.Round(1.2345, MidpointRounding.ToZero);
        Math.Round(1.2345, MidpointRounding.ToNegativeInfinity);
        Math.Round(1.2345, MidpointRounding.ToPositiveInfinity);
    }
}
